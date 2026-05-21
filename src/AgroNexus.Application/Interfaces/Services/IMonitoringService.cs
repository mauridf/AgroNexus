using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;

namespace AgroNexus.Application.Interfaces.Services;

public interface IMonitoringService
{
    // Alerts
    Task<AlertResponse> CreateAlertAsync(CreateAlertRequest request, CancellationToken ct = default);
    Task<IEnumerable<AlertResponse>> GetAlertsByFarmAsync(Guid farmId, CancellationToken ct = default);
    Task<AlertResponse> ResolveAlertAsync(Guid id, CancellationToken ct = default);

    // Certificates
    Task<CertificateResponse> CreateCertificateAsync(CreateCertificateRequest request, CancellationToken ct = default);
    Task<IEnumerable<CertificateResponse>> GetCertificatesByFarmAsync(Guid farmId, CancellationToken ct = default);

    // Climate
    Task<ClimateResponse> CreateClimateRecordAsync(CreateClimateRequest request, CancellationToken ct = default);
    Task<IEnumerable<ClimateResponse>> GetClimateByFarmAsync(Guid farmId, CancellationToken ct = default);
}