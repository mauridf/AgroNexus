using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Financial;

/// <summary>
/// Representa uma venda de produção associada a uma cultura plantada.
/// </summary>
public sealed class ProductionSale : BaseEntity
{
    /// <summary>
    /// FK para a cultura plantada que originou a produção vendida.
    /// </summary>
    public Guid PlantedCultureId { get; private set; }

    /// <summary>
    /// Quantidade vendida (em sacas, toneladas, etc.).
    /// </summary>
    public decimal QuantidadeVendida { get; private set; }

    /// <summary>
    /// Preço unitário de venda.
    /// </summary>
    public decimal PrecoUnitario { get; private set; }

    /// <summary>
    /// Data da venda.
    /// </summary>
    public DateTime DataVenda { get; private set; }

    /// <summary>
    /// Destino da venda (comprador, cooperativa, exportação, etc.).
    /// </summary>
    public string? Destino { get; private set; }

    /// <summary>
    /// Valor total da venda (calculado: QuantidadeVendida * PrecoUnitario).
    /// </summary>
    public decimal ValorTotal => QuantidadeVendida * PrecoUnitario;

    private ProductionSale() { }

    public static ProductionSale Create(
        Guid plantedCultureId,
        decimal quantidadeVendida,
        decimal precoUnitario,
        DateTime dataVenda,
        string? destino = null)
    {
        if (plantedCultureId == Guid.Empty)
            throw new DomainException("Cultura plantada é obrigatória.", "SALE_PLANTED_CULTURE_REQUIRED");

        if (quantidadeVendida <= 0)
            throw new DomainException("Quantidade vendida deve ser maior que zero.", "SALE_QTY_INVALID");

        if (precoUnitario <= 0)
            throw new DomainException("Preço unitário deve ser maior que zero.", "SALE_PRICE_INVALID");

        if (dataVenda > DateTime.UtcNow)
            throw new DomainException("Data da venda não pode ser futura.", "SALE_DATE_INVALID");

        var sale = new ProductionSale
        {
            PlantedCultureId = plantedCultureId,
            QuantidadeVendida = quantidadeVendida,
            PrecoUnitario = precoUnitario,
            DataVenda = dataVenda,
            Destino = destino?.Trim()
        };

        return sale;
    }
}