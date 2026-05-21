using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;

namespace AgroNexus.Api.Endpoints;

/// <summary>
/// Endpoints de autenticação (registro e login).
/// </summary>
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/auth")
            .WithTags("Auth")
            .AllowAnonymous();

        group.MapPost("/register", async (
            CreateUserRequest request,
            IUserService userService,
            CancellationToken ct) =>
        {
            var result = await userService.CreateUserAsync(request, ct);
            return Results.Created($"/api/v1/users/{result.Id}", result);
        })
        .WithName("RegisterUser")
        .WithDescription("Registra um novo usuário (ADM ou PRD)")
        .Produces<UserResponse>(StatusCodes.Status201Created)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/login", async (
            LoginRequest request,
            IUserService userService,
            CancellationToken ct) =>
        {
            var result = await userService.LoginAsync(request, ct);
            return Results.Ok(result);
        })
        .WithName("Login")
        .WithDescription("Autentica um usuário e retorna tokens JWT")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}