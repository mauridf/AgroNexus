using AgroNexus.Domain.Entities.Inventory;

namespace AgroNexus.Domain.Interfaces.Repositories;

public interface IInputRepository : IRepository<Input>
{
    Task<Input?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}