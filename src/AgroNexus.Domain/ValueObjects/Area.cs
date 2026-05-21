using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.ValueObjects;

/// <summary>
/// Value Object que representa uma área em hectares.
/// Garante valores positivos e precisão de 4 casas decimais.
/// </summary>
public sealed record Area
{
    /// <summary>
    /// Valor da área em hectares.
    /// </summary>
    public decimal Hectares { get; }

    /// <summary>
    /// Valor da área em metros quadrados.
    /// </summary>
    public decimal MetrosQuadrados => Hectares * 10000m;

    private Area(decimal hectares)
    {
        Hectares = Math.Round(hectares, 4);
    }

    /// <summary>
    /// Cria uma área a partir de hectares.
    /// </summary>
    public static Area FromHectares(decimal hectares)
    {
        if (hectares < 0)
            throw new DomainException("Área não pode ser negativa.", "AREA_NEGATIVE");

        if (hectares > 1_000_000)
            throw new DomainException("Área excede o limite máximo de 1 milhão de hectares.", "AREA_MAX_EXCEEDED");

        return new Area(hectares);
    }

    /// <summary>
    /// Soma de áreas.
    /// </summary>
    public Area Add(Area other) => new(Hectares + other.Hectares);

    /// <summary>
    /// Verifica se esta área é maior que outra.
    /// </summary>
    public bool IsGreaterThan(Area other) => Hectares > other.Hectares;

    /// <summary>
    /// Verifica se a soma de áreas excede o total permitido.
    /// </summary>
    public static void ValidateSum(Area total, params Area[] parts)
    {
        var sum = parts.Aggregate(0m, (acc, area) => acc + area.Hectares);
        if (sum > total.Hectares)
            throw new DomainException(
                $"A soma das áreas ({sum:F2}ha) excede a área total ({total.Hectares:F2}ha).",
                "AREA_SUM_EXCEEDS_TOTAL");
    }

    public override string ToString() => $"{Hectares:F2} ha";

    public static implicit operator decimal(Area area) => area.Hectares;
    public static Area operator +(Area a, Area b) => new(a.Hectares + b.Hectares);
    public static bool operator >(Area a, Area b) => a.Hectares > b.Hectares;
    public static bool operator <(Area a, Area b) => a.Hectares < b.Hectares;
}