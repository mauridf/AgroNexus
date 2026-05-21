namespace AgroNexus.Application.DTOs.Requests;

public sealed record CreateContractRequest
{
    public Guid FarmId { get; init; }
    public string Tipo { get; init; } = null!;
    public string? ParteContratante { get; init; }
    public decimal Valor { get; init; }
    public DateTime DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
}

public sealed record CreateOperationalCostRequest
{
    public Guid FarmId { get; init; }
    public string Descricao { get; init; } = null!;
    public decimal Valor { get; init; }
    public DateTime Data { get; init; }
}

public sealed record CreateMachineRequest
{
    public Guid FarmId { get; init; }
    public string Descricao { get; init; } = null!;
    public string? Marca { get; init; }
    public string? Modelo { get; init; }
    public int? Ano { get; init; }
    public decimal? ValorAproximado { get; init; }
}