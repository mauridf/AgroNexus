using AgroNexus.Domain.Entities.Farm;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroNexus.Infrastructure.Repositories;

public sealed class FarmRepository : Repository<Farm>, IFarmRepository
{
    public FarmRepository(AgroNexusDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Farm>> GetByProducerIdAsync(Guid producerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(f => f.ProducerId == producerId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Farm?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<decimal> GetTotalPlantedAreaAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await Context.PlantedCultures
            .Where(pc => pc.FarmId == farmId && pc.IsActive && !pc.DataColheitaReal.HasValue)
            .SumAsync(pc => pc.AreaPlantadaHa, cancellationToken);
    }
}