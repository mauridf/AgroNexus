using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.ValueObjects;

/// <summary>
/// Value Object que representa uma porcentagem (0% a 100%).
/// </summary>
public sealed record Percentage
{
    public decimal Value { get; }

    private Percentage(decimal value)
    {
        Value = Math.Round(value, 2);
    }

    public static Percentage Create(decimal value)
    {
        if (value < 0 || value > 100)
            throw new DomainException("Porcentagem deve estar entre 0 e 100.", "PERCENTAGE_INVALID");

        return new Percentage(value);
    }

    public override string ToString() => $"{Value}%";

    public static implicit operator decimal(Percentage p) => p.Value;
}