using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Domain.Entities.Agriculture;
using AgroNexus.Domain.Entities.Farm;
using AgroNexus.Domain.Entities.Financial;
using AgroNexus.Domain.Entities.Identity;
using AgroNexus.Domain.Entities.Inventory;
using AgroNexus.Domain.Entities.Monitoring;
using AgroNexus.Domain.Entities.Operations;
using Mapster;

namespace AgroNexus.Application.Mappings;

/// <summary>
/// Configuração global de mapeamentos usando Mapster.
/// Centraliza todos os mapeamentos Entity → DTO para evitar código repetitivo.
/// </summary>
public static class MappingConfig
{
    /// <summary>
    /// Registra todos os mapeamentos personalizados.
    /// Deve ser chamado uma vez na inicialização (Program.cs).
    /// </summary>
    public static void Configure()
    {
        // User → UserResponse
        TypeAdapterConfig<User, UserResponse>.NewConfig()
            .Map(dest => dest.Role, src => src.Role.ToString());

        // Producer → ProducerResponse
        TypeAdapterConfig<Producer, ProducerResponse>.NewConfig()
            .Map(dest => dest.CpfCnpj, src =>
                src.CpfCnpj.Length == 11
                    ? Convert.ToUInt64(src.CpfCnpj).ToString(@"000\.000\.000\-00")
                    : Convert.ToUInt64(src.CpfCnpj).ToString(@"00\.000\.000\/0000\-00"));

        // Farm → FarmResponse
        TypeAdapterConfig<Farm, FarmResponse>.NewConfig();

        // Culture → CultureResponse
        TypeAdapterConfig<Culture, CultureResponse>.NewConfig();

        // PlantedCulture → PlantedCultureResponse
        TypeAdapterConfig<PlantedCulture, PlantedCultureResponse>.NewConfig()
            .Map(dest => dest.CultureName, src => string.Empty) // Será preenchido no service
            .Map(dest => dest.FarmName, src => string.Empty);

        // Employee → EmployeeResponse
        TypeAdapterConfig<Employee, EmployeeResponse>.NewConfig();

        // Input → InputResponse, InputStock → InputStockResponse
        TypeAdapterConfig<Input, InputResponse>.NewConfig();
        TypeAdapterConfig<InputStock, InputStockResponse>.NewConfig()
            .Map(dest => dest.InputName, src => string.Empty);

        // Contract → ContractResponse
        TypeAdapterConfig<Contract, ContractResponse>.NewConfig();

        // OperationalCost → OperationalCostResponse
        TypeAdapterConfig<OperationalCost, OperationalCostResponse>.NewConfig();

        // Machine → MachineResponse
        TypeAdapterConfig<Machine, MachineResponse>.NewConfig();

        // Alert → AlertResponse
        TypeAdapterConfig<Alert, AlertResponse>.NewConfig();

        // Certificate → CertificateResponse
        TypeAdapterConfig<Certificate, CertificateResponse>.NewConfig();

        // Climate → ClimateResponse
        TypeAdapterConfig<Climate, ClimateResponse>.NewConfig();

        // ProductionSale → ProductionSaleResponse
        TypeAdapterConfig<ProductionSale, ProductionSaleResponse>.NewConfig()
            .Map(dest => dest.ValorTotal, src => src.ValorTotal);
    }
}