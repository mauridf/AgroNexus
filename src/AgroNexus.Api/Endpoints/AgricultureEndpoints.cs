using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;

namespace AgroNexus.Api.Endpoints;

public static class AgricultureEndpoints
{
    public static void MapAgricultureEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/agriculture")
            .WithTags("Agriculture")
            .RequireAuthorization();

        // Cultures
        group.MapPost("/cultures", async (CreateCultureRequest req, IAgricultureService svc, CancellationToken ct) =>
        {
            var result = await svc.CreateCultureAsync(req, ct);
            return Results.Created($"/api/v1/agriculture/cultures/{result.Id}", result);
        }).Produces<CultureResponse>(201).Produces<ErrorResponse>(400);

        group.MapGet("/cultures", async (IAgricultureService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetAllCulturesAsync(ct)))
            .Produces<IEnumerable<CultureResponse>>();

        group.MapGet("/cultures/{id:guid}", async (Guid id, IAgricultureService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetCultureByIdAsync(id, ct)))
            .Produces<CultureResponse>().Produces<ErrorResponse>(404);

        // Planted Cultures
        group.MapPost("/planted", async (CreatePlantedCultureRequest req, IAgricultureService svc, CancellationToken ct) =>
        {
            var result = await svc.CreatePlantedCultureAsync(req, ct);
            return Results.Created($"/api/v1/agriculture/planted/{result.Id}", result);
        }).Produces<PlantedCultureResponse>(201).Produces<ErrorResponse>(400);

        group.MapGet("/planted/farm/{farmId:guid}", async (Guid farmId, IAgricultureService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetPlantedCulturesByFarmAsync(farmId, ct)))
            .Produces<IEnumerable<PlantedCultureResponse>>();

        group.MapPost("/planted/{id:guid}/harvest", async (Guid id, HarvestRequest req, IAgricultureService svc, CancellationToken ct) =>
            Results.Ok(await svc.RegisterHarvestAsync(id, req, ct)))
            .Produces<PlantedCultureResponse>().Produces<ErrorResponse>(400);
    }
}