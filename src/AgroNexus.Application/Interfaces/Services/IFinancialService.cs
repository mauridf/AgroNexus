using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;

namespace AgroNexus.Application.Interfaces.Services;

public interface IFinancialService
{
    Task<ProductionSaleResponse> CreateSaleAsync(CreateProductionSaleRequest request, CancellationToken ct = default);
    Task<IEnumerable<ProductionSaleResponse>> GetSalesByFarmAsync(Guid farmId, CancellationToken ct = default);
}