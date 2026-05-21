using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;
using AgroNexus.Domain.Entities.Inventory;
using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.Interfaces.Repositories;
using Mapster;
using Microsoft.Extensions.Logging;

namespace AgroNexus.Application.Services;

public sealed class InventoryService : IInventoryService
{
    private readonly IInputRepository _inputRepo;
    private readonly IInputPurchaseRepository _purchaseRepo;
    private readonly IInputStockRepository _stockRepo;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(
        IInputRepository inputRepo,
        IInputPurchaseRepository purchaseRepo,
        IInputStockRepository stockRepo,
        ILogger<InventoryService> logger)
    {
        _inputRepo = inputRepo;
        _purchaseRepo = purchaseRepo;
        _stockRepo = stockRepo;
        _logger = logger;
    }

    public async Task<InputResponse> CreateInputAsync(CreateInputRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando insumo: {Name}", request.Name);
        var input = Input.Create(request.Name, request.Tipo, request.UnidadeMedida, request.Fornecedor);
        await _inputRepo.AddAsync(input, ct);
        return input.Adapt<InputResponse>();
    }

    public async Task<IEnumerable<InputResponse>> GetAllInputsAsync(CancellationToken ct = default)
    {
        var inputs = await _inputRepo.GetAllAsync(ct);
        return inputs.Adapt<IEnumerable<InputResponse>>();
    }

    public async Task<InputStockResponse> PurchaseInputAsync(CreateInputPurchaseRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Compra de insumo {InputId} para fazenda {FarmId}", request.InputId, request.FarmId);

        var purchase = InputPurchase.Create(request.FarmId, request.InputId, request.Quantidade, request.ValorTotal, request.DataCompra, request.Fornecedor);
        await _purchaseRepo.AddAsync(purchase, ct);

        // Atualizar estoque
        var stock = await _stockRepo.GetByFarmAndInputAsync(request.FarmId, request.InputId, ct);
        if (stock == null)
        {
            stock = InputStock.Create(request.FarmId, request.InputId, request.Quantidade);
            await _stockRepo.AddAsync(stock, ct);
        }
        else
        {
            stock.AddStock(request.Quantidade);
            await _stockRepo.UpdateAsync(stock, ct);
        }

        _logger.LogInformation("Estoque atualizado: {StockId} = {Qty}", stock.Id, stock.Quantidade);
        return stock.Adapt<InputStockResponse>();
    }

    public async Task<IEnumerable<InputStockResponse>> GetStockByFarmAsync(Guid farmId, CancellationToken ct = default)
    {
        var stocks = await _stockRepo.GetByFarmIdAsync(farmId, ct);
        return stocks.Adapt<IEnumerable<InputStockResponse>>();
    }
}