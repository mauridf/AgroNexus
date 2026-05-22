using AgroNexus.Domain.Exceptions;
using AgroNexus.Domain.ValueObjects;

namespace AgroNexus.Domain.Entities.Farm;

/// <summary>
/// Representa um produtor rural cadastrado no sistema.
/// Pode ser Pessoa Física (CPF) ou Pessoa Jurídica (CNPJ).
/// </summary>
public sealed class Producer : BaseEntity
{
    /// <summary>
    /// FK para o usuário associado a este produtor.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Nome completo ou razão social do produtor.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// CPF (11 dígitos) ou CNPJ (14 dígitos) do produtor.
    /// </summary>
    public CpfCnpj CpfCnpj { get; private set; } = null!;

    /// <summary>
    /// RG do produtor (opcional para PJ).
    /// </summary>
    public string? Rg { get; private set; }

    /// <summary>
    /// Inscrição Estadual do produtor.
    /// </summary>
    public string? InscricaoEstadual { get; private set; }

    /// <summary>
    /// Data de nascimento (PF) ou data de constituição (PJ).
    /// </summary>
    public DateTime? DataNascimento { get; private set; }

    /// <summary>
    /// Telefone principal de contato.
    /// </summary>
    public string? Telefone { get; private set; }

    /// <summary>
    /// Endereço completo do produtor.
    /// </summary>
    public string? Endereco { get; private set; }

    /// <summary>
    /// Cidade do produtor.
    /// </summary>
    public string? Cidade { get; private set; }

    /// <summary>
    /// Estado (UF) do produtor.
    /// </summary>
    public string? Estado { get; private set; }

    /// <summary>
    /// Dados bancários para transações financeiras.
    /// </summary>
    public string? DadosBancarios { get; private set; }

    /// <summary>
    /// CAR - Cadastro Ambiental Rural.
    /// </summary>
    public string? Car { get; private set; }

    // Navegação (EF Core)
    // public ICollection<Farm> Farms { get; private set; } = new List<Farm>();

    private Producer() { }

    public static Producer Create(
        Guid userId,
        string name,
        string cpfCnpj,
        string? rg = null,
        string? inscricaoEstadual = null,
        DateTime? dataNascimento = null,
        string? telefone = null,
        string? endereco = null,
        string? cidade = null,
        string? estado = null,
        string? dadosBancarios = null,
        string? car = null)
    {
        if (userId == Guid.Empty)
            throw new DomainException("Usuário é obrigatório.", "PRODUCER_USER_REQUIRED");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.", "PRODUCER_NAME_REQUIRED");

        var producer = new Producer
        {
            UserId = userId,
            Name = name.Trim(),
            CpfCnpj = CpfCnpj.Create(cpfCnpj),
            Rg = rg?.Trim(),
            InscricaoEstadual = inscricaoEstadual?.Trim(),
            DataNascimento = dataNascimento,
            Telefone = telefone?.Trim(),
            Endereco = endereco?.Trim(),
            Cidade = cidade?.Trim(),
            Estado = estado?.Trim().ToUpperInvariant(),
            DadosBancarios = dadosBancarios?.Trim(),
            Car = car?.Trim()
        };

        return producer;
    }

    /// <summary>
    /// Verifica se o CPF/CNPj é de Pessoa Física (11 dígitos).
    /// </summary>
    public bool IsPessoaFisica() => CpfCnpj.IsCpf;

    /// <summary>
    /// Verifica se o CPF/CNPj é de Pessoa Jurídica (14 dígitos).
    /// </summary>
    public bool IsPessoaJuridica() => CpfCnpj.IsCnpj;

    /// <summary>
    /// Atualiza os dados cadastrais do produtor.
    /// </summary>
    public void Update(
        string name,
        string? rg,
        string? inscricaoEstadual,
        DateTime? dataNascimento,
        string? telefone,
        string? endereco,
        string? cidade,
        string? estado,
        string? dadosBancarios,
        string? car)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.", "PRODUCER_NAME_REQUIRED");

        Name = name.Trim();
        Rg = rg?.Trim();
        InscricaoEstadual = inscricaoEstadual?.Trim();
        DataNascimento = dataNascimento;
        Telefone = telefone?.Trim();
        Endereco = endereco?.Trim();
        Cidade = cidade?.Trim();
        Estado = estado?.Trim().ToUpperInvariant();
        DadosBancarios = dadosBancarios?.Trim();
        Car = car?.Trim();

        MarkAsUpdated();
    }
}