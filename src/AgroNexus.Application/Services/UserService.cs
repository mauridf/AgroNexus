using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;
using AgroNexus.Domain.Entities.Identity;
using AgroNexus.Domain.Enums;
using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Application.Auth;
using Mapster;
using Microsoft.Extensions.Logging;

namespace AgroNexus.Application.Services;

/// <summary>
/// Serviço de gerenciamento de usuários.
/// Responsável por registro, login, alteração de senha e soft delete.
/// </summary>
public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        JwtTokenService jwtTokenService,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando usuário: {Email}", request.Email);

        // Validações
        if (request.Password != request.ConfirmPassword)
            throw new DomainException("Senhas não conferem.", "USER_PASSWORD_MISMATCH");

        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            throw new DomainException("Email já cadastrado.", "USER_EMAIL_EXISTS");

        if (!Enum.TryParse<UserRole>(request.Role?.ToUpperInvariant(), out var role))
            throw new DomainException("Papel inválido. Use ADM ou PRD.", "USER_ROLE_INVALID");

        // Hash da senha (BCrypt com work factor 12 = seguro e performático)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

        // Cria a entidade de domínio
        var user = User.Create(
            email: request.Email,
            passwordHash: passwordHash,
            role: role);

        await _userRepository.AddAsync(user, cancellationToken);

        _logger.LogInformation("Usuário criado com sucesso: {UserId}", user.Id);

        return user.Adapt<UserResponse>();
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tentativa de login: {Email}", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new DomainException("Email ou senha inválidos.", "LOGIN_INVALID");

        // Verifica se o usuário está ativo
        if (!user.IsActive)
            throw new DomainException("Usuário desativado. Contate o administrador.", "USER_INACTIVE");

        // Verifica a senha
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Senha inválida para usuário: {UserId}", user.Id);
            throw new DomainException("Email ou senha inválidos.", "LOGIN_INVALID");
        }

        // Registra o login
        user.RegisterLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Gera os tokens JWT
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        _logger.LogInformation("Login bem-sucedido: {UserId} ({Role})", user.Id, user.Role);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = _jwtTokenService.GetAccessTokenExpiration(),
            User = user.Adapt<UserResponse>()
        };
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Buscando usuário: {UserId}", id);

        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Usuário", id);

        return user.Adapt<UserResponse>();
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listando todos os usuários");

        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Adapt<IEnumerable<UserResponse>>();
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Alterando senha do usuário: {UserId}", userId);

        if (request.NewPassword != request.ConfirmNewPassword)
            throw new DomainException("Nova senha e confirmação não conferem.", "USER_PASSWORD_MISMATCH");

        if (request.NewPassword.Length < 8)
            throw new DomainException("Nova senha deve ter no mínimo 8 caracteres.", "USER_PASSWORD_TOO_SHORT");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Usuário", userId);

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new DomainException("Senha atual incorreta.", "USER_PASSWORD_WRONG");

        var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
        user.UpdatePassword(newHash);

        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Senha alterada com sucesso: {UserId}", userId);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Desativando usuário: {UserId}", id);
        await _userRepository.SoftDeleteAsync(id, cancellationToken);
        _logger.LogInformation("Usuário desativado: {UserId}", id);
    }
}