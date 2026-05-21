using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Inventory;

/// <summary>
/// Representa um insumo agrícola no catálogo geral.
/// Exemplos: Fertilizante NPK, Semente de Soja, Defensivo Agrícola.
/// </summary>
public sealed class Input : BaseEntity
{
    /// <summary>
    /// Nome do insumo.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Tipo do insumo: fertilizante, semente, defensivo, combustível, etc.
    /// </summary>
    public string? Tipo { get; private set; }

    /// <summary>
    /// Unidade de medida: kg, L, ton, saca, unidade, etc.
    /// </summary>
    public string? UnidadeMedida { get; private set; }

    /// <summary>
    /// Fornecedor principal do insumo.
    /// </summary>
    public string? Fornecedor { get; private set; }

    private Input() { }

    public static Input Create(string name, string? tipo = null, string? unidadeMedida = null, string? fornecedor = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do insumo é obrigatório.", "INPUT_NAME_REQUIRED");

        var input = new Input
        {
            Name = name.Trim(),
            Tipo = tipo?.Trim(),
            UnidadeMedida = unidadeMedida?.Trim(),
            Fornecedor = fornecedor?.Trim()
        };

        return input;
    }

    public void Update(string name, string? tipo, string? unidadeMedida, string? fornecedor)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do insumo é obrigatório.", "INPUT_NAME_REQUIRED");

        Name = name.Trim();
        Tipo = tipo?.Trim();
        UnidadeMedida = unidadeMedida?.Trim();
        Fornecedor = fornecedor?.Trim();

        MarkAsUpdated();
    }
}