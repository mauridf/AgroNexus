using System.Reflection;
using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Logging;

namespace AgroNexus.Infrastructure.Migrations;

/// <summary>
/// Executor de migrações usando DbUp.
/// Lê scripts SQL da pasta scripts/migrations e os executa em ordem.
/// Garante que os scripts sejam executados apenas uma vez (idempotente).
/// </summary>
public sealed class DbUpMigrationRunner
{
    private readonly string _connectionString;
    private readonly ILogger<DbUpMigrationRunner> _logger;

    public DbUpMigrationRunner(string connectionString, ILogger<DbUpMigrationRunner> logger)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executa todas as migrações pendentes.
    /// </summary>
    /// <returns>Resultado da execução das migrações</returns>
    public DatabaseUpgradeResult RunMigrations()
    {
        _logger.LogInformation("Iniciando execução de migrações DbUp...");

        // Localiza o diretório dos scripts de migração
        // Em desenvolvimento: caminho relativo ao diretório do projeto
        // Em produção: os scripts são copiados para o diretório de saída
        var scriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations", "Scripts");

        // Se não encontrar no base directory, tenta caminho relativo (desenvolvimento)
        if (!Directory.Exists(scriptsPath))
        {
            scriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "scripts", "migrations");

            // Fallback: tenta a partir do diretório da solution
            if (!Directory.Exists(scriptsPath))
            {
                // Procura recursivamente pela pasta scripts/migrations
                var currentDir = Directory.GetCurrentDirectory();
                while (currentDir != null && !Directory.Exists(Path.Combine(currentDir, "scripts", "migrations")))
                {
                    currentDir = Directory.GetParent(currentDir)?.FullName;
                }

                if (currentDir != null)
                {
                    scriptsPath = Path.Combine(currentDir, "scripts", "migrations");
                }
            }
        }

        _logger.LogInformation("Procurando scripts em: {ScriptsPath}", scriptsPath);

        if (!Directory.Exists(scriptsPath))
        {
            _logger.LogError("Diretório de scripts de migração não encontrado: {ScriptsPath}", scriptsPath);
            throw new DirectoryNotFoundException($"Diretório de scripts não encontrado: {scriptsPath}");
        }

        // Configura o DbUp
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(_connectionString)
            .WithScriptsFromFileSystem(scriptsPath)
            .WithTransactionPerScript() // Cada script em sua própria transação
            .LogToConsole()
            .LogTo(new DbUpLogger(_logger))
            .Build();

        // Garante que o banco existe (cria se necessário)
        EnsureDatabase.For.PostgresqlDatabase(_connectionString);

        // Obtém scripts que serão executados
        var scriptsToExecute = upgrader.GetScriptsToExecute();
        _logger.LogInformation("{Count} script(s) pendente(s) para execução", scriptsToExecute.Count);

        foreach (var script in scriptsToExecute)
        {
            _logger.LogInformation("Script pendente: {ScriptName}", script.Name);
        }

        // Executa as migrações
        var result = upgrader.PerformUpgrade();

        if (result.Successful)
        {
            _logger.LogInformation("Migrações executadas com sucesso!");
        }
        else
        {
            _logger.LogError(result.Error, "Erro ao executar migrações");
        }

        return result;
    }
}

/// <summary>
/// Adaptador para integrar o logging do DbUp com o Serilog/ILogger do .NET.
/// </summary>
internal sealed class DbUpLogger : DbUp.Engine.Output.IUpgradeLog
{
    private readonly ILogger _logger;

    public DbUpLogger(ILogger logger)
    {
        _logger = logger;
    }

    public void WriteInformation(string format, params object[] args)
    {
        _logger.LogInformation(format, args);
    }

    public void WriteError(string format, params object[] args)
    {
        _logger.LogError(format, args);
    }

    public void WriteWarning(string format, params object[] args)
    {
        _logger.LogWarning(format, args);
    }
}