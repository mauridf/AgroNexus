using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;
using AgroNexus.Application.Services;
using AgroNexus.Domain.Interfaces.Repositories;
using AgroNexus.Infrastructure.Auth;
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

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. SERILOG - Logging Estruturado
// ============================================
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.FromLogContext()
          .Enrich.WithMachineName()
          .Enrich.WithEnvironmentName()
          .WriteTo.Console(outputTemplate:
              "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
});

// ============================================
// 2. CONFIGURAÇÕES FORTES (Options Pattern)
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
    // Não habilitar logging detalhado em produção
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

// Validações de segurança
if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
    throw new InvalidOperationException("JWT SecretKey não configurada!");

if (Encoding.UTF8.GetBytes(jwtSettings.SecretKey).Length < 64) // 512 bits = 64 bytes
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
        ClockSkew = TimeSpan.Zero // Sem tolerância
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
        config.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });
});

// ============================================
// 7. INJEÇÃO DE DEPENDÊNCIA
// ============================================

// Serviços de Infraestrutura
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
// 8. OPEN API + SCALAR
// ============================================
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "AgroNexus API",
            Version = "v1",
            Description = "Sistema de Gestão para Produtor Rural - API REST",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "AgroNexus",
                Email = "contato@agronexus.com"
            }
        };
        return Task.CompletedTask;
    });
});

// ============================================
// 9. BUILD DA APLICAÇÃO
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
// 11. MIDDLEWARES PIPELINE
// ============================================

// Logging HTTP (apenas desenvolvimento)
if (app.Environment.IsDevelopment())
{
    app.UseHttpLogging();
}

// Serilog request logging
app.UseSerilogRequestLogging();

// Exception handling
app.UseExceptionHandler("/error");

// HTTPS (em produção)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// CORS
app.UseCors(app.Environment.IsDevelopment() ? "DevelopmentPolicy" : "ProductionPolicy");

// Rate Limiting
app.UseRateLimiter();

// Autenticação & Autorização
app.UseAuthentication();
app.UseAuthorization();

// ============================================
// 12. ENDPOINTS (Minimal API)
// ============================================

// Health Check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
   .WithTags("Health")
   .AllowAnonymous();

// Scalar API Docs
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "AgroNexus API";
    options.Theme = ScalarTheme.DeepSpace;
    options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    options.ShowDownloadButton = true;
    options.HideClientButton = false;
});

// Endpoints de Autenticação
app.MapPost("/api/v1/auth/register", async (
    CreateUserRequest request,
    IUserService userService,
    CancellationToken ct) =>
{
    var result = await userService.CreateUserAsync(request, ct);
    return Results.Created($"/api/v1/users/{result.Id}", result);
})
.WithTags("Auth")
.Produces<UserResponse>(StatusCodes.Status201Created)
.Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
.AllowAnonymous();

app.MapPost("/api/v1/auth/login", async (
    LoginRequest request,
    IUserService userService,
    CancellationToken ct) =>
{
    var result = await userService.LoginAsync(request, ct);
    return Results.Ok(result);
})
.WithTags("Auth")
.Produces<LoginResponse>(StatusCodes.Status200OK)
.Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
.AllowAnonymous();

// Endpoints de Usuários (Admin)
var usersGroup = app.MapGroup("/api/v1/users")
    .WithTags("Users")
    .RequireAuthorization("AdminOnly");

usersGroup.MapGet("/", async (IUserService userService, CancellationToken ct) =>
    Results.Ok(await userService.GetAllAsync(ct)))
    .Produces<IEnumerable<UserResponse>>();

usersGroup.MapGet("/{id:guid}", async (Guid id, IUserService userService, CancellationToken ct) =>
{
    var user = await userService.GetByIdAsync(id, ct);
    return Results.Ok(user);
})
.Produces<UserResponse>()
.Produces<ErrorResponse>(StatusCodes.Status404NotFound);

usersGroup.MapDelete("/{id:guid}", async (Guid id, IUserService userService, CancellationToken ct) =>
{
    await userService.SoftDeleteAsync(id, ct);
    return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent);

// Endpoints de Produtores
var producersGroup = app.MapGroup("/api/v1/producers")
    .WithTags("Producers")
    .RequireAuthorization();

producersGroup.MapPost("/", async (
    CreateProducerRequest request,
    IFarmManagementService service,
    CancellationToken ct) =>
{
    var result = await service.CreateProducerAsync(request, ct);
    return Results.Created($"/api/v1/producers/{result.Id}", result);
})
.Produces<ProducerResponse>(StatusCodes.Status201Created)
.Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

producersGroup.MapGet("/{id:guid}", async (
    Guid id,
    IFarmManagementService service,
    CancellationToken ct) =>
{
    var result = await service.GetProducerByIdAsync(id, ct);
    return Results.Ok(result);
})
.Produces<ProducerResponse>()
.Produces<ErrorResponse>(StatusCodes.Status404NotFound);

producersGroup.MapPut("/{id:guid}", async (
    Guid id,
    UpdateProducerRequest request,
    IFarmManagementService service,
    CancellationToken ct) =>
{
    var result = await service.UpdateProducerAsync(id, request, ct);
    return Results.Ok(result);
})
.Produces<ProducerResponse>()
.Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

producersGroup.MapDelete("/{id:guid}", async (
    Guid id,
    IFarmManagementService service,
    CancellationToken ct) =>
{
    await service.SoftDeleteProducerAsync(id, ct);
    return Results.NoContent();
});

// Endpoints de Fazendas
var farmsGroup = app.MapGroup("/api/v1/farms")
    .WithTags("Farms")
    .RequireAuthorization();

farmsGroup.MapPost("/", async (
    CreateFarmRequest request,
    IFarmManagementService service,
    CancellationToken ct) =>
{
    var result = await service.CreateFarmAsync(request, ct);
    return Results.Created($"/api/v1/farms/{result.Id}", result);
})
.Produces<FarmResponse>(StatusCodes.Status201Created)
.Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

farmsGroup.MapGet("/{id:guid}", async (
    Guid id,
    IFarmManagementService service,
    CancellationToken ct) =>
{
    var result = await service.GetFarmByIdAsync(id, ct);
    return Results.Ok(result);
})
.Produces<FarmResponse>()
.Produces<ErrorResponse>(StatusCodes.Status404NotFound);

farmsGroup.MapGet("/producer/{producerId:guid}", async (
    Guid producerId,
    IFarmManagementService service,
    CancellationToken ct) =>
{
    var result = await service.GetFarmsByProducerAsync(producerId, ct);
    return Results.Ok(result);
})
.Produces<IEnumerable<FarmResponse>>();

farmsGroup.MapPut("/{id:guid}", async (
    Guid id,
    UpdateFarmRequest request,
    IFarmManagementService service,
    CancellationToken ct) =>
{
    var result = await service.UpdateFarmAsync(id, request, ct);
    return Results.Ok(result);
})
.Produces<FarmResponse>()
.Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

farmsGroup.MapDelete("/{id:guid}", async (
    Guid id,
    IFarmManagementService service,
    CancellationToken ct) =>
{
    await service.SoftDeleteFarmAsync(id, ct);
    return Results.NoContent();
});

// ============================================
// 13. START
// ============================================
app.Run();