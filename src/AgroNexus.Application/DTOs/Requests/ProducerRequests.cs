namespace AgroNexus.Application.DTOs.Requests;

public sealed record CreateProducerRequest
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
    public string CpfCnpj { get; init; } = null!;
    public string? Rg { get; init; }
    public string? InscricaoEstadual { get; init; }
    public DateTime? DataNascimento { get; init; }
    public string? Telefone { get; init; }
    public string? Endereco { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public string? DadosBancarios { get; init; }
    public string? Car { get; init; }
}

public sealed record UpdateProducerRequest
{
    public string Name { get; init; } = null!;
    public string? Rg { get; init; }
    public string? InscricaoEstadual { get; init; }
    public DateTime? DataNascimento { get; init; }
    public string? Telefone { get; init; }
    public string? Endereco { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public string? DadosBancarios { get; init; }
    public string? Car { get; init; }
}