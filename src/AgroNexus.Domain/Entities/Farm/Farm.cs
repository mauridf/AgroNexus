using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Farm;

/// <summary>
/// Representa uma fazenda/propriedade rural associada a um produtor.
/// Contém regras de negócio para validação de áreas.
/// </summary>
public sealed class Farm : BaseEntity
{
    /// <summary>
    /// FK para o produtor proprietário da fazenda.
    /// </summary>
    public Guid ProducerId { get; private set; }

    /// <summary>
    /// Nome da fazenda/propriedade.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Endereço completo da fazenda.
    /// </summary>
    public string? Endereco { get; private set; }

    /// <summary>
    /// Cidade onde a fazenda está localizada.
    /// </summary>
    public string? Cidade { get; private set; }

    /// <summary>
    /// Estado (UF) onde a fazenda está localizada.
    /// </summary>
    public string? Estado { get; private set; }

    /// <summary>
    /// Latitude da localização da fazenda.
    /// </summary>
    public decimal? Latitude { get; private set; }

    /// <summary>
    /// Longitude da localização da fazenda.
    /// </summary>
    public decimal? Longitude { get; private set; }

    /// <summary>
    /// Área total da fazenda em hectares.
    /// </summary>
    public decimal TotalAreaHa { get; private set; }

    /// <summary>
    /// Área agricultável em hectares.
    /// </summary>
    public decimal AgriculturalAreaHa { get; private set; }

    /// <summary>
    /// Área de vegetação nativa em hectares.
    /// </summary>
    public decimal VegetationAreaHa { get; private set; }

    /// <summary>
    /// Área construída (benfeitorias) em hectares.
    /// </summary>
    public decimal BuiltAreaHa { get; private set; }

    /// <summary>
    /// Inscrição Estadual da propriedade.
    /// </summary>
    public string? InscricaoEstadual { get; private set; }

    /// <summary>
    /// CAR - Cadastro Ambiental Rural da propriedade.
    /// </summary>
    public string? CodigoCar { get; private set; }

    /// <summary>
    /// CCIR - Certificado de Cadastro de Imóvel Rural.
    /// </summary>
    public string? Ccir { get; private set; }

    /// <summary>
    /// Fonte de água disponível na propriedade.
    /// </summary>
    public string? FonteAgua { get; private set; }

    // Navegação
    // public Producer Producer { get; private set; } = null!;
    // public ICollection<Employee> Employees { get; private set; } = new List<Employee>();

    private Farm() { }

    public static Farm Create(
        Guid producerId,
        string name,
        decimal totalAreaHa,
        decimal agriculturalAreaHa,
        decimal vegetationAreaHa,
        decimal builtAreaHa,
        string? endereco = null,
        string? cidade = null,
        string? estado = null,
        decimal? latitude = null,
        decimal? longitude = null,
        string? inscricaoEstadual = null,
        string? codigoCar = null,
        string? ccir = null,
        string? fonteAgua = null)
    {
        if (producerId == Guid.Empty)
            throw new DomainException("Produtor é obrigatório.", "FARM_PRODUCER_REQUIRED");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da fazenda é obrigatório.", "FARM_NAME_REQUIRED");

        if (totalAreaHa <= 0)
            throw new DomainException("Área total deve ser maior que zero.", "FARM_TOTAL_AREA_INVALID");

        if (agriculturalAreaHa < 0)
            throw new DomainException("Área agricultável não pode ser negativa.", "FARM_AGRI_AREA_INVALID");

        if (vegetationAreaHa < 0)
            throw new DomainException("Área de vegetação não pode ser negativa.", "FARM_VEG_AREA_INVALID");

        if (builtAreaHa < 0)
            throw new DomainException("Área construída não pode ser negativa.", "FARM_BUILT_AREA_INVALID");

        // REGRA DE NEGÓCIO CRÍTICA
        var somaAreas = agriculturalAreaHa + vegetationAreaHa + builtAreaHa;
        if (somaAreas > totalAreaHa)
            throw new DomainException(
                $"A soma das áreas ({somaAreas:F2}ha) não pode exceder a área total ({totalAreaHa:F2}ha). " +
                $"Diferença excedente: {somaAreas - totalAreaHa:F2}ha.",
                "FARM_AREA_EXCEEDS_TOTAL");

        var farm = new Farm
        {
            ProducerId = producerId,
            Name = name.Trim(),
            TotalAreaHa = totalAreaHa,
            AgriculturalAreaHa = agriculturalAreaHa,
            VegetationAreaHa = vegetationAreaHa,
            BuiltAreaHa = builtAreaHa,
            Endereco = endereco?.Trim(),
            Cidade = cidade?.Trim(),
            Estado = estado?.Trim().ToUpperInvariant(),
            Latitude = latitude,
            Longitude = longitude,
            InscricaoEstadual = inscricaoEstadual?.Trim(),
            CodigoCar = codigoCar?.Trim(),
            Ccir = ccir?.Trim(),
            FonteAgua = fonteAgua?.Trim()
        };

        return farm;
    }

    /// <summary>
    /// Verifica se a área plantada informada é compatível com a área agricultável.
    /// </summary>
    /// <param name="areaToPlant">Área que se deseja plantar em hectares</param>
    /// <param name="currentPlantedArea">Área já plantada atualmente em hectares</param>
    /// <returns>True se a área pode ser plantada</returns>
    public bool CanPlantArea(decimal areaToPlant, decimal currentPlantedArea = 0)
    {
        return (currentPlantedArea + areaToPlant) <= AgriculturalAreaHa;
    }

    /// <summary>
    /// Atualiza os dados da fazenda, revalidando as áreas.
    /// </summary>
    public void Update(
        string name,
        decimal totalAreaHa,
        decimal agriculturalAreaHa,
        decimal vegetationAreaHa,
        decimal builtAreaHa,
        string? endereco = null,
        string? cidade = null,
        string? estado = null,
        decimal? latitude = null,
        decimal? longitude = null,
        string? inscricaoEstadual = null,
        string? codigoCar = null,
        string? ccir = null,
        string? fonteAgua = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da fazenda é obrigatório.", "FARM_NAME_REQUIRED");

        var somaAreas = agriculturalAreaHa + vegetationAreaHa + builtAreaHa;
        if (somaAreas > totalAreaHa)
            throw new DomainException(
                $"A soma das áreas ({somaAreas:F2}ha) não pode exceder a área total ({totalAreaHa:F2}ha).",
                "FARM_AREA_EXCEEDS_TOTAL");

        Name = name.Trim();
        TotalAreaHa = totalAreaHa;
        AgriculturalAreaHa = agriculturalAreaHa;
        VegetationAreaHa = vegetationAreaHa;
        BuiltAreaHa = builtAreaHa;
        Endereco = endereco?.Trim();
        Cidade = cidade?.Trim();
        Estado = estado?.Trim().ToUpperInvariant();
        Latitude = latitude;
        Longitude = longitude;
        InscricaoEstadual = inscricaoEstadual?.Trim();
        CodigoCar = codigoCar?.Trim();
        Ccir = ccir?.Trim();
        FonteAgua = fonteAgua?.Trim();

        MarkAsUpdated();
    }
}