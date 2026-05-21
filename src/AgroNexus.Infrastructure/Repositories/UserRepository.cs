using AgroNexus.Domain.Entities.Identity;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroNexus.Infrastructure.Repositories;

public sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AgroNexusDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant().Trim(), cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(u => u.Email == email.ToLowerInvariant().Trim(), cancellationToken);
    }
}