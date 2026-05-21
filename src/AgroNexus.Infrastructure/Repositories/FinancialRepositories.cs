using AgroNexus.Domain.Entities.Financial;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroNexus.Infrastructure.Repositories;

public sealed class ProductionSaleRepository : Repository<ProductionSale>, IProductionSaleRepository
{
    public ProductionSaleRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<IEnumerable<ProductionSale>> GetByPlantedCultureIdAsync(Guid plantedCultureId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(ps => ps.PlantedCultureId == plantedCultureId)
            .OrderByDescending(ps => ps.DataVenda)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductionSale>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(ps => ps.PlantedCultureId != Guid.Empty) // placeholder
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}