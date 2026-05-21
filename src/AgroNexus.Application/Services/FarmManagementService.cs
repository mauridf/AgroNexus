using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;
using AgroNexus.Domain.Entities.Farm;
using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.Interfaces.Repositories;
using Mapster;
using Microsoft.Extensions.Logging;

namespace AgroNexus.Application.Services;

public sealed class FarmManagementService : IFarmManagementService
{
    private readonly IProducerRepository _producerRepository;
    private readonly IFarmRepository _farmRepository;
    private readonly ILogger<FarmManagementService> _logger;

    public FarmManagementService(
        IProducerRepository producerRepository,
        IFarmRepository farmRepository,
        ILogger<FarmManagementService> logger)
    {
        _producerRepository = producerRepository;
        _farmRepository = farmRepository;
        _logger = logger;
    }

    #region Producers

    public async Task<ProducerResponse> CreateProducerAsync(CreateProducerRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando produtor: {ProducerName}, CPF/CNPJ: {CpfCnpj}", request.Name, request.CpfCnpj);

        // Verifica se CPF/CNPJ já existe
        if (await _producerRepository.CpfCnpjExistsAsync(request.CpfCnpj, cancellationToken))
            throw new DomainException("CPF/CNPJ já cadastrado.", "PRODUCER_CPFCNPJ_EXISTS");

        var producer = Producer.Create(
            userId: request.UserId,
            name: request.Name,
            cpfCnpj: request.CpfCnpj,
            rg: request.Rg,
            inscricaoEstadual: request.InscricaoEstadual,
            dataNascimento: request.DataNascimento,
            telefone: request.Telefone,
            endereco: request.Endereco,
            cidade: request.Cidade,
            estado: request.Estado,
            dadosBancarios: request.DadosBancarios,
            car: request.Car);

        await _producerRepository.AddAsync(producer, cancellationToken);

        _logger.LogInformation("Produtor criado: {ProducerId}", producer.Id);

        return producer.Adapt<ProducerResponse>();
    }

    public async Task<ProducerResponse> GetProducerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Buscando produtor: {ProducerId}", id);

        var producer = await _producerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Produtor", id);

        return producer.Adapt<ProducerResponse>();
    }

    public async Task<IEnumerable<ProducerResponse>> GetAllProducersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listando todos os produtores");

        var producers = await _producerRepository.GetAllAsync(cancellationToken);
        return producers.Adapt<IEnumerable<ProducerResponse>>();
    }

    public async Task<ProducerResponse> UpdateProducerAsync(Guid id, UpdateProducerRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Atualizando produtor: {ProducerId}", id);

        var producer = await _producerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Produtor", id);

        producer.Update(
            name: request.Name,
            rg: request.Rg,
            inscricaoEstadual: request.InscricaoEstadual,
            dataNascimento: request.DataNascimento,
            telefone: request.Telefone,
            endereco: request.Endereco,
            cidade: request.Cidade,
            estado: request.Estado,
            dadosBancarios: request.DadosBancarios,
            car: request.Car);

        await _producerRepository.UpdateAsync(producer, cancellationToken);

        _logger.LogInformation("Produtor atualizado: {ProducerId}", id);

        return producer.Adapt<ProducerResponse>();
    }

    public async Task SoftDeleteProducerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Desativando produtor: {ProducerId}", id);
        await _producerRepository.SoftDeleteAsync(id, cancellationToken);
    }

    #endregion

    #region Farms

    public async Task<FarmResponse> CreateFarmAsync(CreateFarmRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando fazenda: {FarmName} para produtor: {ProducerId}",
            request.Name, request.ProducerId);

        // Verifica se o produtor existe
        if (!await _producerRepository.ExistsAsync(request.ProducerId, cancellationToken))
            throw new NotFoundException("Produtor", request.ProducerId);

        var farm = Farm.Create(
            producerId: request.ProducerId,
            name: request.Name,
            totalAreaHa: request.TotalAreaHa,
            agriculturalAreaHa: request.AgriculturalAreaHa,
            vegetationAreaHa: request.VegetationAreaHa,
            builtAreaHa: request.BuiltAreaHa,
            endereco: request.Endereco,
            cidade: request.Cidade,
            estado: request.Estado,
            latitude: request.Latitude,
            longitude: request.Longitude,
            inscricaoEstadual: request.InscricaoEstadual,
            codigoCar: request.CodigoCar,
            ccir: request.Ccir,
            fonteAgua: request.FonteAgua);

        await _farmRepository.AddAsync(farm, cancellationToken);

        _logger.LogInformation("Fazenda criada: {FarmId}, Área total: {TotalArea}ha",
            farm.Id, farm.TotalAreaHa);

        return farm.Adapt<FarmResponse>();
    }

    public async Task<FarmResponse> GetFarmByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Buscando fazenda: {FarmId}", id);

        var farm = await _farmRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Fazenda", id);

        return farm.Adapt<FarmResponse>();
    }

    public async Task<IEnumerable<FarmResponse>> GetFarmsByProducerAsync(Guid producerId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listando fazendas do produtor: {ProducerId}", producerId);

        var farms = await _farmRepository.GetByProducerIdAsync(producerId, cancellationToken);
        return farms.Adapt<IEnumerable<FarmResponse>>();
    }

    public async Task<FarmResponse> UpdateFarmAsync(Guid id, UpdateFarmRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Atualizando fazenda: {FarmId}", id);

        var farm = await _farmRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Fazenda", id);

        farm.Update(
            name: request.Name,
            totalAreaHa: request.TotalAreaHa,
            agriculturalAreaHa: request.AgriculturalAreaHa,
            vegetationAreaHa: request.VegetationAreaHa,
            builtAreaHa: request.BuiltAreaHa,
            endereco: request.Endereco,
            cidade: request.Cidade,
            estado: request.Estado,
            latitude: request.Latitude,
            longitude: request.Longitude,
            inscricaoEstadual: request.InscricaoEstadual,
            codigoCar: request.CodigoCar,
            ccir: request.Ccir,
            fonteAgua: request.FonteAgua);

        await _farmRepository.UpdateAsync(farm, cancellationToken);

        _logger.LogInformation("Fazenda atualizada: {FarmId}", id);

        return farm.Adapt<FarmResponse>();
    }

    public async Task SoftDeleteFarmAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Desativando fazenda: {FarmId}", id);
        await _farmRepository.SoftDeleteAsync(id, cancellationToken);
    }

    #endregion
}