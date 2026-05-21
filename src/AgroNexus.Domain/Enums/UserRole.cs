namespace AgroNexus.Domain.Enums;

/// <summary>
/// Papéis de usuário no sistema.
/// ADM = Administrador (acesso total)
/// PRD = Produtor (acesso restrito às próprias informações)
/// </summary>
public enum UserRole
{
    ADM = 1,
    PRD = 2
}