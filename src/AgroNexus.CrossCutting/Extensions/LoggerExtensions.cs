using Microsoft.Extensions.Logging;

namespace AgroNexus.CrossCutting.Extensions;

/// <summary>
/// Extensões para logging estruturado.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Log de operação bem-sucedida.
    /// </summary>
    public static void LogOperationSuccess<T>(this ILogger<T> logger, string operation, Guid entityId)
    {
        logger.LogInformation("{Operation} realizada com sucesso. EntityId: {EntityId}", operation, entityId);
    }

    /// <summary>
    /// Log de operação com falha de validação.
    /// </summary>
    public static void LogValidationFailure<T>(this ILogger<T> logger, string entity, string errors)
    {
        logger.LogWarning("Falha na validação de {Entity}: {Errors}", entity, errors);
    }
}