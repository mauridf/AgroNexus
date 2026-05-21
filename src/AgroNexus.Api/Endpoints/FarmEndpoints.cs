using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;

namespace AgroNexus.Api.Endpoints;

/// <summary>
/// Endpoints de gerenciamento de fazendas.
/// </summary>
public static class FarmEndpoints
{
    public static void MapFarmEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/farms")
            .WithTags("Farms")
            .RequireAuthorization();

        group.MapPost("/", async (
            CreateFarmRequest request,
            IFarmManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.CreateFarmAsync(request, ct);
            return Results.Created($"/api/v1/farms/{result.Id}", result);
        })
        .WithName("CreateFarm")
        .WithDescription("Cria uma nova fazenda com validação de áreas")
        .Produces<FarmResponse>(StatusCodes.Status201Created)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", async (
            Guid id,
            IFarmManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.GetFarmByIdAsync(id, ct);
            return Results.Ok(result);
        })
        .WithName("GetFarmById")
        .WithDescription("Obtém uma fazenda pelo ID")
        .Produces<FarmResponse>()
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/producer/{producerId:guid}", async (
            Guid producerId,
            IFarmManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.GetFarmsByProducerAsync(producerId, ct);
            return Results.Ok(result);
        })
        .WithName("GetFarmsByProducer")
        .WithDescription("Lista fazendas de um produtor")
        .Produces<IEnumerable<FarmResponse>>();

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateFarmRequest request,
            IFarmManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.UpdateFarmAsync(id, request, ct);
            return Results.Ok(result);
        })
        .WithName("UpdateFarm")
        .WithDescription("Atualiza dados de uma fazenda")
        .Produces<FarmResponse>()
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IFarmManagementService service,
            CancellationToken ct) =>
        {
            await service.SoftDeleteFarmAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("DeleteFarm")
        .WithDescription("Desativa uma fazenda (soft delete)")
        .Produces(StatusCodes.Status204NoContent);
    }
}