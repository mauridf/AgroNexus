namespace AgroNexus.Application.DTOs.Responses;

/// <summary>
/// Resposta com dados do usuário (sem informações sensíveis).
/// </summary>
public sealed record UserResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string Role { get; init; } = null!;
    public DateTime? LastLogin { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Resposta do login com tokens JWT.
/// </summary>
public sealed record LoginResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public DateTime ExpiresAt { get; init; }
    public UserResponse User { get; init; } = null!;
}

/// <summary>
/// Resposta genérica de erro.
/// ATENÇÃO: É uma classe (não record) porque as propriedades precisam ser 
/// modificadas no GlobalExceptionMiddleware.
/// </summary>
public sealed class ErrorResponse
{
    public string Message { get; set; } = null!;
    public string? ErrorCode { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
}