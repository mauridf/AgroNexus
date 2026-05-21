using AgroNexus.Api.Endpoints;
using AgroNexus.Api.Middlewares;
using AgroNexus.Application.Auth;
using AgroNexus.Application.Interfaces.Services;
using AgroNexus.Application.Mappings;
using AgroNexus.Application.Services;
using AgroNexus.CrossCutting.Extensions;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Data;
using AgroNexus.Infrastructure.Migrations;
using AgroNexus.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. SERILOG - Logging Estruturado
// ============================================
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.FromLogContext()
          .Enrich.WithEnvironmentName()
          .Enrich.WithMachineName()
          .WriteTo.Console(outputTemplate:
              "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
});

// ============================================
// 2. CONFIGURAÇÕES (Options Pattern)
// ============================================
builder.Services.Configure<JwtTokenSettings>(
    builder.Configuration.GetSection("Jwt"));

// ============================================
// 3. BANCO DE DADOS - EF Core + PostgreSQL
// ============================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' não configurada!");

builder.Services.AddDbContext<AgroNexusDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(30);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// ============================================
// 4. AUTENTICAÇÃO JWT
// ============================================
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtTokenSettings>()
    ?? throw new InvalidOperationException("Configurações JWT não encontradas!");

if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
    throw new InvalidOperationException("JWT SecretKey não configurada!");

if (Encoding.UTF8.GetBytes(jwtSettings.SecretKey).Length < 64)
    throw new InvalidOperationException("JWT SecretKey deve ter pelo menos 512 bits (64 caracteres)!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("ADM"));

    options.AddPolicy("ProducerOnly", policy =>
        policy.RequireRole("PRD"));

    options.AddPolicy("AdminOrProducer", policy =>
        policy.RequireRole("ADM", "PRD"));
});

// ============================================
// 5. CORS
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

    options.AddPolicy("ProductionPolicy", policy =>
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:3000" })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("X-Total-Count"));
});

// ============================================
// 6. RATE LIMITING
// ============================================
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", config =>
    {
        config.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit", 100);
        config.Window = TimeSpan.FromSeconds(
            builder.Configuration.GetValue<int>("RateLimiting:WindowInSeconds", 60));
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });
});

// ============================================
// 7. INJEÇÃO DE DEPENDÊNCIA
// ============================================

// Auth
builder.Services.AddSingleton<JwtTokenService>();

// Repositórios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProducerRepository, ProducerRepository>();
builder.Services.AddScoped<IFarmRepository, FarmRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ICultureRepository, CultureRepository>();
builder.Services.AddScoped<IPlantedCultureRepository, PlantedCultureRepository>();
builder.Services.AddScoped<IInputRepository, InputRepository>();
builder.Services.AddScoped<IInputPurchaseRepository, InputPurchaseRepository>();
builder.Services.AddScoped<IInputStockRepository, InputStockRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IOperationalCostRepository, OperationalCostRepository>();
builder.Services.AddScoped<IMachineRepository, MachineRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<IClimateRepository, ClimateRepository>();
builder.Services.AddScoped<IProductionSaleRepository, ProductionSaleRepository>();

// Serviços de Aplicação
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFarmManagementService, FarmManagementService>();

// ============================================
// 7.5 MAPEAMENTOS E VALIDADORES
// ============================================
MappingConfig.Configure();        // Mapster
builder.Services.AddValidators(); // FluentValidation

// ============================================
// 8. OPEN API + SCALAR
// ============================================
builder.Services.AddOpenApi();

// ============================================
// 9. BUILD
// ============================================
var app = builder.Build();

// ============================================
// 10. EXECUTAR MIGRAÇÕES DbUp
// ============================================
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Executando migrações DbUp...");
        var migrationRunner = new DbUpMigrationRunner(connectionString,
            scope.ServiceProvider.GetRequiredService<ILogger<DbUpMigrationRunner>>());
        var result = migrationRunner.RunMigrations();

        if (!result.Successful)
        {
            logger.LogError(result.Error, "Falha nas migrações. Aplicação será encerrada.");
            throw result.Error;
        }

        logger.LogInformation("Migrações concluídas com sucesso!");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Erro fatal durante migrações");
        throw;
    }
}

// ============================================
// 11. MIDDLEWARES PIPELINE (Ordem correta!)
// ============================================

// 1. Tratamento global de exceções
app.UseGlobalExceptionHandler();

// 2. Logging de requisições
app.UseRequestLogging();

// 3. Serilog request logging
app.UseSerilogRequestLogging();

// 4. HTTPS (em produção)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// 5. CORS
app.UseCors(app.Environment.IsDevelopment() ? "DevelopmentPolicy" : "ProductionPolicy");

// 6. Rate Limiting
app.UseRateLimiter();

// 7. Autenticação & Autorização
app.UseAuthentication();
app.UseAuthorization();

// ============================================
// 12. ENDPOINTS (Modulares + Organizados)
// ============================================

// Health Check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
   .WithTags("Health")
   .AllowAnonymous();

// OpenAPI + Scalar Docs
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("AgroNexus API - Gestão Agrícola")
        .WithTheme(ScalarTheme.DeepSpace)
        .WithDarkModeToggle(true);
});

// Endpoints de Negócio (Arquivos Modulares)
app.MapAuthEndpoints();     // Registro e Login
app.MapUserEndpoints();     // Gestão de Usuários (Admin)
app.MapProducerEndpoints(); // CRUD de Produtores
app.MapFarmEndpoints();     // CRUD de Fazendas

// ============================================
// 13. START 🚀
// ============================================
app.Run();