using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Agriculture;

/// <summary>
/// Representa uma cultura agrícola cadastrada no sistema (catálogo geral).
/// Exemplos: Arroz, Feijão, Algodão, Soja, Milho.
/// </summary>
public sealed class Culture : BaseEntity
{
    /// <summary>
    /// Nome da cultura.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Ciclo da cultura: Anual, Perene, Semi-perene.
    /// </summary>
    public string? Ciclo { get; private set; }

    /// <summary>
    /// Variedade específica da cultura.
    /// </summary>
    public string? Variedade { get; private set; }

    private Culture() { }

    public static Culture Create(string name, string? ciclo = null, string? variedade = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da cultura é obrigatório.", "CULTURE_NAME_REQUIRED");

        var culture = new Culture
        {
            Name = name.Trim(),
            Ciclo = ciclo?.Trim(),
            Variedade = variedade?.Trim()
        };

        return culture;
    }

    public void Update(string name, string? ciclo, string? variedade)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da cultura é obrigatório.", "CULTURE_NAME_REQUIRED");

        Name = name.Trim();
        Ciclo = ciclo?.Trim();
        Variedade = variedade?.Trim();

        MarkAsUpdated();
    }
}