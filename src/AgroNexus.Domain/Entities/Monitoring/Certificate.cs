using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Monitoring;

/// <summary>
/// Representa um certificado associado a uma fazenda.
/// Exemplos: Orgânico, FairTrade, Rainforest Alliance.
/// </summary>
public sealed class Certificate : BaseEntity
{
    /// <summary>
    /// FK para a fazenda.
    /// </summary>
    public Guid FarmId { get; private set; }

    /// <summary>
    /// Tipo do certificado: Orgânico, FairTrade, etc.
    /// </summary>
    public string Tipo { get; private set; } = null!;

    /// <summary>
    /// Data de emissão do certificado.
    /// </summary>
    public DateTime DataEmissao { get; private set; }

    /// <summary>
    /// Data de validade do certificado.
    /// </summary>
    public DateTime DataValidade { get; private set; }

    private Certificate() { }

    public static Certificate Create(Guid farmId, string tipo, DateTime dataEmissao, DateTime dataValidade)
    {
        if (farmId == Guid.Empty)
            throw new DomainException("Fazenda é obrigatória.", "CERT_FARM_REQUIRED");

        if (string.IsNullOrWhiteSpace(tipo))
            throw new DomainException("Tipo de certificado é obrigatório.", "CERT_TYPE_REQUIRED");

        if (dataValidade <= dataEmissao)
            throw new DomainException("Data de validade deve ser posterior à data de emissão.", "CERT_DATE_INVALID");

        var certificate = new Certificate
        {
            FarmId = farmId,
            Tipo = tipo.Trim(),
            DataEmissao = dataEmissao,
            DataValidade = dataValidade
        };

        return certificate;
    }

    /// <summary>
    /// Verifica se o certificado está vencido.
    /// </summary>
    public bool IsVencido() => DateTime.UtcNow > DataValidade;

    /// <summary>
    /// Verifica se o certificado está dentro da validade.
    /// </summary>
    public bool IsValido() => DateTime.UtcNow >= DataEmissao && DateTime.UtcNow <= DataValidade;
}