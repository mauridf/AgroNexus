using AgroNexus.Domain.Entities;
using AgroNexus.Domain.Entities.Agriculture;
using AgroNexus.Domain.Entities.Farm;
using AgroNexus.Domain.Entities.Financial;
using AgroNexus.Domain.Entities.Identity;
using AgroNexus.Domain.Entities.Inventory;
using AgroNexus.Domain.Entities.Monitoring;
using AgroNexus.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgroNexus.Infrastructure.Data;

/// <summary>
/// Contexto do Entity Framework Core para o banco AgroNexus.
/// Mapeia as entidades para tabelas nos esquemas correspondentes.
/// </summary>
public sealed class AgroNexusDbContext : DbContext
{
    // Identity
    public DbSet<User> Users => Set<User>();

    // Farm
    public DbSet<Producer> Producers => Set<Producer>();
    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<Employee> Employees => Set<Employee>();

    // Agriculture
    public DbSet<Culture> Cultures => Set<Culture>();
    public DbSet<PlantedCulture> PlantedCultures => Set<PlantedCulture>();

    // Inventory
    public DbSet<Input> Inputs => Set<Input>();
    public DbSet<InputPurchase> InputPurchases => Set<InputPurchase>();
    public DbSet<InputStock> InputStocks => Set<InputStock>();

    // Operations
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<OperationalCost> OperationalCosts => Set<OperationalCost>();
    public DbSet<Machine> Machines => Set<Machine>();

    // Monitoring
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Climate> Climates => Set<Climate>();

    // Financial
    public DbSet<ProductionSale> ProductionSales => Set<ProductionSale>();

    public AgroNexusDbContext(DbContextOptions<AgroNexusDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas as configurações de mapeamento
        // Como usamos DbUp para criar as tabelas, o EF Core não gerencia o schema.
        // Mas ainda configuramos o mapeamento para que as queries funcionem corretamente.

        // Helper para criar filtro de soft delete
        Expression<Func<BaseEntity, bool>> softDeleteFilter = e => e.IsActive;

        // Identity schema
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "identity");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
            entity.HasIndex(e => e.Email).IsUnique().HasFilter("is_active = TRUE");
        });

        // Farm schema
        modelBuilder.Entity<Producer>(entity =>
        {
            entity.ToTable("producers", "farm");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.CpfCnpj).HasMaxLength(14).IsRequired();
            entity.Property(e => e.Estado).HasMaxLength(2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
            entity.HasIndex(e => e.CpfCnpj).IsUnique().HasFilter("is_active = TRUE");
        });

        modelBuilder.Entity<Farm>(entity =>
        {
            entity.ToTable("farms", "farm");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.TotalAreaHa).HasPrecision(12, 4).IsRequired();
            entity.Property(e => e.AgriculturalAreaHa).HasPrecision(12, 4).IsRequired();
            entity.Property(e => e.VegetationAreaHa).HasPrecision(12, 4).IsRequired();
            entity.Property(e => e.BuiltAreaHa).HasPrecision(12, 4).IsRequired();
            entity.Property(e => e.Latitude).HasPrecision(9, 6);
            entity.Property(e => e.Longitude).HasPrecision(9, 6);
            entity.Property(e => e.Estado).HasMaxLength(2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees", "farm");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Cpf).HasMaxLength(11);
            entity.Property(e => e.Salario).HasPrecision(12, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        // Agriculture schema
        modelBuilder.Entity<Culture>(entity =>
        {
            entity.ToTable("cultures", "agriculture");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
            entity.HasIndex(e => e.Name).IsUnique().HasFilter("is_active = TRUE");
        });

        modelBuilder.Entity<PlantedCulture>(entity =>
        {
            entity.ToTable("planted_cultures", "agriculture");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Safra).HasMaxLength(50).IsRequired();
            entity.Property(e => e.AreaPlantadaHa).HasPrecision(12, 4).IsRequired();
            entity.Property(e => e.CustoTotal).HasPrecision(15, 2);
            entity.Property(e => e.ReceitaTotal).HasPrecision(15, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        // Inventory schema
        modelBuilder.Entity<Input>(entity =>
        {
            entity.ToTable("inputs", "inventory");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
            entity.HasIndex(e => e.Name).IsUnique().HasFilter("is_active = TRUE");
        });

        modelBuilder.Entity<InputPurchase>(entity =>
        {
            entity.ToTable("input_purchases", "inventory");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantidade).HasPrecision(12, 4).IsRequired();
            entity.Property(e => e.ValorTotal).HasPrecision(15, 2).IsRequired();
            entity.Property(e => e.DataCompra).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<InputStock>(entity =>
        {
            entity.ToTable("input_stocks", "inventory");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantidade).HasPrecision(12, 4).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        // Operations schema
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.ToTable("contracts", "operations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tipo).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Valor).HasPrecision(15, 2).IsRequired();
            entity.Property(e => e.DataInicio).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<OperationalCost>(entity =>
        {
            entity.ToTable("operational_costs", "operations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descricao).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Valor).HasPrecision(15, 2).IsRequired();
            entity.Property(e => e.Data).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Machine>(entity =>
        {
            entity.ToTable("machines", "operations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descricao).HasMaxLength(255).IsRequired();
            entity.Property(e => e.ValorAproximado).HasPrecision(15, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        // Monitoring schema
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.ToTable("alerts", "monitoring");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tipo).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Nivel).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Data).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.ToTable("certificates", "monitoring");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tipo).HasMaxLength(100).IsRequired();
            entity.Property(e => e.DataEmissao).IsRequired();
            entity.Property(e => e.DataValidade).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Climate>(entity =>
        {
            entity.ToTable("climates", "monitoring");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Data).IsRequired();
            entity.Property(e => e.Temperatura).HasPrecision(5, 2);
            entity.Property(e => e.ChuvaMm).HasPrecision(8, 2);
            entity.Property(e => e.Umidade).HasPrecision(5, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        // Financial schema
        modelBuilder.Entity<ProductionSale>(entity =>
        {
            entity.ToTable("production_sales", "financial");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuantidadeVendida).HasPrecision(12, 4).IsRequired();
            entity.Property(e => e.PrecoUnitario).HasPrecision(15, 2).IsRequired();
            entity.Property(e => e.DataVenda).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);

            // Valor total é calculado, não armazenado no banco
            entity.Ignore(e => e.ValorTotal);
        });

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Atualiza automaticamente UpdatedAt para entidades modificadas
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.MarkAsUpdated();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}