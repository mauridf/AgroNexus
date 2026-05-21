namespace AgroNexus.Application.DTOs.Requests;

/// <summary>
/// DTO para criação de um novo usuário.
/// </summary>
public sealed record CreateUserRequest
{
    /// <summary>
    /// Email do usuário (será usado como login).
    /// </summary>
    public string Email { get; init; } = null!;

    /// <summary>
    /// Senha em texto puro (será hasheada antes de armazenar).
    /// </summary>
    public string Password { get; init; } = null!;

    /// <summary>
    /// Confirmação da senha (deve ser igual à senha).
    /// </summary>
    public string ConfirmPassword { get; init; } = null!;

    /// <summary>
    /// Papel do usuário: "ADM" ou "PRD".
    /// </summary>
    public string Role { get; init; } = null!;
}

/// <summary>
/// DTO para login de usuário.
/// </summary>
public sealed record LoginRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}

/// <summary>
/// DTO para atualização de senha.
/// </summary>
public sealed record ChangePasswordRequest
{
    public string CurrentPassword { get; init; } = null!;
    public string NewPassword { get; init; } = null!;
    public string ConfirmNewPassword { get; init; } = null!;
}