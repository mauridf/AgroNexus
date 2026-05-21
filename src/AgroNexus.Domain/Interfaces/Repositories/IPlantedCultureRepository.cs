using AgroNexus.Domain.Entities.Agriculture;

namespace AgroNexus.Domain.Interfaces.Repositories;

public interface IPlantedCultureRepository : IRepository<PlantedCulture>
{
    Task<IEnumerable<PlantedCulture>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlantedCulture>> GetBySafraAsync(Guid farmId, string safra, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPlantedAreaByFarmAsync(Guid farmId, CancellationToken cancellationToken = default);
}