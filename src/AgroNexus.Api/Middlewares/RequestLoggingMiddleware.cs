using System.Diagnostics;
using System.Security.Claims;

namespace AgroNexus.Api.Middlewares;

/// <summary>
/// Middleware para logging detalhado de requisições HTTP.
/// Registra método, path, status code, duração e usuário autenticado.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path;
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

        try
        {
            await _next(context);
            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            var elapsed = stopwatch.ElapsedMilliseconds;

            if (statusCode >= 400)
            {
                _logger.LogWarning(
                    "HTTP {Method} {Path} respondeu {StatusCode} em {Elapsed}ms [User: {UserId}]",
                    method, path, statusCode, elapsed, userId);
            }
            else
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path} respondeu {StatusCode} em {Elapsed}ms [User: {UserId}]",
                    method, path, statusCode, elapsed, userId);
            }
        }
        catch (Exception)
        {
            stopwatch.Stop();
            _logger.LogError(
                "HTTP {Method} {Path} falhou após {Elapsed}ms [User: {UserId}]",
                method, path, stopwatch.ElapsedMilliseconds, userId);
            throw; // Deixa o GlobalExceptionMiddleware capturar
        }
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}