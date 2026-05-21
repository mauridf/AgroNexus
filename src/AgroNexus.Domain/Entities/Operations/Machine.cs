using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Operations;

/// <summary>
/// Representa uma máquina agrícola pertencente a uma fazenda.
/// </summary>
public sealed class Machine : BaseEntity
{
    /// <summary>
    /// FK para a fazenda.
    /// </summary>
    public Guid FarmId { get; private set; }

    /// <summary>
    /// Descrição da máquina.
    /// </summary>
    public string Descricao { get; private set; } = null!;

    /// <summary>
    /// Marca da máquina.
    /// </summary>
    public string? Marca { get; private set; }

    /// <summary>
    /// Modelo da máquina.
    /// </summary>
    public string? Modelo { get; private set; }

    /// <summary>
    /// Ano de fabricação.
    /// </summary>
    public int? Ano { get; private set; }

    /// <summary>
    /// Valor aproximado da máquina.
    /// </summary>
    public decimal? ValorAproximado { get; private set; }

    private Machine() { }

    public static Machine Create(
        Guid farmId,
        string descricao,
        string? marca = null,
        string? modelo = null,
        int? ano = null,
        decimal? valorAproximado = null)
    {
        if (farmId == Guid.Empty)
            throw new DomainException("Fazenda é obrigatória.", "MACHINE_FARM_REQUIRED");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("Descrição é obrigatória.", "MACHINE_DESC_REQUIRED");

        if (ano.HasValue && (ano.Value < 1900 || ano.Value > DateTime.UtcNow.Year + 1))
            throw new DomainException("Ano de fabricação inválido.", "MACHINE_YEAR_INVALID");

        var machine = new Machine
        {
            FarmId = farmId,
            Descricao = descricao.Trim(),
            Marca = marca?.Trim(),
            Modelo = modelo?.Trim(),
            Ano = ano,
            ValorAproximado = valorAproximado
        };

        return machine;
    }
}