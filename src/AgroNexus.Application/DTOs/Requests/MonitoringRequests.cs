namespace AgroNexus.Application.DTOs.Requests;

public sealed record CreateAlertRequest
{
    public Guid FarmId { get; init; }
    public string Tipo { get; init; } = null!;
    public string Nivel { get; init; } = null!;
    public DateTime Data { get; init; }
    public string? Descricao { get; init; }
}

public sealed record CreateCertificateRequest
{
    public Guid FarmId { get; init; }
    public string Tipo { get; init; } = null!;
    public DateTime DataEmissao { get; init; }
    public DateTime DataValidade { get; init; }
}

public sealed record CreateClimateRequest
{
    public Guid FarmId { get; init; }
    public DateTime Data { get; init; }
    public decimal? Temperatura { get; init; }
    public decimal? ChuvaMm { get; init; }
    public decimal? Umidade { get; init; }
}