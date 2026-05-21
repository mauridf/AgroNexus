using AgroNexus.Domain.Entities.Inventory;

namespace AgroNexus.Domain.Interfaces.Repositories;

public interface IInputStockRepository : IRepository<InputStock>
{
    Task<IEnumerable<InputStock>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<InputStock?> GetByFarmAndInputAsync(Guid farmId, Guid inputId, CancellationToken cancellationToken = default);
}