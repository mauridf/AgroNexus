using AgroNexus.Domain.Entities.Inventory;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroNexus.Infrastructure.Repositories;

public sealed class InputRepository : Repository<Input>, IInputRepository
{
    public InputRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<Input?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(i => i.Name == name.Trim(), cancellationToken);
    }
}

public sealed class InputPurchaseRepository : Repository<InputPurchase>, IInputPurchaseRepository
{
    public InputPurchaseRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<IEnumerable<InputPurchase>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(ip => ip.FarmId == farmId)
            .OrderByDescending(ip => ip.DataCompra)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

public sealed class InputStockRepository : Repository<InputStock>, IInputStockRepository
{
    public InputStockRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<IEnumerable<InputStock>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(s => s.FarmId == farmId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<InputStock?> GetByFarmAndInputAsync(Guid farmId, Guid inputId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(s => s.FarmId == farmId && s.InputId == inputId, cancellationToken);
    }
}