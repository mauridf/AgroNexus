namespace AgroNexus.Application.DTOs.Requests;

public sealed record CreateProductionSaleRequest
{
    public Guid PlantedCultureId { get; init; }
    public decimal QuantidadeVendida { get; init; }
    public decimal PrecoUnitario { get; init; }
    public DateTime DataVenda { get; init; }
    public string? Destino { get; init; }
}