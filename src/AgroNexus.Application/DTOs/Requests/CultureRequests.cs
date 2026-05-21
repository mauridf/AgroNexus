namespace AgroNexus.Application.DTOs.Requests;

public sealed record CreateCultureRequest
{
    public string Name { get; init; } = null!;
    public string? Ciclo { get; init; }
    public string? Variedade { get; init; }
}

public sealed record CreatePlantedCultureRequest
{
    public Guid FarmId { get; init; }
    public Guid CultureId { get; init; }
    public string Safra { get; init; } = null!;
    public decimal AreaPlantadaHa { get; init; }
    public DateTime? DataPlantio { get; init; }
    public DateTime? DataColheitaPrevista { get; init; }
    public decimal? ProdutividadeEsperadaSacasHa { get; init; }
    public decimal? CustoTotal { get; init; }
}

public sealed record UpdatePlantedCultureRequest
{
    public string Safra { get; init; } = null!;
    public decimal AreaPlantadaHa { get; init; }
    public DateTime? DataPlantio { get; init; }
    public DateTime? DataColheitaPrevista { get; init; }
    public decimal? ProdutividadeEsperadaSacasHa { get; init; }
    public decimal? CustoTotal { get; init; }
}

public sealed record HarvestRequest
{
    public DateTime DataColheitaReal { get; init; }
    public decimal ProdutividadeObtidaSacasHa { get; init; }
    public decimal ReceitaTotal { get; init; }
}