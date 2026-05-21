using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Inventory;

/// <summary>
/// Representa uma compra de insumo realizada por uma fazenda.
/// </summary>
public sealed class InputPurchase : BaseEntity
{
    /// <summary>
    /// FK para a fazenda que comprou o insumo.
    /// </summary>
    public Guid FarmId { get; private set; }

    /// <summary>
    /// FK para o insumo comprado.
    /// </summary>
    public Guid InputId { get; private set; }

    /// <summary>
    /// Quantidade comprada.
    /// </summary>
    public decimal Quantidade { get; private set; }

    /// <summary>
    /// Valor total da compra.
    /// </summary>
    public decimal ValorTotal { get; private set; }

    /// <summary>
    /// Data da compra.
    /// </summary>
    public DateTime DataCompra { get; private set; }

    /// <summary>
    /// Fornecedor da compra (pode ser diferente do fornecedor padrão do insumo).
    /// </summary>
    public string? Fornecedor { get; private set; }

    private InputPurchase() { }

    public static InputPurchase Create(
        Guid farmId,
        Guid inputId,
        decimal quantidade,
        decimal valorTotal,
        DateTime dataCompra,
        string? fornecedor = null)
    {
        if (farmId == Guid.Empty)
            throw new DomainException("Fazenda é obrigatória.", "INPUT_PURCHASE_FARM_REQUIRED");

        if (inputId == Guid.Empty)
            throw new DomainException("Insumo é obrigatório.", "INPUT_PURCHASE_INPUT_REQUIRED");

        if (quantidade <= 0)
            throw new DomainException("Quantidade deve ser maior que zero.", "INPUT_PURCHASE_QTY_INVALID");

        if (valorTotal <= 0)
            throw new DomainException("Valor total deve ser maior que zero.", "INPUT_PURCHASE_VALUE_INVALID");

        if (dataCompra > DateTime.UtcNow)
            throw new DomainException("Data da compra não pode ser futura.", "INPUT_PURCHASE_DATE_INVALID");

        var purchase = new InputPurchase
        {
            FarmId = farmId,
            InputId = inputId,
            Quantidade = quantidade,
            ValorTotal = valorTotal,
            DataCompra = dataCompra,
            Fornecedor = fornecedor?.Trim()
        };

        return purchase;
    }
}