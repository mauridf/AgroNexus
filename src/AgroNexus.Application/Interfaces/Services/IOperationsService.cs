using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;

namespace AgroNexus.Application.Interfaces.Services;

public interface IOperationsService
{
    // Contracts
    Task<ContractResponse> CreateContractAsync(CreateContractRequest request, CancellationToken ct = default);
    Task<IEnumerable<ContractResponse>> GetContractsByFarmAsync(Guid farmId, CancellationToken ct = default);

    // Costs
    Task<OperationalCostResponse> CreateCostAsync(CreateOperationalCostRequest request, CancellationToken ct = default);
    Task<IEnumerable<OperationalCostResponse>> GetCostsByFarmAsync(Guid farmId, CancellationToken ct = default);

    // Machines
    Task<MachineResponse> CreateMachineAsync(CreateMachineRequest request, CancellationToken ct = default);
    Task<IEnumerable<MachineResponse>> GetMachinesByFarmAsync(Guid farmId, CancellationToken ct = default);

    // Employees
    Task<EmployeeResponse> CreateEmployeeAsync(CreateEmployeeRequest request, CancellationToken ct = default);
    Task<IEnumerable<EmployeeResponse>> GetEmployeesByFarmAsync(Guid farmId, CancellationToken ct = default);
    Task<EmployeeResponse> DismissEmployeeAsync(Guid id, DismissEmployeeRequest request, CancellationToken ct = default);
}