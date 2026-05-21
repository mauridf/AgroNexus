namespace AgroNexus.Application.DTOs.Requests;

public sealed record CreateFarmRequest
{
    public Guid ProducerId { get; init; }
    public string Name { get; init; } = null!;
    public decimal TotalAreaHa { get; init; }
    public decimal AgriculturalAreaHa { get; init; }
    public decimal VegetationAreaHa { get; init; }
    public decimal BuiltAreaHa { get; init; }
    public string? Endereco { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public string? InscricaoEstadual { get; init; }
    public string? CodigoCar { get; init; }
    public string? Ccir { get; init; }
    public string? FonteAgua { get; init; }
}

public sealed record UpdateFarmRequest
{
    public string Name { get; init; } = null!;
    public decimal TotalAreaHa { get; init; }
    public decimal AgriculturalAreaHa { get; init; }
    public decimal VegetationAreaHa { get; init; }
    public decimal BuiltAreaHa { get; init; }
    public string? Endereco { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public string? InscricaoEstadual { get; init; }
    public string? CodigoCar { get; init; }
    public string? Ccir { get; init; }
    public string? FonteAgua { get; init; }
}