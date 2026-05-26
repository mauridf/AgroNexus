using AgroNexus.Application.DTOs.Responses;

namespace AgroNexus.Application.Interfaces.Services;

/// <summary>
/// Serviço de dashboard com indicadores e métricas.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Dashboard principal do produtor (visão geral de todas as fazendas).
    /// </summary>
    Task<ProducerDashboardResponse> GetProducerDashboardAsync(Guid producerId, CancellationToken ct = default);

    /// <summary>
    /// Dashboard financeiro do produtor.
    /// </summary>
    Task<FinancialDashboardResponse> GetFinancialDashboardAsync(Guid producerId, CancellationToken ct = default);

    /// <summary>
    /// Dashboard detalhado de uma fazenda específica.
    /// </summary>
    Task<FarmDashboardResponse> GetFarmDashboardAsync(Guid farmId, CancellationToken ct = default);

    /// <summary>
    /// Dashboard do administrador (visão geral do sistema).
    /// </summary>
    Task<AdminDashboardResponse> GetAdminDashboardAsync(CancellationToken ct = default);
}