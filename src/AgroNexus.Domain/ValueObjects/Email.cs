using AgroNexus.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace AgroNexus.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um endereço de email validado.
/// </summary>
public sealed partial record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Cria um email validado.
    /// </summary>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email é obrigatório.", "EMAIL_REQUIRED");

        var emailLimpo = email.Trim().ToLowerInvariant();

        if (!EmailRegex().IsMatch(emailLimpo))
            throw new DomainException("Formato de email inválido.", "EMAIL_INVALID_FORMAT");

        if (emailLimpo.Length > 255)
            throw new DomainException("Email excede 255 caracteres.", "EMAIL_TOO_LONG");

        return new Email(emailLimpo);
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();
}