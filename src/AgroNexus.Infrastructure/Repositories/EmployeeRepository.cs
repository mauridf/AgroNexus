using AgroNexus.Domain.Entities.Farm;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroNexus.Infrastructure.Repositories;

public sealed class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<IEnumerable<Employee>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(e => e.FarmId == farmId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}