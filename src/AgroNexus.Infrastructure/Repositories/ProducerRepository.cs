using AgroNexus.Domain.Entities.Farm;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroNexus.Infrastructure.Repositories;

public sealed class ProducerRepository : Repository<Producer>, IProducerRepository
{
    public ProducerRepository(AgroNexusDbContext context) : base(context)
    {
    }

    public async Task<Producer?> GetByCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken = default)
    {
        var cpfCnpjLimpo = new string(cpfCnpj.Where(char.IsDigit).ToArray());
        return await DbSet
            .FirstOrDefaultAsync(p => p.CpfCnpj == cpfCnpjLimpo, cancellationToken);
    }

    public async Task<Producer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<bool> CpfCnpjExistsAsync(string cpfCnpj, CancellationToken cancellationToken = default)
    {
        var cpfCnpjLimpo = new string(cpfCnpj.Where(char.IsDigit).ToArray());
        return await DbSet
            .AnyAsync(p => p.CpfCnpj == cpfCnpjLimpo, cancellationToken);
    }
}