using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;

namespace AgroNexus.Api.Endpoints;

public static class InventoryEndpoints
{
    public static void MapInventoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/inventory")
            .WithTags("Inventory")
            .RequireAuthorization();

        group.MapPost("/inputs", async (CreateInputRequest req, IInventoryService svc, CancellationToken ct) =>
        {
            var result = await svc.CreateInputAsync(req, ct);
            return Results.Created($"/api/v1/inventory/inputs/{result.Id}", result);
        }).Produces<InputResponse>(201);

        group.MapGet("/inputs", async (IInventoryService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetAllInputsAsync(ct)))
            .Produces<IEnumerable<InputResponse>>();

        group.MapPost("/purchases", async (CreateInputPurchaseRequest req, IInventoryService svc, CancellationToken ct) =>
        {
            var result = await svc.PurchaseInputAsync(req, ct);
            return Results.Created($"/api/v1/inventory/stock/{result.Id}", result);
        }).Produces<InputStockResponse>(201);

        group.MapGet("/stock/farm/{farmId:guid}", async (Guid farmId, IInventoryService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetStockByFarmAsync(farmId, ct)))
            .Produces<IEnumerable<InputStockResponse>>();
    }
}