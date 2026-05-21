using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;

namespace AgroNexus.Api.Endpoints;

/// <summary>
/// Endpoints de gerenciamento de usuários (apenas Admin).
/// </summary>
public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/users")
            .WithTags("Users")
            .RequireAuthorization("AdminOnly");

        group.MapGet("/", async (IUserService userService, CancellationToken ct) =>
            Results.Ok(await userService.GetAllAsync(ct)))
            .WithName("GetAllUsers")
            .WithDescription("Lista todos os usuários (apenas Admin)")
            .Produces<IEnumerable<UserResponse>>();

        group.MapGet("/{id:guid}", async (Guid id, IUserService userService, CancellationToken ct) =>
        {
            var user = await userService.GetByIdAsync(id, ct);
            return Results.Ok(user);
        })
        .WithName("GetUserById")
        .WithDescription("Obtém um usuário pelo ID")
        .Produces<UserResponse>()
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, IUserService userService, CancellationToken ct) =>
        {
            await userService.SoftDeleteAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("DeleteUser")
        .WithDescription("Desativa um usuário (soft delete)")
        .Produces(StatusCodes.Status204NoContent);
    }
}