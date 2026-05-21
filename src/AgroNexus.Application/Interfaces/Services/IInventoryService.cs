using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;

namespace AgroNexus.Application.Interfaces.Services;

public interface IInventoryService
{
    // Inputs
    Task<InputResponse> CreateInputAsync(CreateInputRequest request, CancellationToken ct = default);
    Task<IEnumerable<InputResponse>> GetAllInputsAsync(CancellationToken ct = default);

    // Purchases
    Task<InputStockResponse> PurchaseInputAsync(CreateInputPurchaseRequest request, CancellationToken ct = default);
    Task<IEnumerable<InputStockResponse>> GetStockByFarmAsync(Guid farmId, CancellationToken ct = default);
}