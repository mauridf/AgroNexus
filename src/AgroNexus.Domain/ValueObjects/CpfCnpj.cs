using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um CPF (11 dígitos) ou CNPJ (14 dígitos).
/// É imutável e contém lógica de validação.
/// </summary>
public sealed record CpfCnpj
{
    /// <summary>
    /// Valor limpo (apenas números).
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Indica se é Pessoa Física (CPF).
    /// </summary>
    public bool IsCpf => Value.Length == 11;

    /// <summary>
    /// Indica se é Pessoa Jurídica (CNPJ).
    /// </summary>
    public bool IsCnpj => Value.Length == 14;

    private CpfCnpj(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Cria um CPF/CNPJ a partir de uma string (com ou sem máscara).
    /// </summary>
    public static CpfCnpj Create(string cpfCnpj)
    {
        if (string.IsNullOrWhiteSpace(cpfCnpj))
            throw new DomainException("CPF/CNPJ é obrigatório.", "CPFCNPJ_REQUIRED");

        // Remove máscara
        var apenasNumeros = new string(cpfCnpj.Where(char.IsDigit).ToArray());

        if (apenasNumeros.Length != 11 && apenasNumeros.Length != 14)
            throw new DomainException(
                $"CPF/CNPJ deve ter 11 ou 14 dígitos. Fornecido: {apenasNumeros.Length} dígitos.",
                "CPFCNPJ_INVALID_LENGTH");

        // Validação básica de CPF (números repetidos)
        if (apenasNumeros.Length == 11 && !ValidarCpf(apenasNumeros))
            throw new DomainException("CPF inválido.", "CPF_INVALID");

        // Validação básica de CNPJ
        if (apenasNumeros.Length == 14 && !ValidarCnpj(apenasNumeros))
            throw new DomainException("CNPJ inválido.", "CNPJ_INVALID");

        return new CpfCnpj(apenasNumeros);
    }

    /// <summary>
    /// Retorna o valor formatado com máscara.
    /// </summary>
    public string Formatted => IsCpf
        ? Convert.ToUInt64(Value).ToString(@"000\.000\.000\-00")
        : Convert.ToUInt64(Value).ToString(@"00\.000\.000\/0000\-00");

    public override string ToString() => Value;

    #region Validação

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.Length != 11) return false;

        // Verifica se todos os dígitos são iguais
        if (cpf.Distinct().Count() == 1) return false;

        // Validação dos dígitos verificadores
        var multiplicador1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCpf = cpf[..9];
        var soma = tempCpf.Select((t, i) => int.Parse(t.ToString()) * multiplicador1[i]).Sum();
        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        tempCpf += digito1;
        soma = tempCpf.Select((t, i) => int.Parse(t.ToString()) * multiplicador2[i]).Sum();
        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return cpf.EndsWith(digito1.ToString() + digito2);
    }

    private static bool ValidarCnpj(string cnpj)
    {
        if (cnpj.Length != 14) return false;

        // Verifica se todos os dígitos são iguais
        if (cnpj.Distinct().Count() == 1) return false;

        // Validação dos dígitos verificadores
        var multiplicador1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCnpj = cnpj[..12];
        var soma = tempCnpj.Select((t, i) => int.Parse(t.ToString()) * multiplicador1[i]).Sum();
        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        tempCnpj += digito1;
        soma = tempCnpj.Select((t, i) => int.Parse(t.ToString()) * multiplicador2[i]).Sum();
        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return cnpj.EndsWith(digito1.ToString() + digito2);
    }

    #endregion
}