using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;
using AgroNexus.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace AgroNexus.Application.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IFarmRepository _farmRepo;
    private readonly IProducerRepository _producerRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IMachineRepository _machineRepo;
    private readonly IAlertRepository _alertRepo;
    private readonly ICertificateRepository _certRepo;
    private readonly IClimateRepository _climateRepo;
    private readonly IPlantedCultureRepository _plantedCultureRepo;
    private readonly IProductionSaleRepository _saleRepo;
    private readonly IOperationalCostRepository _costRepo;
    private readonly IInputPurchaseRepository _purchaseRepo;
    private readonly IInputStockRepository _stockRepo;
    private readonly IContractRepository _contractRepo;
    private readonly IUserRepository _userRepo;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        IFarmRepository farmRepo,
        IProducerRepository producerRepo,
        IEmployeeRepository employeeRepo,
        IMachineRepository machineRepo,
        IAlertRepository alertRepo,
        ICertificateRepository certRepo,
        IClimateRepository climateRepo,
        IPlantedCultureRepository plantedCultureRepo,
        IProductionSaleRepository saleRepo,
        IOperationalCostRepository costRepo,
        IInputPurchaseRepository purchaseRepo,
        IInputStockRepository stockRepo,
        IContractRepository contractRepo,
        IUserRepository userRepo,
        ILogger<DashboardService> logger)
    {
        _farmRepo = farmRepo;
        _producerRepo = producerRepo;
        _employeeRepo = employeeRepo;
        _machineRepo = machineRepo;
        _alertRepo = alertRepo;
        _certRepo = certRepo;
        _climateRepo = climateRepo;
        _plantedCultureRepo = plantedCultureRepo;
        _saleRepo = saleRepo;
        _costRepo = costRepo;
        _purchaseRepo = purchaseRepo;
        _stockRepo = stockRepo;
        _contractRepo = contractRepo;
        _userRepo = userRepo;
        _logger = logger;
    }

    public async Task<ProducerDashboardResponse> GetProducerDashboardAsync(Guid producerId, CancellationToken ct = default)
    {
        _logger.LogInformation("Gerando dashboard do produtor: {ProducerId}", producerId);

        var farms = (await _farmRepo.GetByProducerIdAsync(producerId, ct)).ToList();

        var totalArea = farms.Sum(f => f.TotalArea.Hectares);
        var agriculturalArea = farms.Sum(f => f.AgriculturalArea.Hectares);

        // Área plantada atual (culturas não colhidas)
        decimal totalPlanted = 0;
        int activeEmployees = 0;
        int totalMachines = 0;
        int unresolvedAlerts = 0;
        int expiringCerts = 0;

        foreach (var farm in farms)
        {
            totalPlanted += await _plantedCultureRepo.GetTotalPlantedAreaByFarmAsync(farm.Id, ct);
            activeEmployees += (await _employeeRepo.GetByFarmIdAsync(farm.Id, ct))
                .Count(e => e.IsEmployed());
            totalMachines += (await _machineRepo.GetByFarmIdAsync(farm.Id, ct)).Count();
            unresolvedAlerts += (await _alertRepo.GetUnresolvedByFarmIdAsync(farm.Id, ct)).Count();
        }

        // Certificados próximos do vencimento (30 dias)
        var allExpiringCerts = await _certRepo.GetExpiringSoonAsync(30, ct);
        expiringCerts = allExpiringCerts.Count(c => farms.Any(f => f.Id == c.FarmId));

        var utilizationPct = agriculturalArea > 0
            ? Math.Round(totalPlanted / agriculturalArea * 100, 2)
            : 0;

        return new ProducerDashboardResponse
        {
            TotalFarms = farms.Count,
            TotalAreaHa = totalArea,
            TotalAgriculturalAreaHa = agriculturalArea,
            TotalPlantedAreaHa = totalPlanted,
            AreaUtilizationPercentage = utilizationPct,
            TotalActiveEmployees = activeEmployees,
            TotalMachines = totalMachines,
            UnresolvedAlerts = unresolvedAlerts,
            ExpiringCertificates = expiringCerts
        };
    }

    public async Task<FinancialDashboardResponse> GetFinancialDashboardAsync(Guid producerId, CancellationToken ct = default)
    {
        _logger.LogInformation("Gerando dashboard financeiro do produtor: {ProducerId}", producerId);

        var farms = (await _farmRepo.GetByProducerIdAsync(producerId, ct)).ToList();
        var currentYear = DateTime.UtcNow.Year;

        decimal totalRevenue = 0;
        decimal totalCost = 0;
        decimal totalInputCost = 0;
        var monthlyRevenues = new List<MonthlyRevenue>();

        foreach (var farm in farms)
        {
            // Custos operacionais do ano
            var costs = await _costRepo.GetByDateRangeAsync(farm.Id,
                new DateTime(currentYear, 1, 1), new DateTime(currentYear, 12, 31), ct);
            totalCost += costs.Sum(c => c.Valor);

            // Compras de insumos do ano
            var purchases = (await _purchaseRepo.GetByFarmIdAsync(farm.Id, ct))
                .Where(p => p.DataCompra.Year == currentYear);
            totalInputCost += purchases.Sum(p => p.ValorTotal);

            // Vendas do ano (via culturas plantadas)
            var plantedCultures = await _plantedCultureRepo.GetByFarmIdAsync(farm.Id, ct);
            foreach (var pc in plantedCultures)
            {
                var sales = await _saleRepo.GetByPlantedCultureIdAsync(pc.Id, ct);
                var yearSales = sales.Where(s => s.DataVenda.Year == currentYear);
                totalRevenue += yearSales.Sum(s => s.ValorTotal);
            }
        }

        // Receita mensal (últimos 12 meses)
        var now = DateTime.UtcNow;
        for (int i = 11; i >= 0; i--)
        {
            var month = now.AddMonths(-i);
            decimal monthRevenue = 0;
            decimal monthCost = 0;

            foreach (var farm in farms)
            {
                var plantedCultures = await _plantedCultureRepo.GetByFarmIdAsync(farm.Id, ct);
                foreach (var pc in plantedCultures)
                {
                    var sales = await _saleRepo.GetByPlantedCultureIdAsync(pc.Id, ct);
                    monthRevenue += sales
                        .Where(s => s.DataVenda.Year == month.Year && s.DataVenda.Month == month.Month)
                        .Sum(s => s.ValorTotal);
                }

                var costs = await _costRepo.GetByDateRangeAsync(farm.Id,
                    new DateTime(month.Year, month.Month, 1),
                    new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month)), ct);
                monthCost += costs.Sum(c => c.Valor);
            }

            monthlyRevenues.Add(new MonthlyRevenue
            {
                Year = month.Year,
                Month = month.Month,
                Revenue = monthRevenue,
                Cost = monthCost
            });
        }

        var totalAllCosts = totalCost + totalInputCost;
        var profit = totalRevenue - totalAllCosts;
        var margin = totalRevenue > 0 ? Math.Round(profit / totalRevenue * 100, 2) : 0;

        // Top 5 culturas mais lucrativas
        var cultureProfits = new List<CultureProfitability>();
        foreach (var farm in farms)
        {
            var plantedCultures = await _plantedCultureRepo.GetByFarmIdAsync(farm.Id, ct);
            foreach (var pc in plantedCultures.Where(p => p.DataColheitaReal.HasValue))
            {
                var sales = await _saleRepo.GetByPlantedCultureIdAsync(pc.Id, ct);
                var revenue = sales.Sum(s => s.ValorTotal);
                var cost = pc.CustoTotal ?? 0;
                var cultureProfit = revenue - cost;

                cultureProfits.Add(new CultureProfitability
                {
                    CultureName = $"Cultura {pc.CultureId.ToString()[..8]}",
                    Safra = pc.Safra,
                    Revenue = revenue,
                    Cost = cost,
                    Profit = cultureProfit,
                    ProfitMarginPercentage = revenue > 0 ? Math.Round(cultureProfit / revenue * 100, 2) : 0
                });
            }
        }

        return new FinancialDashboardResponse
        {
            RevenueCurrentYear = totalRevenue,
            OperationalCostCurrentYear = totalCost,
            InputCostCurrentYear = totalInputCost,
            EstimatedProfit = profit,
            ProfitMarginPercentage = margin,
            MonthlyRevenues = monthlyRevenues,
            TopProfitableCultures = cultureProfits.OrderByDescending(c => c.Profit).Take(5).ToList()
        };
    }

    public async Task<FarmDashboardResponse> GetFarmDashboardAsync(Guid farmId, CancellationToken ct = default)
    {
        _logger.LogInformation("Gerando dashboard da fazenda: {FarmId}", farmId);

        var farm = await _farmRepo.GetByIdAsync(farmId, ct);
        if (farm == null) return null!;

        var plantedCultures = (await _plantedCultureRepo.GetByFarmIdAsync(farmId, ct)).ToList();
        var activeCultures = plantedCultures.Where(pc => !pc.DataColheitaReal.HasValue).ToList();
        var currentSafra = activeCultures.FirstOrDefault()?.Safra;

        var totalPlanted = activeCultures.Sum(pc => pc.AreaPlantadaHa);
        var utilizationPct = farm.AgriculturalArea.Hectares > 0
            ? Math.Round(totalPlanted / farm.AgriculturalArea.Hectares * 100, 2) : 0;

        // Financeiro da safra atual
        decimal revenueSafra = 0;
        decimal costSafra = 0;
        foreach (var pc in plantedCultures.Where(p => p.Safra == currentSafra))
        {
            var sales = await _saleRepo.GetByPlantedCultureIdAsync(pc.Id, ct);
            revenueSafra += sales.Sum(s => s.ValorTotal);
            costSafra += pc.CustoTotal ?? 0;
        }

        // Estoque
        var stocks = (await _stockRepo.GetByFarmIdAsync(farmId, ct)).ToList();
        var expiredCount = stocks.Count(s => s.IsVencido());

        // Alertas recentes
        var alerts = (await _alertRepo.GetUnresolvedByFarmIdAsync(farmId, ct))
            .OrderByDescending(a => a.Data).Take(5);

        // Clima recente
        var recentClimates = await _climateRepo.GetByDateRangeAsync(farmId,
            DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, ct);
        var lastClimate = recentClimates.OrderByDescending(c => c.Data).FirstOrDefault();

        // Contratos ativos
        var activeContracts = (await _contractRepo.GetActiveContractsAsync(farmId, ct)).Count();

        // Certificados
        var certificates = (await _certRepo.GetByFarmIdAsync(farmId, ct)).ToList();
        var expiringCerts = certificates.Count(c => c.IsValido() && c.DataValidade <= DateTime.UtcNow.AddDays(30));

        return new FarmDashboardResponse
        {
            FarmId = farm.Id,
            FarmName = farm.Name,
            TotalAreaHa = farm.TotalArea.Hectares,
            AgriculturalAreaHa = farm.AgriculturalArea.Hectares,
            VegetationAreaHa = farm.VegetationArea.Hectares,
            BuiltAreaHa = farm.BuiltArea.Hectares,
            PlantedAreaHa = totalPlanted,
            AreaUtilizationPercentage = utilizationPct,
            CurrentSafra = currentSafra,
            ActiveCulturesCount = activeCultures.Count,
            ActiveCultures = activeCultures.Select(pc => new PlantedCultureSummary
            {
                CultureName = $"Cultura {pc.CultureId.ToString()[..8]}",
                AreaHa = pc.AreaPlantadaHa,
                PlantingDate = pc.DataPlantio,
                ExpectedHarvestDate = pc.DataColheitaPrevista,
                DaysUntilHarvest = pc.DataColheitaPrevista.HasValue
                    ? (pc.DataColheitaPrevista.Value - DateTime.UtcNow).Days
                    : 0
            }).ToList(),
            RevenueCurrentSafra = revenueSafra,
            CostCurrentSafra = costSafra,
            ProfitCurrentSafra = revenueSafra - costSafra,
            TotalInputsInStock = stocks.Count(s => s.Quantidade > 0),
            ExpiredInputsCount = expiredCount,
            ActiveEmployees = (await _employeeRepo.GetByFarmIdAsync(farmId, ct)).Count(e => e.IsEmployed()),
            UnresolvedAlerts = alerts.Count(),
            RecentAlerts = alerts.Select(a => new AlertSummary
            {
                Tipo = a.Tipo,
                Nivel = a.Nivel,
                Descricao = a.Descricao,
                Data = a.Data
            }).ToList(),
            ActiveContracts = activeContracts,
            ValidCertificates = certificates.Count(c => c.IsValido()),
            ExpiringCertificates = expiringCerts,
            LastClimateRecord = lastClimate != null ? new ClimateSummary
            {
                Data = lastClimate.Data,
                Temperatura = lastClimate.Temperatura,
                ChuvaMm = lastClimate.ChuvaMm,
                Umidade = lastClimate.Umidade
            } : null
        };
    }

    public async Task<AdminDashboardResponse> GetAdminDashboardAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Gerando dashboard do administrador");

        var users = (await _userRepo.GetAllAsync(ct)).ToList();
        var producers = (await _producerRepo.GetAllAsync(ct)).ToList();
        var allFarms = new List<Domain.Entities.Farm.Farm>();

        foreach (var producer in producers)
        {
            var farms = await _farmRepo.GetByProducerIdAsync(producer.Id, ct);
            allFarms.AddRange(farms);
        }

        var totalArea = allFarms.Sum(f => f.TotalArea.Hectares);
        int totalEmployees = 0;
        int totalAlerts = 0;

        foreach (var farm in allFarms)
        {
            totalEmployees += (await _employeeRepo.GetByFarmIdAsync(farm.Id, ct)).Count(e => e.IsEmployed());
            totalAlerts += (await _alertRepo.GetUnresolvedByFarmIdAsync(farm.Id, ct)).Count();
        }

        var topProducers = producers
            .Select(p =>
            {
                var producerFarms = allFarms.Where(f => f.ProducerId == p.Id).ToList();
                return new TopProducer
                {
                    ProducerName = p.Name,
                    FarmCount = producerFarms.Count,
                    TotalAreaHa = producerFarms.Sum(f => f.TotalArea.Hectares)
                };
            })
            .OrderByDescending(tp => tp.TotalAreaHa)
            .Take(10)
            .ToList();

        return new AdminDashboardResponse
        {
            TotalUsers = users.Count,
            TotalProducers = producers.Count,
            TotalFarms = allFarms.Count,
            TotalAreaManagedHa = totalArea,
            TotalActiveEmployees = totalEmployees,
            TotalUnresolvedAlerts = totalAlerts,
            TopProducers = topProducers
        };
    }
}