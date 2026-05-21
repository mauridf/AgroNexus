using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;
using AgroNexus.Domain.Entities.Financial;
using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.Interfaces.Repositories;
using Mapster;
using Microsoft.Extensions.Logging;

namespace AgroNexus.Application.Services;

public sealed class FinancialService : IFinancialService
{
    private readonly IProductionSaleRepository _saleRepo;
    private readonly ILogger<FinancialService> _logger;

    public FinancialService(IProductionSaleRepository saleRepo, ILogger<FinancialService> logger)
    {
        _saleRepo = saleRepo;
        _logger = logger;
    }

    public async Task<ProductionSaleResponse> CreateSaleAsync(CreateProductionSaleRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando venda para cultura plantada {Id}", request.PlantedCultureId);
        var sale = ProductionSale.Create(request.PlantedCultureId, request.QuantidadeVendida, request.PrecoUnitario, request.DataVenda, request.Destino);
        await _saleRepo.AddAsync(sale, ct);
        _logger.LogInformation("Venda criada. Valor total: {Valor}", sale.ValorTotal);
        return sale.Adapt<ProductionSaleResponse>();
    }

    public async Task<IEnumerable<ProductionSaleResponse>> GetSalesByFarmAsync(Guid farmId, CancellationToken ct = default)
    {
        var sales = await _saleRepo.GetByFarmIdAsync(farmId, ct);
        return sales.Adapt<IEnumerable<ProductionSaleResponse>>();
    }
}