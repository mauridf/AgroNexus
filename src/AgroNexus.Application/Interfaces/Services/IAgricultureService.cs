using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;

namespace AgroNexus.Application.Interfaces.Services;

public interface IAgricultureService
{
    // Cultures
    Task<CultureResponse> CreateCultureAsync(CreateCultureRequest request, CancellationToken ct = default);
    Task<IEnumerable<CultureResponse>> GetAllCulturesAsync(CancellationToken ct = default);
    Task<CultureResponse> GetCultureByIdAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteCultureAsync(Guid id, CancellationToken ct = default);

    // Planted Cultures
    Task<PlantedCultureResponse> CreatePlantedCultureAsync(CreatePlantedCultureRequest request, CancellationToken ct = default);
    Task<IEnumerable<PlantedCultureResponse>> GetPlantedCulturesByFarmAsync(Guid farmId, CancellationToken ct = default);
    Task<PlantedCultureResponse> GetPlantedCultureByIdAsync(Guid id, CancellationToken ct = default);
    Task<PlantedCultureResponse> RegisterHarvestAsync(Guid id, HarvestRequest request, CancellationToken ct = default);
    Task SoftDeletePlantedCultureAsync(Guid id, CancellationToken ct = default);
}