using AgroNexus.Domain.Entities;
using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroNexus.Infrastructure.Repositories;

/// <summary>
/// Implementação genérica do repositório base.
/// Fornece operações CRUD com suporte a soft delete e filtro global de ativos.
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public abstract class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AgroNexusDbContext Context;
    protected readonly DbSet<T> DbSet;

    protected Repository(AgroNexusDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Ignora o filtro global para buscar também entidades inativas (útil para verificação)
        return await DbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(typeof(T).Name, id);

        entity.Deactivate();
        await UpdateAsync(entity, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(e => e.Id == id && e.IsActive, cancellationToken);
    }
}