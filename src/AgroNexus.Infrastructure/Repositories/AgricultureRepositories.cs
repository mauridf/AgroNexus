using AgroNexus.Domain.Entities.Agriculture;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroNexus.Infrastructure.Repositories;

public sealed class CultureRepository : Repository<Culture>, ICultureRepository
{
    public CultureRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<Culture?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Name == name.Trim(), cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(c => c.Name == name.Trim(), cancellationToken);
    }
}

public sealed class PlantedCultureRepository : Repository<PlantedCulture>, IPlantedCultureRepository
{
    public PlantedCultureRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<IEnumerable<PlantedCulture>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(pc => pc.FarmId == farmId)
            .OrderByDescending(pc => pc.DataPlantio)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PlantedCulture>> GetBySafraAsync(Guid farmId, string safra, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(pc => pc.FarmId == farmId && pc.Safra == safra.Trim())
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalPlantedAreaByFarmAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(pc => pc.FarmId == farmId && !pc.DataColheitaReal.HasValue)
            .SumAsync(pc => pc.AreaPlantadaHa, cancellationToken);
    }
}