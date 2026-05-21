using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;

namespace AgroNexus.Application.Interfaces.Services;

public interface IFarmManagementService
{
    // Producers
    Task<ProducerResponse> CreateProducerAsync(CreateProducerRequest request, CancellationToken cancellationToken = default);
    Task<ProducerResponse> GetProducerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProducerResponse>> GetAllProducersAsync(CancellationToken cancellationToken = default);
    Task<ProducerResponse> UpdateProducerAsync(Guid id, UpdateProducerRequest request, CancellationToken cancellationToken = default);
    Task SoftDeleteProducerAsync(Guid id, CancellationToken cancellationToken = default);

    // Farms
    Task<FarmResponse> CreateFarmAsync(CreateFarmRequest request, CancellationToken cancellationToken = default);
    Task<FarmResponse> GetFarmByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FarmResponse>> GetFarmsByProducerAsync(Guid producerId, CancellationToken cancellationToken = default);
    Task<FarmResponse> UpdateFarmAsync(Guid id, UpdateFarmRequest request, CancellationToken cancellationToken = default);
    Task SoftDeleteFarmAsync(Guid id, CancellationToken cancellationToken = default);
}