using AgroNexus.Domain.Entities.Operations;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroNexus.Infrastructure.Repositories;

public sealed class ContractRepository : Repository<Contract>, IContractRepository
{
    public ContractRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<IEnumerable<Contract>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.FarmId == farmId)
            .OrderByDescending(c => c.DataInicio)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Contract>> GetActiveContractsAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await DbSet
            .Where(c => c.FarmId == farmId && c.DataInicio <= now && (c.DataFim == null || c.DataFim >= now))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

public sealed class OperationalCostRepository : Repository<OperationalCost>, IOperationalCostRepository
{
    public OperationalCostRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<IEnumerable<OperationalCost>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(oc => oc.FarmId == farmId)
            .OrderByDescending(oc => oc.Data)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OperationalCost>> GetByDateRangeAsync(Guid farmId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(oc => oc.FarmId == farmId && oc.Data >= start && oc.Data <= end)
            .OrderByDescending(oc => oc.Data)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

public sealed class MachineRepository : Repository<Machine>, IMachineRepository
{
    public MachineRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<IEnumerable<Machine>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.FarmId == farmId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}