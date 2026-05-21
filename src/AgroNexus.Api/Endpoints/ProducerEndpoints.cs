using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;

namespace AgroNexus.Api.Endpoints;

/// <summary>
/// Endpoints de gerenciamento de produtores.
/// </summary>
public static class ProducerEndpoints
{
    public static void MapProducerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/producers")
            .WithTags("Producers")
            .RequireAuthorization();

        group.MapPost("/", async (
            CreateProducerRequest request,
            IFarmManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.CreateProducerAsync(request, ct);
            return Results.Created($"/api/v1/producers/{result.Id}", result);
        })
        .WithName("CreateProducer")
        .WithDescription("Cria um novo produtor rural")
        .Produces<ProducerResponse>(StatusCodes.Status201Created)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", async (
            Guid id,
            IFarmManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.GetProducerByIdAsync(id, ct);
            return Results.Ok(result);
        })
        .WithName("GetProducerById")
        .WithDescription("Obtém um produtor pelo ID")
        .Produces<ProducerResponse>()
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            IFarmManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.GetAllProducersAsync(ct);
            return Results.Ok(result);
        })
        .WithName("GetAllProducers")
        .WithDescription("Lista todos os produtores")
        .Produces<IEnumerable<ProducerResponse>>();

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateProducerRequest request,
            IFarmManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.UpdateProducerAsync(id, request, ct);
            return Results.Ok(result);
        })
        .WithName("UpdateProducer")
        .WithDescription("Atualiza dados de um produtor")
        .Produces<ProducerResponse>()
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IFarmManagementService service,
            CancellationToken ct) =>
        {
            await service.SoftDeleteProducerAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("DeleteProducer")
        .WithDescription("Desativa um produtor (soft delete)")
        .Produces(StatusCodes.Status204NoContent);
    }
}