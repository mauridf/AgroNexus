namespace AgroNexus.Application.DTOs.Requests;

public sealed record CreateEmployeeRequest
{
    public Guid FarmId { get; init; }
    public string Name { get; init; } = null!;
    public string? Cpf { get; init; }
    public string? Funcao { get; init; }
    public decimal? Salario { get; init; }
    public DateTime? DataAdmissao { get; init; }
}

public sealed record UpdateEmployeeRequest
{
    public string Name { get; init; } = null!;
    public string? Cpf { get; init; }
    public string? Funcao { get; init; }
    public decimal? Salario { get; init; }
}

public sealed record DismissEmployeeRequest
{
    public DateTime DataDemissao { get; init; }
}