using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Operations;

/// <summary>
/// Representa um custo operacional registrado para uma fazenda.
/// </summary>
public sealed class OperationalCost : BaseEntity
{
    /// <summary>
    /// FK para a fazenda.
    /// </summary>
    public Guid FarmId { get; private set; }

    /// <summary>
    /// Descrição do custo.
    /// </summary>
    public string Descricao { get; private set; } = null!;

    /// <summary>
    /// Valor do custo.
    /// </summary>
    public decimal Valor { get; private set; }

    /// <summary>
    /// Data em que o custo ocorreu.
    /// </summary>
    public DateTime Data { get; private set; }

    private OperationalCost() { }

    public static OperationalCost Create(Guid farmId, string descricao, decimal valor, DateTime data)
    {
        if (farmId == Guid.Empty)
            throw new DomainException("Fazenda é obrigatória.", "OP_COST_FARM_REQUIRED");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("Descrição é obrigatória.", "OP_COST_DESC_REQUIRED");

        if (valor <= 0)
            throw new DomainException("Valor deve ser maior que zero.", "OP_COST_VALUE_INVALID");

        if (data > DateTime.UtcNow)
            throw new DomainException("Data não pode ser futura.", "OP_COST_DATE_INVALID");

        var cost = new OperationalCost
        {
            FarmId = farmId,
            Descricao = descricao.Trim(),
            Valor = valor,
            Data = data
        };

        return cost;
    }
}