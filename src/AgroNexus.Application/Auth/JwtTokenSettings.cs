namespace AgroNexus.Application.Auth;

/// <summary>
/// Configurações do JWT carregadas das variáveis de ambiente/secret manager.
/// </summary>
public sealed class JwtTokenSettings
{
    /// <summary>
    /// Chave secreta para assinatura do token (mínimo 512 bits = 64 caracteres).
    /// </summary>
    public string SecretKey { get; set; } = null!;

    /// <summary>
    /// Emissor do token (quem gerou).
    /// </summary>
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// Audiência do token (para quem é destinado).
    /// </summary>
    public string Audience { get; set; } = null!;

    /// <summary>
    /// Tempo de expiração do access token em minutos.
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 60;

    /// <summary>
    /// Tempo de expiração do refresh token em dias.
    /// </summary>
    public int RefreshTokenExpirationInDays { get; set; } = 7;
}