namespace AgroNexus.Application.DTOs.Responses;

// ============================================
// DASHBOARD PRINCIPAL
// ============================================

/// <summary>
/// Resumo geral do dashboard para um produtor.
/// </summary>
public sealed record ProducerDashboardResponse
{
    /// <summary>
    /// Total de fazendas do produtor.
    /// </summary>
    public int TotalFarms { get; init; }

    /// <summary>
    /// Área total somada de todas as fazendas (ha).
    /// </summary>
    public decimal TotalAreaHa { get; init; }

    /// <summary>
    /// Área agricultável total (ha).
    /// </summary>
    public decimal TotalAgriculturalAreaHa { get; init; }

    /// <summary>
    /// Área plantada atual (ha).
    /// </summary>
    public decimal TotalPlantedAreaHa { get; init; }

    /// <summary>
    /// Porcentagem da área agricultável utilizada.
    /// </summary>
    public decimal AreaUtilizationPercentage { get; init; }

    /// <summary>
    /// Total de funcionários ativos.
    /// </summary>
    public int TotalActiveEmployees { get; init; }

    /// <summary>
    /// Total de máquinas agrícolas.
    /// </summary>
    public int TotalMachines { get; init; }

    /// <summary>
    /// Alertas não resolvidos.
    /// </summary>
    public int UnresolvedAlerts { get; init; }

    /// <summary>
    /// Certificados próximos do vencimento (30 dias).
    /// </summary>
    public int ExpiringCertificates { get; init; }
}

// ============================================
// DASHBOARD FINANCEIRO
// ============================================

/// <summary>
/// Resumo financeiro do produtor.
/// </summary>
public sealed record FinancialDashboardResponse
{
    /// <summary>
    /// Receita total com vendas no ano atual.
    /// </summary>
    public decimal RevenueCurrentYear { get; init; }

    /// <summary>
    /// Custo operacional total no ano atual.
    /// </summary>
    public decimal OperationalCostCurrentYear { get; init; }

    /// <summary>
    /// Custo total com insumos no ano atual.
    /// </summary>
    public decimal InputCostCurrentYear { get; init; }

    /// <summary>
    /// Lucro estimado (Receita - Custos).
    /// </summary>
    public decimal EstimatedProfit { get; init; }

    /// <summary>
    /// Margem de lucro (%).
    /// </summary>
    public decimal ProfitMarginPercentage { get; init; }

    /// <summary>
    /// Receita dos últimos 12 meses (para gráfico).
    /// </summary>
    public List<MonthlyRevenue> MonthlyRevenues { get; init; } = new();

    /// <summary>
    /// Top 5 culturas mais lucrativas.
    /// </summary>
    public List<CultureProfitability> TopProfitableCultures { get; init; } = new();
}

public sealed record MonthlyRevenue
{
    public int Year { get; init; }
    public int Month { get; init; }
    public decimal Revenue { get; init; }
    public decimal Cost { get; init; }
}

public sealed record CultureProfitability
{
    public string CultureName { get; init; } = null!;
    public string Safra { get; init; } = null!;
    public decimal Revenue { get; init; }
    public decimal Cost { get; init; }
    public decimal Profit { get; init; }
    public decimal ProfitMarginPercentage { get; init; }
}

// ============================================
// DASHBOARD DE FAZENDA ESPECÍFICA
// ============================================

/// <summary>
/// Dashboard detalhado de uma fazenda específica.
/// </summary>
public sealed record FarmDashboardResponse
{
    public Guid FarmId { get; init; }
    public string FarmName { get; init; } = null!;

    // Áreas
    public decimal TotalAreaHa { get; init; }
    public decimal AgriculturalAreaHa { get; init; }
    public decimal VegetationAreaHa { get; init; }
    public decimal BuiltAreaHa { get; init; }
    public decimal PlantedAreaHa { get; init; }
    public decimal AreaUtilizationPercentage { get; init; }

    // Safra Atual
    public string? CurrentSafra { get; init; }
    public int ActiveCulturesCount { get; init; }
    public List<PlantedCultureSummary> ActiveCultures { get; init; } = new();

    // Financeiro
    public decimal RevenueCurrentSafra { get; init; }
    public decimal CostCurrentSafra { get; init; }
    public decimal ProfitCurrentSafra { get; init; }

    // Estoque
    public int TotalInputsInStock { get; init; }
    public int ExpiredInputsCount { get; init; }

    // Pessoas
    public int ActiveEmployees { get; init; }

    // Alertas
    public int UnresolvedAlerts { get; init; }
    public List<AlertSummary> RecentAlerts { get; init; } = new();

    // Contratos
    public int ActiveContracts { get; init; }

    // Certificados
    public int ValidCertificates { get; init; }
    public int ExpiringCertificates { get; init; }

    // Clima recente
    public ClimateSummary? LastClimateRecord { get; init; }
}

public sealed record PlantedCultureSummary
{
    public string CultureName { get; init; } = null!;
    public decimal AreaHa { get; init; }
    public DateTime? PlantingDate { get; init; }
    public DateTime? ExpectedHarvestDate { get; init; }
    public int DaysUntilHarvest { get; init; }
}

public sealed record AlertSummary
{
    public string Tipo { get; init; } = null!;
    public string Nivel { get; init; } = null!;
    public string? Descricao { get; init; }
    public DateTime Data { get; init; }
}

public sealed record ClimateSummary
{
    public DateTime Data { get; init; }
    public decimal? Temperatura { get; init; }
    public decimal? ChuvaMm { get; init; }
    public decimal? Umidade { get; init; }
}

// ============================================
// DASHBOARD ADMIN
// ============================================

/// <summary>
/// Dashboard do administrador com visão geral do sistema.
/// </summary>
public sealed record AdminDashboardResponse
{
    public int TotalUsers { get; init; }
    public int TotalProducers { get; init; }
    public int TotalFarms { get; init; }
    public decimal TotalAreaManagedHa { get; init; }
    public int TotalActiveEmployees { get; init; }
    public int TotalUnresolvedAlerts { get; init; }
    public List<TopProducer> TopProducers { get; init; } = new();
}

public sealed record TopProducer
{
    public string ProducerName { get; init; } = null!;
    public int FarmCount { get; init; }
    public decimal TotalAreaHa { get; init; }
}