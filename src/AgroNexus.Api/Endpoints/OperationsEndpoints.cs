using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;

namespace AgroNexus.Api.Endpoints;

public static class OperationsEndpoints
{
    public static void MapOperationsEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/operations")
            .WithTags("Operations")
            .RequireAuthorization();

        // Contracts
        group.MapPost("/contracts", async (CreateContractRequest req, IOperationsService svc, CancellationToken ct) =>
        {
            var r = await svc.CreateContractAsync(req, ct);
            return Results.Created($"/api/v1/operations/contracts/{r.Id}", r);
        }).Produces<ContractResponse>(201);
        group.MapGet("/contracts/farm/{farmId:guid}", async (Guid farmId, IOperationsService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetContractsByFarmAsync(farmId, ct)));

        // Costs
        group.MapPost("/costs", async (CreateOperationalCostRequest req, IOperationsService svc, CancellationToken ct) =>
        {
            var r = await svc.CreateCostAsync(req, ct);
            return Results.Created($"/api/v1/operations/costs/{r.Id}", r);
        }).Produces<OperationalCostResponse>(201);
        group.MapGet("/costs/farm/{farmId:guid}", async (Guid farmId, IOperationsService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetCostsByFarmAsync(farmId, ct)));

        // Machines
        group.MapPost("/machines", async (CreateMachineRequest req, IOperationsService svc, CancellationToken ct) =>
        {
            var r = await svc.CreateMachineAsync(req, ct);
            return Results.Created($"/api/v1/operations/machines/{r.Id}", r);
        }).Produces<MachineResponse>(201);
        group.MapGet("/machines/farm/{farmId:guid}", async (Guid farmId, IOperationsService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetMachinesByFarmAsync(farmId, ct)));

        // Employees
        group.MapPost("/employees", async (CreateEmployeeRequest req, IOperationsService svc, CancellationToken ct) =>
        {
            var r = await svc.CreateEmployeeAsync(req, ct);
            return Results.Created($"/api/v1/operations/employees/{r.Id}", r);
        }).Produces<EmployeeResponse>(201);
        group.MapGet("/employees/farm/{farmId:guid}", async (Guid farmId, IOperationsService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetEmployeesByFarmAsync(farmId, ct)));
        group.MapPost("/employees/{id:guid}/dismiss", async (Guid id, DismissEmployeeRequest req, IOperationsService svc, CancellationToken ct) =>
            Results.Ok(await svc.DismissEmployeeAsync(id, req, ct)));
    }
}