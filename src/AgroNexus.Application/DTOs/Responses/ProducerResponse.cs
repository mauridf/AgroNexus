namespace AgroNexus.Application.DTOs.Responses;

public sealed record ProducerResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
    public string CpfCnpj { get; init; } = null!;
    public string? Rg { get; init; }
    public string? InscricaoEstadual { get; init; }
    public DateTime? DataNascimento { get; init; }
    public string? Telefone { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record FarmResponse
{
    public Guid Id { get; init; }
    public Guid ProducerId { get; init; }
    public string Name { get; init; } = null!;
    public decimal TotalAreaHa { get; init; }
    public decimal AgriculturalAreaHa { get; init; }
    public decimal VegetationAreaHa { get; init; }
    public decimal BuiltAreaHa { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public DateTime CreatedAt { get; init; }
}