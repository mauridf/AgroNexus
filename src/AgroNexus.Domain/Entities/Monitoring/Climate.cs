using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Monitoring;

/// <summary>
/// Representa um registro climático para uma fazenda em uma data específica.
/// </summary>
public sealed class Climate : BaseEntity
{
    /// <summary>
    /// FK para a fazenda.
    /// </summary>
    public Guid FarmId { get; private set; }

    /// <summary>
    /// Data do registro climático.
    /// </summary>
    public DateTime Data { get; private set; }

    /// <summary>
    /// Temperatura registrada em graus Celsius.
    /// </summary>
    public decimal? Temperatura { get; private set; }

    /// <summary>
    /// Chuva em milímetros.
    /// </summary>
    public decimal? ChuvaMm { get; private set; }

    /// <summary>
    /// Umidade relativa do ar em porcentagem (0-100).
    /// </summary>
    public decimal? Umidade { get; private set; }

    private Climate() { }

    public static Climate Create(Guid farmId, DateTime data, decimal? temperatura = null, decimal? chuvaMm = null, decimal? umidade = null)
    {
        if (farmId == Guid.Empty)
            throw new DomainException("Fazenda é obrigatória.", "CLIMATE_FARM_REQUIRED");

        if (data > DateTime.UtcNow)
            throw new DomainException("Data não pode ser futura.", "CLIMATE_DATE_INVALID");

        if (umidade.HasValue && (umidade.Value < 0 || umidade.Value > 100))
            throw new DomainException("Umidade deve estar entre 0 e 100.", "CLIMATE_HUMIDITY_INVALID");

        var climate = new Climate
        {
            FarmId = farmId,
            Data = data,
            Temperatura = temperatura,
            ChuvaMm = chuvaMm,
            Umidade = umidade
        };

        return climate;
    }
}