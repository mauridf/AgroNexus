using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Farm;

/// <summary>
/// Representa um funcionário vinculado a uma fazenda.
/// </summary>
public sealed class Employee : BaseEntity
{
    /// <summary>
    /// FK para a fazenda onde o funcionário trabalha.
    /// </summary>
    public Guid FarmId { get; private set; }

    /// <summary>
    /// Nome completo do funcionário.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// CPF do funcionário (apenas números).
    /// </summary>
    public string? Cpf { get; private set; }

    /// <summary>
    /// Função/cargo do funcionário na fazenda.
    /// </summary>
    public string? Funcao { get; private set; }

    /// <summary>
    /// Salário do funcionário.
    /// </summary>
    public decimal? Salario { get; private set; }

    /// <summary>
    /// Data de admissão do funcionário.
    /// </summary>
    public DateTime? DataAdmissao { get; private set; }

    /// <summary>
    /// Data de demissão do funcionário (null se ainda ativo).
    /// </summary>
    public DateTime? DataDemissao { get; private set; }

    // Navegação
    // public Farm Farm { get; private set; } = null!;

    private Employee() { }

    public static Employee Create(
        Guid farmId,
        string name,
        string? cpf = null,
        string? funcao = null,
        decimal? salario = null,
        DateTime? dataAdmissao = null)
    {
        if (farmId == Guid.Empty)
            throw new DomainException("Fazenda é obrigatória.", "EMPLOYEE_FARM_REQUIRED");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do funcionário é obrigatório.", "EMPLOYEE_NAME_REQUIRED");

        var employee = new Employee
        {
            FarmId = farmId,
            Name = name.Trim(),
            Cpf = cpf != null ? new string(cpf.Where(char.IsDigit).ToArray()) : null,
            Funcao = funcao?.Trim(),
            Salario = salario,
            DataAdmissao = dataAdmissao
        };

        return employee;
    }

    /// <summary>
    /// Registra a demissão do funcionário.
    /// </summary>
    public void Dismiss(DateTime dataDemissao)
    {
        if (DataDemissao.HasValue)
            throw new DomainException("Funcionário já foi demitido.", "EMPLOYEE_ALREADY_DISMISSED");

        if (dataDemissao < DataAdmissao)
            throw new DomainException("Data de demissão não pode ser anterior à admissão.", "EMPLOYEE_DISMISS_DATE_INVALID");

        DataDemissao = dataDemissao;
        MarkAsUpdated();
    }

    /// <summary>
    /// Verifica se o funcionário está ativo (não demitido).
    /// </summary>
    public bool IsEmployed() => !DataDemissao.HasValue;

    public void Update(
        string name,
        string? cpf,
        string? funcao,
        decimal? salario)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do funcionário é obrigatório.", "EMPLOYEE_NAME_REQUIRED");

        Name = name.Trim();
        Cpf = cpf != null ? new string(cpf.Where(char.IsDigit).ToArray()) : null;
        Funcao = funcao?.Trim();
        Salario = salario;

        MarkAsUpdated();
    }
}