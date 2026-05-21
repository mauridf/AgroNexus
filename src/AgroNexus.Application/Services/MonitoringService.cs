using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;
using AgroNexus.Domain.Entities.Monitoring;
using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.Interfaces.Repositories;
using Mapster;
using Microsoft.Extensions.Logging;

namespace AgroNexus.Application.Services;

public sealed class MonitoringService : IMonitoringService
{
    private readonly IAlertRepository _alertRepo;
    private readonly ICertificateRepository _certRepo;
    private readonly IClimateRepository _climateRepo;
    private readonly ILogger<MonitoringService> _logger;

    public MonitoringService(
        IAlertRepository alertRepo,
        ICertificateRepository certRepo,
        IClimateRepository climateRepo,
        ILogger<MonitoringService> logger)
    {
        _alertRepo = alertRepo;
        _certRepo = certRepo;
        _climateRepo = climateRepo;
        _logger = logger;
    }

    public async Task<AlertResponse> CreateAlertAsync(CreateAlertRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando alerta: {Tipo} - {Nivel}", request.Tipo, request.Nivel);
        var alert = Alert.Create(request.FarmId, request.Tipo, request.Nivel, request.Data, request.Descricao);
        await _alertRepo.AddAsync(alert, ct);
        return alert.Adapt<AlertResponse>();
    }

    public async Task<IEnumerable<AlertResponse>> GetAlertsByFarmAsync(Guid farmId, CancellationToken ct = default)
    {
        var alerts = await _alertRepo.GetByFarmIdAsync(farmId, ct);
        return alerts.Adapt<IEnumerable<AlertResponse>>();
    }

    public async Task<AlertResponse> ResolveAlertAsync(Guid id, CancellationToken ct = default)
    {
        var alert = await _alertRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Alerta", id);
        alert.Resolve();
        await _alertRepo.UpdateAsync(alert, ct);
        return alert.Adapt<AlertResponse>();
    }

    public async Task<CertificateResponse> CreateCertificateAsync(CreateCertificateRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando certificado: {Tipo}", request.Tipo);
        var cert = Certificate.Create(request.FarmId, request.Tipo, request.DataEmissao, request.DataValidade);
        await _certRepo.AddAsync(cert, ct);
        return cert.Adapt<CertificateResponse>();
    }

    public async Task<IEnumerable<CertificateResponse>> GetCertificatesByFarmAsync(Guid farmId, CancellationToken ct = default)
    {
        var certs = await _certRepo.GetByFarmIdAsync(farmId, ct);
        return certs.Adapt<IEnumerable<CertificateResponse>>();
    }

    public async Task<ClimateResponse> CreateClimateRecordAsync(CreateClimateRequest request, CancellationToken ct = default)
    {
        var climate = Climate.Create(request.FarmId, request.Data, request.Temperatura, request.ChuvaMm, request.Umidade);
        await _climateRepo.AddAsync(climate, ct);
        return climate.Adapt<ClimateResponse>();
    }

    public async Task<IEnumerable<ClimateResponse>> GetClimateByFarmAsync(Guid farmId, CancellationToken ct = default)
    {
        var climates = await _climateRepo.GetByFarmIdAsync(farmId, ct);
        return climates.Adapt<IEnumerable<ClimateResponse>>();
    }
}