using AgroNexus.Domain.Entities;
using AgroNexus.Domain.Entities.Agriculture;
using AgroNexus.Domain.Entities.Farm;
using AgroNexus.Domain.Entities.Financial;
using AgroNexus.Domain.Entities.Identity;
using AgroNexus.Domain.Entities.Inventory;
using AgroNexus.Domain.Entities.Monitoring;
using AgroNexus.Domain.Entities.Operations;
using AgroNexus.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgroNexus.Infrastructure.Data;

public sealed class AgroNexusDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Producer> Producers => Set<Producer>();
    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Culture> Cultures => Set<Culture>();
    public DbSet<PlantedCulture> PlantedCultures => Set<PlantedCulture>();
    public DbSet<Input> Inputs => Set<Input>();
    public DbSet<InputPurchase> InputPurchases => Set<InputPurchase>();
    public DbSet<InputStock> InputStocks => Set<InputStock>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<OperationalCost> OperationalCosts => Set<OperationalCost>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Climate> Climates => Set<Climate>();
    public DbSet<ProductionSale> ProductionSales => Set<ProductionSale>();

    public AgroNexusDbContext(DbContextOptions<AgroNexusDbContext> options) : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Força TODAS as colunas para snake_case em todo o contexto
        configurationBuilder.Properties<string>().HaveColumnType("varchar");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ============================================
        // CONFIGURAÇÃO GLOBAL: snake_case para TODAS as entidades
        // ============================================
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Converte nome da tabela para snake_case
            var tableName = entity.GetTableName()!;
            entity.SetTableName(ToSnakeCase(tableName));

            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                property.SetColumnName(ToSnakeCase(columnName));
            }

            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName()!;
                key.SetName(ToSnakeCase(keyName));
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                var fkName = foreignKey.GetConstraintName()!;
                foreignKey.SetConstraintName(ToSnakeCase(fkName));
            }

            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName()!;
                index.SetDatabaseName(ToSnakeCase(indexName));
            }
        }

        // ============================================
        // CONFIGURAÇÕES ESPECÍFICAS DE VALUE OBJECTS
        // ============================================

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "identity");

            // ⚠️ Configurar o nome da coluna ANTES do HasConversion!
            entity.Property(e => e.Email)
                .HasColumnName("email")  // ← Força snake_case explícito
                .HasConversion(e => e.Value, v => Email.Create(v))
                .HasMaxLength(255).IsRequired();

            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")  // ← Redundante com o loop, mas seguro
                .HasMaxLength(255).IsRequired();

            entity.Property(e => e.Role)
                .HasColumnName("role")
                .IsRequired();

            entity.Property(e => e.LastLogin)
                .HasColumnName("last_login");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
            entity.HasIndex(e => e.Email).IsUnique().HasFilter("is_active = TRUE");
        });

        // Producer
        modelBuilder.Entity<Producer>(entity =>
        {
            entity.ToTable("producers", "farm");

            entity.Property(e => e.CpfCnpj)
                .HasColumnName("cpf_cnpj")  // ← Força snake_case explícito
                .HasConversion(c => c.Value, v => CpfCnpj.Create(v))
                .HasMaxLength(14).IsRequired();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Rg).HasColumnName("rg");
            entity.Property(e => e.InscricaoEstadual).HasColumnName("inscricao_estadual");
            entity.Property(e => e.DataNascimento).HasColumnName("data_nascimento");
            entity.Property(e => e.Telefone).HasColumnName("telefone");
            entity.Property(e => e.Endereco).HasColumnName("endereco");
            entity.Property(e => e.Cidade).HasColumnName("cidade");
            entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(2);
            entity.Property(e => e.DadosBancarios).HasColumnName("dados_bancarios");
            entity.Property(e => e.Car).HasColumnName("car");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
            entity.HasIndex(e => e.CpfCnpj).IsUnique().HasFilter("is_active = TRUE");
        });

        // Farm
        modelBuilder.Entity<Farm>(entity =>
        {
            entity.ToTable("farms", "farm");

            entity.Property(e => e.TotalArea)
                .HasColumnName("total_area_ha")
                .HasConversion(a => a.Hectares, h => Area.FromHectares(h))
                .HasPrecision(12, 4).IsRequired();

            entity.Property(e => e.AgriculturalArea)
                .HasColumnName("agricultural_area_ha")
                .HasConversion(a => a.Hectares, h => Area.FromHectares(h))
                .HasPrecision(12, 4).IsRequired();

            entity.Property(e => e.VegetationArea)
                .HasColumnName("vegetation_area_ha")
                .HasConversion(a => a.Hectares, h => Area.FromHectares(h))
                .HasPrecision(12, 4).IsRequired();

            entity.Property(e => e.BuiltArea)
                .HasColumnName("built_area_ha")
                .HasConversion(a => a.Hectares, h => Area.FromHectares(h))
                .HasPrecision(12, 4).IsRequired();

            entity.Property(e => e.ProducerId).HasColumnName("producer_id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Endereco).HasColumnName("endereco");
            entity.Property(e => e.Cidade).HasColumnName("cidade");
            entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(2);
            entity.Property(e => e.InscricaoEstadual).HasColumnName("inscricao_estadual");
            entity.Property(e => e.CodigoCar).HasColumnName("codigo_car");
            entity.Property(e => e.Ccir).HasColumnName("ccir");
            entity.Property(e => e.FonteAgua).HasColumnName("fonte_agua");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();

            entity.OwnsOne(e => e.Location, loc =>
            {
                loc.Property(c => c.Latitude).HasColumnName("latitude").HasPrecision(9, 6);
                loc.Property(c => c.Longitude).HasColumnName("longitude").HasPrecision(9, 6);
            });

            entity.Ignore(e => e.TotalAreaHa);
            entity.Ignore(e => e.AgriculturalAreaHa);
            entity.Ignore(e => e.VegetationAreaHa);
            entity.Ignore(e => e.BuiltAreaHa);
            entity.Ignore(e => e.Latitude);
            entity.Ignore(e => e.Longitude);

            entity.HasQueryFilter(e => e.IsActive);
        });

        // Demais entidades: só configurar HasMaxLength, Precision e HasQueryFilter
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees", "farm");
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Cpf).HasMaxLength(11);
            entity.Property(e => e.Salario).HasPrecision(12, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Culture>(entity =>
        {
            entity.ToTable("cultures", "agriculture");
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
            entity.HasIndex(e => e.Name).IsUnique().HasFilter("is_active = TRUE");
        });

        modelBuilder.Entity<PlantedCulture>(entity =>
        {
            entity.ToTable("planted_cultures", "agriculture");
            entity.Property(e => e.Safra).HasMaxLength(50).IsRequired();
            entity.Property(e => e.AreaPlantadaHa).HasPrecision(12, 4).IsRequired();
            entity.Property(e => e.CustoTotal).HasPrecision(15, 2);
            entity.Property(e => e.ReceitaTotal).HasPrecision(15, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Input>(entity =>
        {
            entity.ToTable("inputs", "inventory");
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
            entity.HasIndex(e => e.Name).IsUnique().HasFilter("is_active = TRUE");
        });

        modelBuilder.Entity<InputPurchase>(entity =>
        {
            entity.ToTable("input_purchases", "inventory");
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
            entity.Property(e => e.Quantidade).HasPrecision(12, 4).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.ToTable("contracts", "operations");
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
            entity.Property(e => e.Descricao).HasMaxLength(255).IsRequired();
            entity.Property(e => e.ValorAproximado).HasPrecision(15, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Alert>(entity =>
        {
            entity.ToTable("alerts", "monitoring");
            entity.Property(e => e.Tipo).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Nivel).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Data).IsRequired();
            entity.Property(e => e.Resolvido).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.ToTable("certificates", "monitoring");
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
            entity.Property(e => e.Data).IsRequired();
            entity.Property(e => e.Temperatura).HasPrecision(5, 2);
            entity.Property(e => e.ChuvaMm).HasPrecision(8, 2);
            entity.Property(e => e.Umidade).HasPrecision(5, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<ProductionSale>(entity =>
        {
            entity.ToTable("production_sales", "financial");
            entity.Property(e => e.QuantidadeVendida).HasPrecision(12, 4).IsRequired();
            entity.Property(e => e.PrecoUnitario).HasPrecision(15, 2).IsRequired();
            entity.Property(e => e.DataVenda).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasQueryFilter(e => e.IsActive);
            entity.Ignore(e => e.ValorTotal);
        });

        base.OnModelCreating(modelBuilder);
    }

    private static string ToSnakeCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "_" + char.ToLower(c) : char.ToLower(c).ToString()));
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.MarkAsUpdated();
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}