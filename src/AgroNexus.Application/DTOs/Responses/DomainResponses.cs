namespace AgroNexus.Application.DTOs.Responses;

// Agriculture
public sealed record CultureResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Ciclo { get; init; }
    public string? Variedade { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record PlantedCultureResponse
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public string FarmName { get; init; } = null!;
    public Guid CultureId { get; init; }
    public string CultureName { get; init; } = null!;
    public string Safra { get; init; } = null!;
    public decimal AreaPlantadaHa { get; init; }
    public DateTime? DataPlantio { get; init; }
    public DateTime? DataColheitaPrevista { get; init; }
    public DateTime? DataColheitaReal { get; init; }
    public decimal? ProdutividadeEsperadaSacasHa { get; init; }
    public decimal? ProdutividadeObtidaSacasHa { get; init; }
    public decimal? CustoTotal { get; init; }
    public decimal? ReceitaTotal { get; init; }
    public decimal? Lucro => ReceitaTotal.HasValue && CustoTotal.HasValue ? ReceitaTotal - CustoTotal : null;
    public DateTime CreatedAt { get; init; }
}

// Farm
public sealed record EmployeeResponse
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public string Name { get; init; } = null!;
    public string? Cpf { get; init; }
    public string? Funcao { get; init; }
    public decimal? Salario { get; init; }
    public DateTime? DataAdmissao { get; init; }
    public DateTime? DataDemissao { get; init; }
    public bool Empregado => !DataDemissao.HasValue;
    public DateTime CreatedAt { get; init; }
}

// Inventory
public sealed record InputResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Tipo { get; init; }
    public string? UnidadeMedida { get; init; }
    public string? Fornecedor { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record InputStockResponse
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public Guid InputId { get; init; }
    public string InputName { get; init; } = null!;
    public decimal Quantidade { get; init; }
    public DateTime? Validade { get; init; }
    public bool Vencido => Validade.HasValue && Validade.Value < DateTime.UtcNow;
    public DateTime CreatedAt { get; init; }
}

// Operations
public sealed record ContractResponse
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public string Tipo { get; init; } = null!;
    public string? ParteContratante { get; init; }
    public decimal Valor { get; init; }
    public DateTime DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public bool Ativo => DateTime.UtcNow >= DataInicio && (!DataFim.HasValue || DateTime.UtcNow <= DataFim.Value);
    public DateTime CreatedAt { get; init; }
}

public sealed record OperationalCostResponse
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public string Descricao { get; init; } = null!;
    public decimal Valor { get; init; }
    public DateTime Data { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record MachineResponse
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public string Descricao { get; init; } = null!;
    public string? Marca { get; init; }
    public string? Modelo { get; init; }
    public int? Ano { get; init; }
    public decimal? ValorAproximado { get; init; }
    public DateTime CreatedAt { get; init; }
}

// Monitoring
public sealed record AlertResponse
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public string Tipo { get; init; } = null!;
    public string? Descricao { get; init; }
    public string Nivel { get; init; } = null!;
    public DateTime Data { get; init; }
    public bool Resolvido { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record CertificateResponse
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public string Tipo { get; init; } = null!;
    public DateTime DataEmissao { get; init; }
    public DateTime DataValidade { get; init; }
    public bool Valido => DateTime.UtcNow >= DataEmissao && DateTime.UtcNow <= DataValidade;
    public DateTime CreatedAt { get; init; }
}

public sealed record ClimateResponse
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public DateTime Data { get; init; }
    public decimal? Temperatura { get; init; }
    public decimal? ChuvaMm { get; init; }
    public decimal? Umidade { get; init; }
    public DateTime CreatedAt { get; init; }
}

// Financial
public sealed record ProductionSaleResponse
{
    public Guid Id { get; init; }
    public Guid PlantedCultureId { get; init; }
    public decimal QuantidadeVendida { get; init; }
    public decimal PrecoUnitario { get; init; }
    public decimal ValorTotal => QuantidadeVendida * PrecoUnitario;
    public DateTime DataVenda { get; init; }
    public string? Destino { get; init; }
    public DateTime CreatedAt { get; init; }
}