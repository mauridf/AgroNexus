using AgroNexus.Domain.Entities.Identity;

namespace AgroNexus.Domain.Interfaces.Repositories;

/// <summary>
/// Interface específica para o repositório de usuários.
/// Contém operações além do CRUD básico.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Busca um usuário pelo email (usado no login).
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se já existe um usuário com o email informado.
    /// </summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}