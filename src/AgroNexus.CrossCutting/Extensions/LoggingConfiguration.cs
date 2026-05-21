using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace AgroNexus.Infrastructure.Logging;

/// <summary>
/// Configuração centralizada de logging para o sistema.
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// Cria a configuração padrão do Serilog.
    /// </summary>
    public static LoggerConfiguration CreateDefaultConfiguration()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Debug();
    }

    /// <summary>
    /// Configuração para produção (Render).
    /// </summary>
    public static LoggerConfiguration CreateProductionConfiguration()
    {
        return CreateDefaultConfiguration()
            .MinimumLevel.Warning()
            .MinimumLevel.Override("AgroNexus", LogEventLevel.Information)
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:o} {Level:u3}] {Message:lj}{NewLine}{Exception}");
    }
}