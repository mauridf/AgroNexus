using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;
using AgroNexus.Domain.Entities.Agriculture;
using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.Interfaces.Repositories;
using Mapster;
using Microsoft.Extensions.Logging;

namespace AgroNexus.Application.Services;

public sealed class AgricultureService : IAgricultureService
{
    private readonly ICultureRepository _cultureRepo;
    private readonly IPlantedCultureRepository _plantedCultureRepo;
    private readonly IFarmRepository _farmRepo;
    private readonly ILogger<AgricultureService> _logger;

    public AgricultureService(
        ICultureRepository cultureRepo,
        IPlantedCultureRepository plantedCultureRepo,
        IFarmRepository farmRepo,
        ILogger<AgricultureService> logger)
    {
        _cultureRepo = cultureRepo;
        _plantedCultureRepo = plantedCultureRepo;
        _farmRepo = farmRepo;
        _logger = logger;
    }

    public async Task<CultureResponse> CreateCultureAsync(CreateCultureRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando cultura: {Name}", request.Name);

        if (await _cultureRepo.NameExistsAsync(request.Name, ct))
            throw new DomainException("Cultura já cadastrada.", "CULTURE_EXISTS");

        var culture = Culture.Create(request.Name, request.Ciclo, request.Variedade);
        await _cultureRepo.AddAsync(culture, ct);

        _logger.LogInformation("Cultura criada: {Id}", culture.Id);
        return culture.Adapt<CultureResponse>();
    }

    public async Task<IEnumerable<CultureResponse>> GetAllCulturesAsync(CancellationToken ct = default)
    {
        var cultures = await _cultureRepo.GetAllAsync(ct);
        return cultures.Adapt<IEnumerable<CultureResponse>>();
    }

    public async Task<CultureResponse> GetCultureByIdAsync(Guid id, CancellationToken ct = default)
    {
        var culture = await _cultureRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Cultura", id);
        return culture.Adapt<CultureResponse>();
    }

    public async Task SoftDeleteCultureAsync(Guid id, CancellationToken ct = default)
    {
        await _cultureRepo.SoftDeleteAsync(id, ct);
    }

    public async Task<PlantedCultureResponse> CreatePlantedCultureAsync(CreatePlantedCultureRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Plantando cultura {CultureId} na fazenda {FarmId}", request.CultureId, request.FarmId);

        var farm = await _farmRepo.GetByIdAsync(request.FarmId, ct)
            ?? throw new NotFoundException("Fazenda", request.FarmId);

        var currentPlanted = await _plantedCultureRepo.GetTotalPlantedAreaByFarmAsync(request.FarmId, ct);

        if (!farm.CanPlantArea(request.AreaPlantadaHa, currentPlanted))
            throw new DomainException(
                $"Área insuficiente. Disponível: {farm.AgriculturalAreaHa - currentPlanted:F2}ha, Solicitado: {request.AreaPlantadaHa:F2}ha.",
                "PLANTED_AREA_EXCEEDS");

        var planted = PlantedCulture.Create(
            request.FarmId, request.CultureId, request.Safra, request.AreaPlantadaHa,
            request.DataPlantio, request.DataColheitaPrevista,
            request.ProdutividadeEsperadaSacasHa, request.CustoTotal);

        await _plantedCultureRepo.AddAsync(planted, ct);

        _logger.LogInformation("Cultura plantada: {Id}", planted.Id);
        return planted.Adapt<PlantedCultureResponse>();
    }

    public async Task<IEnumerable<PlantedCultureResponse>> GetPlantedCulturesByFarmAsync(Guid farmId, CancellationToken ct = default)
    {
        var planted = await _plantedCultureRepo.GetByFarmIdAsync(farmId, ct);
        return planted.Adapt<IEnumerable<PlantedCultureResponse>>();
    }

    public async Task<PlantedCultureResponse> GetPlantedCultureByIdAsync(Guid id, CancellationToken ct = default)
    {
        var planted = await _plantedCultureRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Cultura Plantada", id);
        return planted.Adapt<PlantedCultureResponse>();
    }

    public async Task<PlantedCultureResponse> RegisterHarvestAsync(Guid id, HarvestRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Registrando colheita: {Id}", id);

        var planted = await _plantedCultureRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Cultura Plantada", id);

        planted.RegisterHarvest(request.DataColheitaReal, request.ProdutividadeObtidaSacasHa, request.ReceitaTotal);
        await _plantedCultureRepo.UpdateAsync(planted, ct);

        _logger.LogInformation("Colheita registrada. Lucro: {Lucro}", planted.CalcularLucro());
        return planted.Adapt<PlantedCultureResponse>();
    }

    public async Task SoftDeletePlantedCultureAsync(Guid id, CancellationToken ct = default)
    {
        await _plantedCultureRepo.SoftDeleteAsync(id, ct);
    }
}