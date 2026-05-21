using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Agriculture;

/// <summary>
/// Representa uma cultura plantada em uma fazenda específica.
/// Registra informações da safra, área plantada, produtividade e custos.
/// </summary>
public sealed class PlantedCulture : BaseEntity
{
    /// <summary>
    /// FK para a fazenda onde a cultura foi plantada.
    /// </summary>
    public Guid FarmId { get; private set; }

    /// <summary>
    /// FK para a cultura (catálogo).
    /// </summary>
    public Guid CultureId { get; private set; }

    /// <summary>
    /// Identificação da safra (ex: "2024/2025", "2025", "Safra-Verão").
    /// </summary>
    public string Safra { get; private set; } = null!;

    /// <summary>
    /// Área plantada em hectares.
    /// </summary>
    public decimal AreaPlantadaHa { get; private set; }

    /// <summary>
    /// Data do plantio.
    /// </summary>
    public DateTime? DataPlantio { get; private set; }

    /// <summary>
    /// Data prevista para colheita.
    /// </summary>
    public DateTime? DataColheitaPrevista { get; private set; }

    /// <summary>
    /// Data real da colheita (preenchida quando a colheita é realizada).
    /// </summary>
    public DateTime? DataColheitaReal { get; private set; }

    /// <summary>
    /// Produtividade esperada em sacas por hectare.
    /// </summary>
    public decimal? ProdutividadeEsperadaSacasHa { get; private set; }

    /// <summary>
    /// Produtividade obtida em sacas por hectare (preenchida após colheita).
    /// </summary>
    public decimal? ProdutividadeObtidaSacasHa { get; private set; }

    /// <summary>
    /// Custo total da cultura plantada.
    /// </summary>
    public decimal? CustoTotal { get; private set; }

    /// <summary>
    /// Receita total obtida com a venda da produção.
    /// </summary>
    public decimal? ReceitaTotal { get; private set; }

    private PlantedCulture() { }

    public static PlantedCulture Create(
        Guid farmId,
        Guid cultureId,
        string safra,
        decimal areaPlantadaHa,
        DateTime? dataPlantio = null,
        DateTime? dataColheitaPrevista = null,
        decimal? produtividadeEsperadaSacasHa = null,
        decimal? custoTotal = null)
    {
        if (farmId == Guid.Empty)
            throw new DomainException("Fazenda é obrigatória.", "PLANTED_CULTURE_FARM_REQUIRED");

        if (cultureId == Guid.Empty)
            throw new DomainException("Cultura é obrigatória.", "PLANTED_CULTURE_CULTURE_REQUIRED");

        if (string.IsNullOrWhiteSpace(safra))
            throw new DomainException("Safra é obrigatória.", "PLANTED_CULTURE_SAFRA_REQUIRED");

        if (areaPlantadaHa <= 0)
            throw new DomainException("Área plantada deve ser maior que zero.", "PLANTED_CULTURE_AREA_INVALID");

        // Nota: A validação se a área plantada não excede a área agricultável da fazenda
        // será feita na camada de serviço, pois depende do repositório (Farm).

        var plantedCulture = new PlantedCulture
        {
            FarmId = farmId,
            CultureId = cultureId,
            Safra = safra.Trim(),
            AreaPlantadaHa = areaPlantadaHa,
            DataPlantio = dataPlantio,
            DataColheitaPrevista = dataColheitaPrevista,
            ProdutividadeEsperadaSacasHa = produtividadeEsperadaSacasHa,
            CustoTotal = custoTotal
        };

        return plantedCulture;
    }

    /// <summary>
    /// Registra a colheita real com data e produtividade obtida.
    /// </summary>
    public void RegisterHarvest(DateTime dataColheitaReal, decimal produtividadeObtidaSacasHa, decimal receitaTotal)
    {
        if (dataColheitaReal < DataPlantio)
            throw new DomainException("Data da colheita não pode ser anterior ao plantio.", "PLANTED_CULTURE_HARVEST_DATE_INVALID");

        if (produtividadeObtidaSacasHa < 0)
            throw new DomainException("Produtividade obtida não pode ser negativa.", "PLANTED_CULTURE_PRODUTIVITY_INVALID");

        DataColheitaReal = dataColheitaReal;
        ProdutividadeObtidaSacasHa = produtividadeObtidaSacasHa;
        ReceitaTotal = receitaTotal;

        MarkAsUpdated();
    }

    /// <summary>
    /// Calcula o lucro da cultura plantada.
    /// </summary>
    public decimal? CalcularLucro()
    {
        if (!ReceitaTotal.HasValue || !CustoTotal.HasValue)
            return null;

        return ReceitaTotal.Value - CustoTotal.Value;
    }

    public void Update(
        string safra,
        decimal areaPlantadaHa,
        DateTime? dataPlantio,
        DateTime? dataColheitaPrevista,
        decimal? produtividadeEsperadaSacasHa,
        decimal? custoTotal)
    {
        if (string.IsNullOrWhiteSpace(safra))
            throw new DomainException("Safra é obrigatória.", "PLANTED_CULTURE_SAFRA_REQUIRED");

        if (areaPlantadaHa <= 0)
            throw new DomainException("Área plantada deve ser maior que zero.", "PLANTED_CULTURE_AREA_INVALID");

        Safra = safra.Trim();
        AreaPlantadaHa = areaPlantadaHa;
        DataPlantio = dataPlantio;
        DataColheitaPrevista = dataColheitaPrevista;
        ProdutividadeEsperadaSacasHa = produtividadeEsperadaSacasHa;
        CustoTotal = custoTotal;

        MarkAsUpdated();
    }
}