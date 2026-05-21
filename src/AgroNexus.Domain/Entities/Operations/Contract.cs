using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Operations;

/// <summary>
/// Representa um contrato associado a uma fazenda.
/// Tipos: Arrendamento, Parceria, Financiamento, etc.
/// </summary>
public sealed class Contract : BaseEntity
{
    /// <summary>
    /// FK para a fazenda.
    /// </summary>
    public Guid FarmId { get; private set; }

    /// <summary>
    /// Tipo do contrato: Arrendamento, Parceria, Financiamento.
    /// </summary>
    public string Tipo { get; private set; } = null!;

    /// <summary>
    /// Nome da parte contratante.
    /// </summary>
    public string? ParteContratante { get; private set; }

    /// <summary>
    /// Valor do contrato.
    /// </summary>
    public decimal Valor { get; private set; }

    /// <summary>
    /// Data de início do contrato.
    /// </summary>
    public DateTime DataInicio { get; private set; }

    /// <summary>
    /// Data de término do contrato (null se indeterminado).
    /// </summary>
    public DateTime? DataFim { get; private set; }

    private Contract() { }

    public static Contract Create(
        Guid farmId,
        string tipo,
        decimal valor,
        DateTime dataInicio,
        string? parteContratante = null,
        DateTime? dataFim = null)
    {
        if (farmId == Guid.Empty)
            throw new DomainException("Fazenda é obrigatória.", "CONTRACT_FARM_REQUIRED");

        if (string.IsNullOrWhiteSpace(tipo))
            throw new DomainException("Tipo de contrato é obrigatório.", "CONTRACT_TYPE_REQUIRED");

        if (valor <= 0)
            throw new DomainException("Valor do contrato deve ser maior que zero.", "CONTRACT_VALUE_INVALID");

        if (dataFim.HasValue && dataFim.Value < dataInicio)
            throw new DomainException("Data de término não pode ser anterior à data de início.", "CONTRACT_DATE_INVALID");

        var contract = new Contract
        {
            FarmId = farmId,
            Tipo = tipo.Trim(),
            Valor = valor,
            DataInicio = dataInicio,
            ParteContratante = parteContratante?.Trim(),
            DataFim = dataFim
        };

        return contract;
    }

    /// <summary>
    /// Verifica se o contrato está ativo (dentro do período de vigência).
    /// </summary>
    public bool IsActive()
    {
        var now = DateTime.UtcNow;
        return now >= DataInicio && (!DataFim.HasValue || now <= DataFim.Value);
    }

    /// <summary>
    /// Encerra o contrato na data especificada.
    /// </summary>
    public void Terminate(DateTime dataFim)
    {
        if (dataFim < DataInicio)
            throw new DomainException("Data de término não pode ser anterior à data de início.", "CONTRACT_DATE_INVALID");

        DataFim = dataFim;
        MarkAsUpdated();
    }
}