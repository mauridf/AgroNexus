using AgroNexus.Domain.Entities.Farm;

namespace AgroNexus.Domain.Interfaces.Repositories;

/// <summary>
/// Interface específica para o repositório de produtores.
/// </summary>
public interface IProducerRepository : IRepository<Producer>
{
    /// <summary>
    /// Busca um produtor pelo CPF/CNPJ.
    /// </summary>
    Task<Producer?> GetByCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca o produtor associado a um usuário.
    /// </summary>
    Task<Producer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se já existe um produtor com o CPF/CNPJ informado.
    /// </summary>
    Task<bool> CpfCnpjExistsAsync(string cpfCnpj, CancellationToken cancellationToken = default);
}