using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;

namespace AgroNexus.Api.Endpoints;

public static class FinancialEndpoints
{
    public static void MapFinancialEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/financial")
            .WithTags("Financial")
            .RequireAuthorization();

        group.MapPost("/sales", async (CreateProductionSaleRequest req, IFinancialService svc, CancellationToken ct) =>
        {
            var r = await svc.CreateSaleAsync(req, ct);
            return Results.Created($"/api/v1/financial/sales/{r.Id}", r);
        }).Produces<ProductionSaleResponse>(201);

        group.MapGet("/sales/farm/{farmId:guid}", async (Guid farmId, IFinancialService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetSalesByFarmAsync(farmId, ct)))
            .Produces<IEnumerable<ProductionSaleResponse>>();
    }
}