using AgroNexus.Domain.Entities.Agriculture;

namespace AgroNexus.Domain.Interfaces.Repositories;

public interface ICultureRepository : IRepository<Culture>
{
    Task<Culture?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
}