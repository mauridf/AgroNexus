using AgroNexus.Domain.Entities.Identity;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Domain.ValueObjects;
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
        var emailVO = Email.Create(email); // Cria ValueObject para comparação
        return await DbSet
            .FirstOrDefaultAsync(u => u.Email == emailVO, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailVO = Email.Create(email);
        return await DbSet
            .AnyAsync(u => u.Email == emailVO, cancellationToken);
    }
}