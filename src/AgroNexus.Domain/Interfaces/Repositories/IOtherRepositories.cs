using AgroNexus.Domain.Entities.Farm;
using AgroNexus.Domain.Entities.Inventory;
using AgroNexus.Domain.Entities.Operations;
using AgroNexus.Domain.Entities.Monitoring;
using AgroNexus.Domain.Entities.Financial;

namespace AgroNexus.Domain.Interfaces.Repositories;

// Farm
public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
}

// Inventory
public interface IInputPurchaseRepository : IRepository<InputPurchase>
{
    Task<IEnumerable<InputPurchase>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
}

// Operations
public interface IContractRepository : IRepository<Contract>
{
    Task<IEnumerable<Contract>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contract>> GetActiveContractsAsync(Guid farmId, CancellationToken cancellationToken = default);
}

public interface IOperationalCostRepository : IRepository<OperationalCost>
{
    Task<IEnumerable<OperationalCost>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OperationalCost>> GetByDateRangeAsync(Guid farmId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
}

public interface IMachineRepository : IRepository<Machine>
{
    Task<IEnumerable<Machine>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
}

// Monitoring
public interface IAlertRepository : IRepository<Alert>
{
    Task<IEnumerable<Alert>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Alert>> GetUnresolvedByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
}

public interface ICertificateRepository : IRepository<Certificate>
{
    Task<IEnumerable<Certificate>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Certificate>> GetExpiringSoonAsync(int daysThreshold, CancellationToken cancellationToken = default);
}

public interface IClimateRepository : IRepository<Climate>
{
    Task<IEnumerable<Climate>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Climate>> GetByDateRangeAsync(Guid farmId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
}

// Financial
public interface IProductionSaleRepository : IRepository<ProductionSale>
{
    Task<IEnumerable<ProductionSale>> GetByPlantedCultureIdAsync(Guid plantedCultureId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductionSale>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
}