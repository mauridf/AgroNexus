using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace AgroNexus.Api.Middlewares;

/// <summary>
/// Middleware global para captura e tratamento de exceções não tratadas.
/// Garante que erros nunca exponham detalhes sensíveis em produção.
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Message = "Ocorreu um erro interno no servidor.",
            ErrorCode = "INTERNAL_ERROR"
        };

        switch (exception)
        {
            case DomainException domainEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = domainEx.Message;
                errorResponse.ErrorCode = domainEx.ErrorCode ?? "DOMAIN_ERROR";

                _logger.LogWarning("Erro de domínio: {ErrorCode} - {Message}",
                    domainEx.ErrorCode, domainEx.Message);
                break;

            case ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = validationEx.Message;
                errorResponse.ErrorCode = validationEx.ErrorCode;
                errorResponse.Errors = validationEx.Errors;
                break;

            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = notFoundEx.Message;
                errorResponse.ErrorCode = notFoundEx.ErrorCode;

                _logger.LogWarning("Recurso não encontrado: {Message}", notFoundEx.Message);
                break;

            case ForbiddenException forbiddenEx:
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                errorResponse.Message = forbiddenEx.Message;
                errorResponse.ErrorCode = forbiddenEx.ErrorCode;

                _logger.LogWarning("Acesso negado: {Message}", forbiddenEx.Message);
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Você não está autenticado.";
                errorResponse.ErrorCode = "UNAUTHORIZED";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Em desenvolvimento, podemos expor detalhes do erro
                if (_environment.IsDevelopment())
                {
                    errorResponse.Message = exception.Message;
                    errorResponse.ErrorCode = exception.GetType().Name;
                }

                _logger.LogError(exception, "Erro interno não esperado");
                break;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await response.WriteAsync(json);
    }
}

/// <summary>
/// Extensão para registrar o middleware de exceção global.
/// </summary>
public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}