using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;

namespace AgroNexus.Api.Endpoints;

/// <summary>
/// Endpoints de dashboard com indicadores para produtores e administradores.
/// </summary>
public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/dashboard")
            .WithTags("Dashboard")
            .RequireAuthorization();

        // ============================================
        // DASHBOARD DO PRODUTOR
        // ============================================
        group.MapGet("/producer/{producerId:guid}", async (
            Guid producerId,
            IDashboardService service,
            CancellationToken ct) =>
        {
            var result = await service.GetProducerDashboardAsync(producerId, ct);
            return Results.Ok(result);
        })
        .WithName("GetProducerDashboard")
        .WithDescription("Dashboard principal do produtor com visão geral de todas as fazendas")
        .Produces<ProducerDashboardResponse>(StatusCodes.Status200OK);

        // ============================================
        // DASHBOARD FINANCEIRO
        // ============================================
        group.MapGet("/financial/{producerId:guid}", async (
            Guid producerId,
            IDashboardService service,
            CancellationToken ct) =>
        {
            var result = await service.GetFinancialDashboardAsync(producerId, ct);
            return Results.Ok(result);
        })
        .WithName("GetFinancialDashboard")
        .WithDescription("Dashboard financeiro com receitas, custos e lucratividade")
        .Produces<FinancialDashboardResponse>(StatusCodes.Status200OK);

        // ============================================
        // DASHBOARD DA FAZENDA
        // ============================================
        group.MapGet("/farm/{farmId:guid}", async (
            Guid farmId,
            IDashboardService service,
            CancellationToken ct) =>
        {
            var result = await service.GetFarmDashboardAsync(farmId, ct);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetFarmDashboard")
        .WithDescription("Dashboard detalhado de uma fazenda específica")
        .Produces<FarmDashboardResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // ============================================
        // DASHBOARD DO ADMINISTRADOR
        // ============================================
        group.MapGet("/admin", async (
            IDashboardService service,
            CancellationToken ct) =>
        {
            var result = await service.GetAdminDashboardAsync(ct);
            return Results.Ok(result);
        })
        .WithName("GetAdminDashboard")
        .WithDescription("Dashboard do administrador com visão geral do sistema (apenas Admin)")
        .RequireAuthorization("AdminOnly")
        .Produces<AdminDashboardResponse>(StatusCodes.Status200OK);

        // ============================================
        // DASHBOARD RESUMIDO (QUICK STATS)
        // ============================================
        group.MapGet("/quick/{producerId:guid}", async (
            Guid producerId,
            IDashboardService service,
            CancellationToken ct) =>
        {
            var dashboard = await service.GetProducerDashboardAsync(producerId, ct);
            // Retorna apenas os cards principais para carregamento rápido
            return Results.Ok(new
            {
                dashboard.TotalFarms,
                dashboard.TotalAreaHa,
                dashboard.AreaUtilizationPercentage,
                dashboard.UnresolvedAlerts,
                dashboard.ExpiringCertificates,
                dashboard.TotalActiveEmployees
            });
        })
        .WithName("GetQuickDashboard")
        .WithDescription("Dashboard rápido com indicadores principais")
        .Produces(StatusCodes.Status200OK);
    }
}