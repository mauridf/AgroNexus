namespace AgroNexus.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades do sistema.
/// Fornece propriedades comuns como Id, datas de criação/atualização e soft delete.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único global da entidade.
    /// Usamos Guid para evitar conflitos em sistemas distribuídos.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Data e hora de criação do registro (UTC).
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Data e hora da última atualização do registro (UTC).
    /// Nullable porque no momento da criação ainda não houve atualização.
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>
    /// Indicador de soft delete (deleção lógica).
    /// true = ativo, false = desativado/excluído logicamente.
    /// </summary>
    public bool IsActive { get; protected set; }

    /// <summary>
    /// Construtor protegido para ser usado pelas subclasses e pelo EF Core.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// Marca a entidade como atualizada, alterando o UpdatedAt.
    /// </summary>
    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Realiza soft delete da entidade.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Reativa a entidade após soft delete.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }
}