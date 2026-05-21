using AgroNexus.Application.DTOs.Requests;
using AgroNexus.Application.DTOs.Responses;
using AgroNexus.Application.Interfaces.Services;
using AgroNexus.Domain.Entities.Farm;
using AgroNexus.Domain.Entities.Operations;
using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.Interfaces.Repositories;
using Mapster;
using Microsoft.Extensions.Logging;

namespace AgroNexus.Application.Services;

public sealed class OperationsService : IOperationsService
{
    private readonly IContractRepository _contractRepo;
    private readonly IOperationalCostRepository _costRepo;
    private readonly IMachineRepository _machineRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly ILogger<OperationsService> _logger;

    public OperationsService(
        IContractRepository contractRepo,
        IOperationalCostRepository costRepo,
        IMachineRepository machineRepo,
        IEmployeeRepository employeeRepo,
        ILogger<OperationsService> logger)
    {
        _contractRepo = contractRepo;
        _costRepo = costRepo;
        _machineRepo = machineRepo;
        _employeeRepo = employeeRepo;
        _logger = logger;
    }

    public async Task<ContractResponse> CreateContractAsync(CreateContractRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando contrato para fazenda {FarmId}", request.FarmId);
        var contract = Contract.Create(request.FarmId, request.Tipo, request.Valor, request.DataInicio, request.ParteContratante, request.DataFim);
        await _contractRepo.AddAsync(contract, ct);
        return contract.Adapt<ContractResponse>();
    }

    public async Task<IEnumerable<ContractResponse>> GetContractsByFarmAsync(Guid farmId, CancellationToken ct = default)
    {
        var contracts = await _contractRepo.GetByFarmIdAsync(farmId, ct);
        return contracts.Adapt<IEnumerable<ContractResponse>>();
    }

    public async Task<OperationalCostResponse> CreateCostAsync(CreateOperationalCostRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando custo operacional: {Desc}", request.Descricao);
        var cost = OperationalCost.Create(request.FarmId, request.Descricao, request.Valor, request.Data);
        await _costRepo.AddAsync(cost, ct);
        return cost.Adapt<OperationalCostResponse>();
    }

    public async Task<IEnumerable<OperationalCostResponse>> GetCostsByFarmAsync(Guid farmId, CancellationToken ct = default)
    {
        var costs = await _costRepo.GetByFarmIdAsync(farmId, ct);
        return costs.Adapt<IEnumerable<OperationalCostResponse>>();
    }

    public async Task<MachineResponse> CreateMachineAsync(CreateMachineRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando máquina: {Desc}", request.Descricao);
        var machine = Machine.Create(request.FarmId, request.Descricao, request.Marca, request.Modelo, request.Ano, request.ValorAproximado);
        await _machineRepo.AddAsync(machine, ct);
        return machine.Adapt<MachineResponse>();
    }

    public async Task<IEnumerable<MachineResponse>> GetMachinesByFarmAsync(Guid farmId, CancellationToken ct = default)
    {
        var machines = await _machineRepo.GetByFarmIdAsync(farmId, ct);
        return machines.Adapt<IEnumerable<MachineResponse>>();
    }

    public async Task<EmployeeResponse> CreateEmployeeAsync(CreateEmployeeRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando funcionário: {Name}", request.Name);
        var employee = Employee.Create(request.FarmId, request.Name, request.Cpf, request.Funcao, request.Salario, request.DataAdmissao);
        await _employeeRepo.AddAsync(employee, ct);
        return employee.Adapt<EmployeeResponse>();
    }

    public async Task<IEnumerable<EmployeeResponse>> GetEmployeesByFarmAsync(Guid farmId, CancellationToken ct = default)
    {
        var employees = await _employeeRepo.GetByFarmIdAsync(farmId, ct);
        return employees.Adapt<IEnumerable<EmployeeResponse>>();
    }

    public async Task<EmployeeResponse> DismissEmployeeAsync(Guid id, DismissEmployeeRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Demitindo funcionário: {Id}", id);
        var employee = await _employeeRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Funcionário", id);
        employee.Dismiss(request.DataDemissao);
        await _employeeRepo.UpdateAsync(employee, ct);
        return employee.Adapt<EmployeeResponse>();
    }
}