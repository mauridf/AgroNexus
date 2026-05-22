using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.ValueObjects;

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

    public Coordinate? Location { get; private set; }
    public Area TotalArea { get; private set; } = null!;
    public Area AgriculturalArea { get; private set; } = null!;
    public Area VegetationArea { get; private set; } = null!;
    public Area BuiltArea { get; private set; } = null!;


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

    // Propriedades de compatibilidade para o EF Core
    /// <summary>
    /// Área total da fazenda em hectares.
    /// </summary>
    public decimal TotalAreaHa => TotalArea.Hectares;
    /// <summary>
    /// Área agricultável em hectares.
    /// </summary>
    public decimal AgriculturalAreaHa => AgriculturalArea.Hectares;
    /// <summary>
    /// Área de vegetação nativa em hectares.
    /// </summary>
    public decimal VegetationAreaHa => VegetationArea.Hectares;
    /// <summary>
    /// Área construída (benfeitorias) em hectares.
    /// </summary>
    public decimal BuiltAreaHa => BuiltArea.Hectares;
    /// <summary>
    /// Latitude da localização da fazenda.
    /// </summary>
    public decimal? Latitude => Location?.Latitude;
    /// <summary>
    /// Longitude da localização da fazenda.
    /// </summary>
    public decimal? Longitude => Location?.Longitude;

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

        var total = Area.FromHectares(totalAreaHa);
        var agricultural = Area.FromHectares(agriculturalAreaHa);
        var vegetation = Area.FromHectares(vegetationAreaHa);
        var built = Area.FromHectares(builtAreaHa);

        // Valida soma das áreas usando ValueObject
        Area.ValidateSum(total, agricultural, vegetation, built);

        Coordinate? coord = null;
        if (latitude.HasValue && longitude.HasValue)
            coord = Coordinate.Create(latitude.Value, longitude.Value);

        var farm = new Farm
        {
            ProducerId = producerId,
            Name = name.Trim(),
            TotalArea = total,
            AgriculturalArea = agricultural,
            VegetationArea = vegetation,
            BuiltArea = built,
            Endereco = endereco?.Trim(),
            Cidade = cidade?.Trim(),
            Estado = estado?.Trim().ToUpperInvariant(),
            Location = coord,
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

        var total = Area.FromHectares(totalAreaHa);
        var agricultural = Area.FromHectares(agriculturalAreaHa);
        var vegetation = Area.FromHectares(vegetationAreaHa);
        var built = Area.FromHectares(builtAreaHa);

        Area.ValidateSum(total, agricultural, vegetation, built);

        Name = name.Trim();
        TotalArea = total;
        AgriculturalArea = agricultural;
        VegetationArea = vegetation;
        BuiltArea = built;
        Endereco = endereco?.Trim();
        Cidade = cidade?.Trim();
        Estado = estado?.Trim().ToUpperInvariant();
        InscricaoEstadual = inscricaoEstadual?.Trim();
        CodigoCar = codigoCar?.Trim();
        Ccir = ccir?.Trim();
        FonteAgua = fonteAgua?.Trim();

        if (latitude.HasValue && longitude.HasValue)
            Location = Coordinate.Create(latitude.Value, longitude.Value);

        MarkAsUpdated();
    }
}