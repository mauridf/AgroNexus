using AgroNexus.Domain.Entities.Monitoring;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroNexus.Infrastructure.Repositories;

public sealed class AlertRepository : Repository<Alert>, IAlertRepository
{
    public AlertRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<IEnumerable<Alert>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(a => a.FarmId == farmId)
            .OrderByDescending(a => a.Data)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Alert>> GetUnresolvedByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(a => a.FarmId == farmId && !a.Resolvido)
            .OrderByDescending(a => a.Data)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

public sealed class CertificateRepository : Repository<Certificate>, ICertificateRepository
{
    public CertificateRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<IEnumerable<Certificate>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.FarmId == farmId)
            .OrderByDescending(c => c.DataValidade)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Certificate>> GetExpiringSoonAsync(int daysThreshold, CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddDays(daysThreshold);
        return await DbSet
            .Where(c => c.DataValidade <= threshold && c.DataValidade >= DateTime.UtcNow)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

public sealed class ClimateRepository : Repository<Climate>, IClimateRepository
{
    public ClimateRepository(AgroNexusDbContext context) : base(context) { }

    public async Task<IEnumerable<Climate>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.FarmId == farmId)
            .OrderByDescending(c => c.Data)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Climate>> GetByDateRangeAsync(Guid farmId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.FarmId == farmId && c.Data >= start && c.Data <= end)
            .OrderBy(c => c.Data)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}