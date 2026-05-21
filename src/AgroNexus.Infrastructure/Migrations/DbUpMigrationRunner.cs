using DbUp;
using DbUp.Engine;
using DbUp.Engine.Output;
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
        var scriptsPath = FindScriptsPath();

        _logger.LogInformation("Procurando scripts em: {ScriptsPath}", scriptsPath);

        if (!Directory.Exists(scriptsPath))
        {
            _logger.LogError("Diretório de scripts de migração não encontrado: {ScriptsPath}", scriptsPath);
            throw new DirectoryNotFoundException($"Diretório de scripts não encontrado: {scriptsPath}");
        }

        // Lista os scripts encontrados
        var scriptFiles = Directory.GetFiles(scriptsPath, "*.sql").OrderBy(f => f);
        _logger.LogInformation("{Count} arquivo(s) SQL encontrado(s)", scriptFiles.Count());
        foreach (var file in scriptFiles)
        {
            _logger.LogInformation("  - {FileName}", Path.GetFileName(file));
        }

        // Configura o DbUp
        // DESABILITA a substituição de variáveis para evitar conflitos com $2a$ do BCrypt
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(_connectionString)
            .WithScriptsFromFileSystem(scriptsPath)
            .WithTransactionPerScript()
            .LogToNowhere()
            .LogTo(new DbUpLogger(_logger))
            .JournalToPostgresqlTable("public", "schema_versions")
            .WithVariablesDisabled() // ← ISTO resolve o erro!
            .Build();

        // Garante que o banco existe (cria se necessário)
        EnsureDatabase.For.PostgresqlDatabase(_connectionString);
        _logger.LogInformation("Banco de dados verificado/garantido");

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
            _logger.LogInformation("Migrações executadas com sucesso! Scripts executados: {Count}",
                result.Scripts.Count(s => s.SqlScriptOptions != null));
        }
        else
        {
            _logger.LogError(result.Error, "Erro ao executar migrações: {ErrorMessage}", result.Error?.Message);
        }

        return result;
    }

    /// <summary>
    /// Localiza o diretório de scripts de migração em diferentes ambientes.
    /// </summary>
    private static string FindScriptsPath()
    {
        // 1. Tenta no diretório base da aplicação (produção)
        var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations", "Scripts");
        if (Directory.Exists(basePath))
            return basePath;

        // 2. Tenta caminho relativo a partir do diretório atual
        var devPath1 = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "scripts", "migrations");
        if (Directory.Exists(devPath1))
            return devPath1;

        // 3. Procura recursivamente pela pasta scripts/migrations
        var currentDir = Directory.GetCurrentDirectory();
        while (currentDir != null)
        {
            var candidatePath = Path.Combine(currentDir, "scripts", "migrations");
            if (Directory.Exists(candidatePath))
                return candidatePath;

            currentDir = Directory.GetParent(currentDir)?.FullName;
        }

        return basePath;
    }
}

/// <summary>
/// Adaptador para integrar o logging do DbUp com o ILogger do .NET.
/// </summary>
internal sealed class DbUpLogger : IUpgradeLog
{
    private readonly ILogger _logger;

    public DbUpLogger(ILogger logger)
    {
        _logger = logger;
    }

    public void LogTrace(string format, params object[] args) => _logger.LogTrace(format, args);
    public void LogDebug(string format, params object[] args) => _logger.LogDebug(format, args);
    public void LogInformation(string format, params object[] args) => _logger.LogInformation(format, args);
    public void LogWarning(string format, params object[] args) => _logger.LogWarning(format, args);
    public void LogError(string format, params object[] args) => _logger.LogError(format, args);
    public void LogError(Exception ex, string format, params object[] args) => _logger.LogError(ex, format, args);

    // Compatibilidade com versões anteriores
    public void WriteInformation(string format, params object[] args) => LogInformation(format, args);
    public void WriteError(string format, params object[] args) => LogError(format, args);
    public void WriteWarning(string format, params object[] args) => LogWarning(format, args);
}