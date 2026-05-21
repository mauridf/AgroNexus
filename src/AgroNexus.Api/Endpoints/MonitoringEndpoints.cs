using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;

namespace AgroNexus.Api.Endpoints;

public static class MonitoringEndpoints
{
    public static void MapMonitoringEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/monitoring")
            .WithTags("Monitoring")
            .RequireAuthorization();

        group.MapPost("/alerts", async (CreateAlertRequest req, IMonitoringService svc, CancellationToken ct) =>
        {
            var r = await svc.CreateAlertAsync(req, ct);
            return Results.Created($"/api/v1/monitoring/alerts/{r.Id}", r);
        }).Produces<AlertResponse>(201);
        group.MapGet("/alerts/farm/{farmId:guid}", async (Guid farmId, IMonitoringService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetAlertsByFarmAsync(farmId, ct)));
        group.MapPatch("/alerts/{id:guid}/resolve", async (Guid id, IMonitoringService svc, CancellationToken ct) =>
            Results.Ok(await svc.ResolveAlertAsync(id, ct)));

        group.MapPost("/certificates", async (CreateCertificateRequest req, IMonitoringService svc, CancellationToken ct) =>
        {
            var r = await svc.CreateCertificateAsync(req, ct);
            return Results.Created($"/api/v1/monitoring/certificates/{r.Id}", r);
        }).Produces<CertificateResponse>(201);
        group.MapGet("/certificates/farm/{farmId:guid}", async (Guid farmId, IMonitoringService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetCertificatesByFarmAsync(farmId, ct)));

        group.MapPost("/climate", async (CreateClimateRequest req, IMonitoringService svc, CancellationToken ct) =>
        {
            var r = await svc.CreateClimateRecordAsync(req, ct);
            return Results.Created($"/api/v1/monitoring/climate/{r.Id}", r);
        }).Produces<ClimateResponse>(201);
        group.MapGet("/climate/farm/{farmId:guid}", async (Guid farmId, IMonitoringService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetClimateByFarmAsync(farmId, ct)));
    }
}