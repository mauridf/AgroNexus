namespace AgroNexus.Domain.Exceptions;

/// <summary>
/// Exceção base para erros de domínio.
/// Deve ser usada quando uma regra de negócio é violada.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Código de erro opcional para identificação do tipo de violação.
    /// </summary>
    public string? ErrorCode { get; }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exceção lançada quando uma entidade não é encontrada.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, Guid id)
        : base($"{entityName} com ID '{id}' não foi encontrado(a).", "NOT_FOUND")
    {
    }

    public NotFoundException(string message) : base(message, "NOT_FOUND")
    {
    }
}

/// <summary>
/// Exceção lançada quando há violação de validação.
/// </summary>
public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Uma ou mais validações falharam.", "VALIDATION_ERROR")
    {
        Errors = errors;
    }
}

/// <summary>
/// Exceção lançada quando um usuário não tem permissão para realizar uma ação.
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "Você não tem permissão para realizar esta ação.")
        : base(message, "FORBIDDEN")
    {
    }
}