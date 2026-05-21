using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;
using AgroNexus.Domain.Entities.Identity;
using AgroNexus.Domain.Enums;
using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.Interfaces.Repositories;
using BCrypt.Net;
using Mapster;
using Microsoft.Extensions.Logging;

namespace AgroNexus.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
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

        // Hash da senha (usando BCrypt)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

        // Cria a entidade de domínio
        var user = User.Create(
            email: request.Email,
            passwordHash: passwordHash,
            role: role);

        await _userRepository.AddAsync(user, cancellationToken);

        _logger.LogInformation("Usuário criado: {UserId}", user.Id);

        return user.Adapt<UserResponse>();
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tentativa de login: {Email}", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new DomainException("Email ou senha inválidos.", "LOGIN_INVALID");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new DomainException("Email ou senha inválidos.", "LOGIN_INVALID");

        user.RegisterLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);

        // TODO: Gerar tokens JWT (será implementado no próximo passo)
        _logger.LogInformation("Login bem-sucedido: {UserId}", user.Id);

        return new LoginResponse
        {
            AccessToken = "PLACEHOLDER_JWT_TOKEN",
            RefreshToken = "PLACEHOLDER_REFRESH_TOKEN",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = user.Adapt<UserResponse>()
        };
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Usuário", id);

        return user.Adapt<UserResponse>();
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Adapt<IEnumerable<UserResponse>>();
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Alterando senha do usuário: {UserId}", userId);

        if (request.NewPassword != request.ConfirmNewPassword)
            throw new DomainException("Nova senha e confirmação não conferem.", "USER_PASSWORD_MISMATCH");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Usuário", userId);

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new DomainException("Senha atual incorreta.", "USER_PASSWORD_WRONG");

        var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
        user.UpdatePassword(newHash);

        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Senha alterada: {UserId}", userId);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Desativando usuário: {UserId}", id);
        await _userRepository.SoftDeleteAsync(id, cancellationToken);
    }
}