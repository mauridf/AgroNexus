using AgroNexus.Domain.Entities;

namespace AgroNexus.Domain.Interfaces.Repositories;

/// <summary>
/// Interface genérica para repositórios.
/// Define operações básicas de CRUD com suporte a soft delete.
/// </summary>
/// <typeparam name="T">Tipo da entidade (deve herdar de BaseEntity)</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Obtém uma entidade pelo ID (apenas ativas).
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todas as entidades ativas.
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona uma nova entidade.
    /// </summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma entidade existente.
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Realiza soft delete (desativa) uma entidade.
    /// </summary>
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se uma entidade existe (apenas ativas).
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}