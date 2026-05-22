using AgroNexus.Domain.Enums;
using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.ValueObjects;

namespace AgroNexus.Domain.Entities.Identity;

/// <summary>
/// Representa um usuário do sistema (Administrador ou Produtor).
/// Entidade raiz do agregado Identity.
/// </summary>
public sealed class User : BaseEntity
{
    /// <summary>
    /// Email do usuário (usado como login).
    /// </summary>
    public Email Email { get; private set; } = null!;

    /// <summary>
    /// Hash da senha (nunca armazenamos senha em texto puro).
    /// </summary>
    public string PasswordHash { get; private set; } = null!;

    /// <summary>
    /// Papel do usuário: ADM (Administrador) ou PRD (Produtor).
    /// </summary>
    public UserRole Role { get; private set; }

    /// <summary>
    /// Data do último login do usuário.
    /// </summary>
    public DateTime? LastLogin { get; private set; }

    /// <summary>
    /// Construtor privado para o EF Core.
    /// </summary>
    private User() { }

    /// <summary>
    /// Fábrica para criar um novo usuário.
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="passwordHash">Hash da senha já processado</param>
    /// <param name="role">Papel do usuário</param>
    /// <returns>Nova instância de User</returns>
    public static User Create(string email, string passwordHash, UserRole role)
    {
        // Validações de domínio
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Senha é obrigatória.", "USER_PASSWORD_REQUIRED");

        if (role != UserRole.ADM && role != UserRole.PRD)
            throw new DomainException("Papel de usuário inválido.", "USER_ROLE_INVALID");

        var user = new User
        {
            Email = Email.Create(email),
            PasswordHash = passwordHash,
            Role = role
        };

        return user;
    }

    /// <summary>
    /// Atualiza o hash da senha do usuário.
    /// </summary>
    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("Nova senha é obrigatória.", "USER_PASSWORD_REQUIRED");

        PasswordHash = newPasswordHash;
        MarkAsUpdated();
    }

    /// <summary>
    /// Registra o login do usuário, atualizando a data do último acesso.
    /// </summary>
    public void RegisterLogin()
    {
        LastLogin = DateTime.UtcNow;
        MarkAsUpdated();
    }

    /// <summary>
    /// Altera o papel do usuário (apenas ADM pode fazer isso).
    /// </summary>
    public void ChangeRole(UserRole newRole)
    {
        if (newRole != UserRole.ADM && newRole != UserRole.PRD)
            throw new DomainException("Papel de usuário inválido.", "USER_ROLE_INVALID");

        Role = newRole;
        MarkAsUpdated();
    }

    /// <summary>
    /// Verifica se o usuário é administrador.
    /// </summary>
    public bool IsAdmin() => Role == UserRole.ADM;

    /// <summary>
    /// Verifica se o usuário é produtor.
    /// </summary>
    public bool IsProducer() => Role == UserRole.PRD;
}