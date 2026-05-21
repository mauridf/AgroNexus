namespace AgroNexus.Application.DTOs.Requests;

public sealed record CreateInputRequest
{
    public string Name { get; init; } = null!;
    public string? Tipo { get; init; }
    public string? UnidadeMedida { get; init; }
    public string? Fornecedor { get; init; }
}

public sealed record CreateInputPurchaseRequest
{
    public Guid FarmId { get; init; }
    public Guid InputId { get; init; }
    public decimal Quantidade { get; init; }
    public decimal ValorTotal { get; init; }
    public DateTime DataCompra { get; init; }
    public string? Fornecedor { get; init; }
}

public sealed record UpdateInputStockRequest
{
    public decimal Quantidade { get; init; }
    public DateTime? Validade { get; init; }
}