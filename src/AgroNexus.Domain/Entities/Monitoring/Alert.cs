using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Monitoring;

/// <summary>
/// Representa um alerta de monitoramento registrado para uma fazenda.
/// Pode ser sobre pragas, doenças, clima, maquinário, etc.
/// </summary>
public sealed class Alert : BaseEntity
{
    /// <summary>
    /// FK para a fazenda.
    /// </summary>
    public Guid FarmId { get; private set; }

    /// <summary>
    /// Tipo do alerta: praga, doença, clima, maquinário.
    /// </summary>
    public string Tipo { get; private set; } = null!;

    /// <summary>
    /// Descrição detalhada do alerta.
    /// </summary>
    public string? Descricao { get; private set; }

    /// <summary>
    /// Nível de severidade: baixo, médio, alto.
    /// </summary>
    public string Nivel { get; private set; } = null!;

    /// <summary>
    /// Data do alerta.
    /// </summary>
    public DateTime Data { get; private set; }

    /// <summary>
    /// Indica se o alerta foi resolvido.
    /// </summary>
    public bool Resolvido { get; private set; }

    private Alert() { }

    public static Alert Create(Guid farmId, string tipo, string nivel, DateTime data, string? descricao = null)
    {
        if (farmId == Guid.Empty)
            throw new DomainException("Fazenda é obrigatória.", "ALERT_FARM_REQUIRED");

        if (string.IsNullOrWhiteSpace(tipo))
            throw new DomainException("Tipo de alerta é obrigatório.", "ALERT_TYPE_REQUIRED");

        if (string.IsNullOrWhiteSpace(nivel))
            throw new DomainException("Nível de severidade é obrigatório.", "ALERT_LEVEL_REQUIRED");

        var niveisValidos = new[] { "baixo", "médio", "medio", "alto" };
        if (!niveisValidos.Contains(nivel.ToLowerInvariant()))
            throw new DomainException("Nível inválido. Use: baixo, médio ou alto.", "ALERT_LEVEL_INVALID");

        var alert = new Alert
        {
            FarmId = farmId,
            Tipo = tipo.Trim(),
            Nivel = nivel.Trim().ToLowerInvariant(),
            Data = data,
            Descricao = descricao?.Trim(),
            Resolvido = false
        };

        return alert;
    }

    /// <summary>
    /// Marca o alerta como resolvido.
    /// </summary>
    public void Resolve()
    {
        Resolvido = true;
        MarkAsUpdated();
    }
}