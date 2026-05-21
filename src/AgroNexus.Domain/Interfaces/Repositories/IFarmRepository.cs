using AgroNexus.Domain.Entities.Farm;

namespace AgroNexus.Domain.Interfaces.Repositories;

/// <summary>
/// Interface específica para o repositório de fazendas.
/// </summary>
public interface IFarmRepository : IRepository<Farm>
{
    /// <summary>
    /// Busca todas as fazendas de um produtor.
    /// </summary>
    Task<IEnumerable<Farm>> GetByProducerIdAsync(Guid producerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca uma fazenda com todos os relacionamentos carregados.
    /// </summary>
    Task<Farm?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcula a área total já plantada em uma fazenda (soma das culturas plantadas ativas).
    /// </summary>
    Task<decimal> GetTotalPlantedAreaAsync(Guid farmId, CancellationToken cancellationToken = default);
}