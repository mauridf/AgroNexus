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

/// <summary>
/// Contexto do Entity Framework Core para o banco AgroNexus.
/// Mapeia as entidades para tabelas nos esquemas correspondentes.
/// Todas as colunas são mapeadas explicitamente em snake_case para
/// compatibilidade com as tabelas criadas pelo DbUp.
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
        // Helper para criar filtro de soft delete
        Expression<Func<BaseEntity, bool>> softDeleteFilter = e => e.IsActive;

        // ============================================
        // IDENTITY SCHEMA
        // ============================================
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "identity");
            entity.HasKey(e => e.Id).HasName("pk_users");

            // Email é ValueObject - mapeia para coluna email
            entity.Property(e => e.Email)
                .HasConversion(e => e.Value, v => Email.Create(v))
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(255)
                .IsRequired();

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

        // ============================================
        // FARM SCHEMA
        // ============================================
        modelBuilder.Entity<Producer>(entity =>
        {
            entity.ToTable("producers", "farm");
            entity.HasKey(e => e.Id).HasName("pk_producers");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            // CpfCnpj é ValueObject
            entity.Property(e => e.CpfCnpj)
                .HasConversion(c => c.Value, v => CpfCnpj.Create(v))
                .HasColumnName("cpf_cnpj")
                .HasMaxLength(14)
                .IsRequired();

            entity.Property(e => e.Rg)
                .HasColumnName("rg");

            entity.Property(e => e.InscricaoEstadual)
                .HasColumnName("inscricao_estadual");

            entity.Property(e => e.DataNascimento)
                .HasColumnName("data_nascimento");

            entity.Property(e => e.Telefone)
                .HasColumnName("telefone");

            entity.Property(e => e.Endereco)
                .HasColumnName("endereco");

            entity.Property(e => e.Cidade)
                .HasColumnName("cidade");

            entity.Property(e => e.Estado)
                .HasColumnName("estado")
                .HasMaxLength(2);

            entity.Property(e => e.DadosBancarios)
                .HasColumnName("dados_bancarios");

            entity.Property(e => e.Car)
                .HasColumnName("car");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
            entity.HasIndex(e => e.CpfCnpj).IsUnique().HasFilter("is_active = TRUE");
        });

        modelBuilder.Entity<Farm>(entity =>
        {
            entity.ToTable("farms", "farm");
            entity.HasKey(e => e.Id).HasName("pk_farms");

            entity.Property(e => e.ProducerId)
                .HasColumnName("producer_id");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            // Areas são ValueObjects mapeados para colunas snake_case
            entity.Property(e => e.TotalArea)
                .HasConversion(a => a.Hectares, h => Area.FromHectares(h))
                .HasColumnName("total_area_ha")
                .HasPrecision(12, 4)
                .IsRequired();

            entity.Property(e => e.AgriculturalArea)
                .HasConversion(a => a.Hectares, h => Area.FromHectares(h))
                .HasColumnName("agricultural_area_ha")
                .HasPrecision(12, 4)
                .IsRequired();

            entity.Property(e => e.VegetationArea)
                .HasConversion(a => a.Hectares, h => Area.FromHectares(h))
                .HasColumnName("vegetation_area_ha")
                .HasPrecision(12, 4)
                .IsRequired();

            entity.Property(e => e.BuiltArea)
                .HasConversion(a => a.Hectares, h => Area.FromHectares(h))
                .HasColumnName("built_area_ha")
                .HasPrecision(12, 4)
                .IsRequired();

            entity.Property(e => e.Endereco)
                .HasColumnName("endereco");

            entity.Property(e => e.Cidade)
                .HasColumnName("cidade");

            entity.Property(e => e.Estado)
                .HasColumnName("estado")
                .HasMaxLength(2);

            entity.Property(e => e.InscricaoEstadual)
                .HasColumnName("inscricao_estadual");

            entity.Property(e => e.CodigoCar)
                .HasColumnName("codigo_car");

            entity.Property(e => e.Ccir)
                .HasColumnName("ccir");

            entity.Property(e => e.FonteAgua)
                .HasColumnName("fonte_agua");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            // Coordinate é ValueObject (opcional) - owned type
            entity.OwnsOne(e => e.Location, loc =>
            {
                loc.Property(c => c.Latitude).HasColumnName("latitude").HasPrecision(9, 6);
                loc.Property(c => c.Longitude).HasColumnName("longitude").HasPrecision(9, 6);
            });

            // Propriedades de compatibilidade (não mapeadas no banco)
            entity.Ignore(e => e.TotalAreaHa);
            entity.Ignore(e => e.AgriculturalAreaHa);
            entity.Ignore(e => e.VegetationAreaHa);
            entity.Ignore(e => e.BuiltAreaHa);
            entity.Ignore(e => e.Latitude);
            entity.Ignore(e => e.Longitude);

            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees", "farm");
            entity.HasKey(e => e.Id).HasName("pk_employees");

            entity.Property(e => e.FarmId)
                .HasColumnName("farm_id");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Cpf)
                .HasColumnName("cpf")
                .HasMaxLength(11);

            entity.Property(e => e.Funcao)
                .HasColumnName("funcao");

            entity.Property(e => e.Salario)
                .HasColumnName("salario")
                .HasPrecision(12, 2);

            entity.Property(e => e.DataAdmissao)
                .HasColumnName("data_admissao");

            entity.Property(e => e.DataDemissao)
                .HasColumnName("data_demissao");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
        });

        // ============================================
        // AGRICULTURE SCHEMA
        // ============================================
        modelBuilder.Entity<Culture>(entity =>
        {
            entity.ToTable("cultures", "agriculture");
            entity.HasKey(e => e.Id).HasName("pk_cultures");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Ciclo)
                .HasColumnName("ciclo");

            entity.Property(e => e.Variedade)
                .HasColumnName("variedade");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
            entity.HasIndex(e => e.Name).IsUnique().HasFilter("is_active = TRUE");
        });

        modelBuilder.Entity<PlantedCulture>(entity =>
        {
            entity.ToTable("planted_cultures", "agriculture");
            entity.HasKey(e => e.Id).HasName("pk_planted_cultures");

            entity.Property(e => e.FarmId)
                .HasColumnName("farm_id");

            entity.Property(e => e.CultureId)
                .HasColumnName("culture_id");

            entity.Property(e => e.Safra)
                .HasColumnName("safra")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.AreaPlantadaHa)
                .HasColumnName("area_plantada_ha")
                .HasPrecision(12, 4)
                .IsRequired();

            entity.Property(e => e.DataPlantio)
                .HasColumnName("data_plantio");

            entity.Property(e => e.DataColheitaPrevista)
                .HasColumnName("data_colheita_prevista");

            entity.Property(e => e.DataColheitaReal)
                .HasColumnName("data_colheita_real");

            entity.Property(e => e.ProdutividadeEsperadaSacasHa)
                .HasColumnName("produtividade_esperada_sacas_ha")
                .HasPrecision(12, 4);

            entity.Property(e => e.ProdutividadeObtidaSacasHa)
                .HasColumnName("produtividade_obtida_sacas_ha")
                .HasPrecision(12, 4);

            entity.Property(e => e.CustoTotal)
                .HasColumnName("custo_total")
                .HasPrecision(15, 2);

            entity.Property(e => e.ReceitaTotal)
                .HasColumnName("receita_total")
                .HasPrecision(15, 2);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
        });

        // ============================================
        // INVENTORY SCHEMA
        // ============================================
        modelBuilder.Entity<Input>(entity =>
        {
            entity.ToTable("inputs", "inventory");
            entity.HasKey(e => e.Id).HasName("pk_inputs");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.Tipo)
                .HasColumnName("tipo");

            entity.Property(e => e.UnidadeMedida)
                .HasColumnName("unidade_medida");

            entity.Property(e => e.Fornecedor)
                .HasColumnName("fornecedor");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
            entity.HasIndex(e => e.Name).IsUnique().HasFilter("is_active = TRUE");
        });

        modelBuilder.Entity<InputPurchase>(entity =>
        {
            entity.ToTable("input_purchases", "inventory");
            entity.HasKey(e => e.Id).HasName("pk_input_purchases");

            entity.Property(e => e.FarmId)
                .HasColumnName("farm_id");

            entity.Property(e => e.InputId)
                .HasColumnName("input_id");

            entity.Property(e => e.Quantidade)
                .HasColumnName("quantidade")
                .HasPrecision(12, 4)
                .IsRequired();

            entity.Property(e => e.ValorTotal)
                .HasColumnName("valor_total")
                .HasPrecision(15, 2)
                .IsRequired();

            entity.Property(e => e.DataCompra)
                .HasColumnName("data_compra")
                .IsRequired();

            entity.Property(e => e.Fornecedor)
                .HasColumnName("fornecedor");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<InputStock>(entity =>
        {
            entity.ToTable("input_stocks", "inventory");
            entity.HasKey(e => e.Id).HasName("pk_input_stocks");

            entity.Property(e => e.FarmId)
                .HasColumnName("farm_id");

            entity.Property(e => e.InputId)
                .HasColumnName("input_id");

            entity.Property(e => e.Quantidade)
                .HasColumnName("quantidade")
                .HasPrecision(12, 4)
                .IsRequired();

            entity.Property(e => e.Validade)
                .HasColumnName("validade");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
        });

        // ============================================
        // OPERATIONS SCHEMA
        // ============================================
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.ToTable("contracts", "operations");
            entity.HasKey(e => e.Id).HasName("pk_contracts");

            entity.Property(e => e.FarmId)
                .HasColumnName("farm_id");

            entity.Property(e => e.Tipo)
                .HasColumnName("tipo")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.ParteContratante)
                .HasColumnName("parte_contratante");

            entity.Property(e => e.Valor)
                .HasColumnName("valor")
                .HasPrecision(15, 2)
                .IsRequired();

            entity.Property(e => e.DataInicio)
                .HasColumnName("data_inicio")
                .IsRequired();

            entity.Property(e => e.DataFim)
                .HasColumnName("data_fim");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<OperationalCost>(entity =>
        {
            entity.ToTable("operational_costs", "operations");
            entity.HasKey(e => e.Id).HasName("pk_operational_costs");

            entity.Property(e => e.FarmId)
                .HasColumnName("farm_id");

            entity.Property(e => e.Descricao)
                .HasColumnName("descricao")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Valor)
                .HasColumnName("valor")
                .HasPrecision(15, 2)
                .IsRequired();

            entity.Property(e => e.Data)
                .HasColumnName("data")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Machine>(entity =>
        {
            entity.ToTable("machines", "operations");
            entity.HasKey(e => e.Id).HasName("pk_machines");

            entity.Property(e => e.FarmId)
                .HasColumnName("farm_id");

            entity.Property(e => e.Descricao)
                .HasColumnName("descricao")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Marca)
                .HasColumnName("marca");

            entity.Property(e => e.Modelo)
                .HasColumnName("modelo");

            entity.Property(e => e.Ano)
                .HasColumnName("ano");

            entity.Property(e => e.ValorAproximado)
                .HasColumnName("valor_aproximado")
                .HasPrecision(15, 2);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
        });

        // ============================================
        // MONITORING SCHEMA
        // ============================================
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.ToTable("alerts", "monitoring");
            entity.HasKey(e => e.Id).HasName("pk_alerts");

            entity.Property(e => e.FarmId)
                .HasColumnName("farm_id");

            entity.Property(e => e.Tipo)
                .HasColumnName("tipo")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Descricao)
                .HasColumnName("descricao");

            entity.Property(e => e.Nivel)
                .HasColumnName("nivel")
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(e => e.Data)
                .HasColumnName("data")
                .IsRequired();

            entity.Property(e => e.Resolvido)
                .HasColumnName("resolvido")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.ToTable("certificates", "monitoring");
            entity.HasKey(e => e.Id).HasName("pk_certificates");

            entity.Property(e => e.FarmId)
                .HasColumnName("farm_id");

            entity.Property(e => e.Tipo)
                .HasColumnName("tipo")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.DataEmissao)
                .HasColumnName("data_emissao")
                .IsRequired();

            entity.Property(e => e.DataValidade)
                .HasColumnName("data_validade")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
        });

        modelBuilder.Entity<Climate>(entity =>
        {
            entity.ToTable("climates", "monitoring");
            entity.HasKey(e => e.Id).HasName("pk_climates");

            entity.Property(e => e.FarmId)
                .HasColumnName("farm_id");

            entity.Property(e => e.Data)
                .HasColumnName("data")
                .IsRequired();

            entity.Property(e => e.Temperatura)
                .HasColumnName("temperatura")
                .HasPrecision(5, 2);

            entity.Property(e => e.ChuvaMm)
                .HasColumnName("chuva_mm")
                .HasPrecision(8, 2);

            entity.Property(e => e.Umidade)
                .HasColumnName("umidade")
                .HasPrecision(5, 2);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.HasQueryFilter(e => e.IsActive);
        });

        // ============================================
        // FINANCIAL SCHEMA
        // ============================================
        modelBuilder.Entity<ProductionSale>(entity =>
        {
            entity.ToTable("production_sales", "financial");
            entity.HasKey(e => e.Id).HasName("pk_production_sales");

            entity.Property(e => e.PlantedCultureId)
                .HasColumnName("planted_culture_id");

            entity.Property(e => e.QuantidadeVendida)
                .HasColumnName("quantidade_vendida")
                .HasPrecision(12, 4)
                .IsRequired();

            entity.Property(e => e.PrecoUnitario)
                .HasColumnName("preco_unitario")
                .HasPrecision(15, 2)
                .IsRequired();

            entity.Property(e => e.DataVenda)
                .HasColumnName("data_venda")
                .IsRequired();

            entity.Property(e => e.Destino)
                .HasColumnName("destino");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

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