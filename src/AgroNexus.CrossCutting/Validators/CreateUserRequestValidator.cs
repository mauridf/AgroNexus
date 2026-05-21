using AgroNexus.Application.DTOs.Requests;
using FluentValidation;

namespace AgroNexus.CrossCutting.Validators;

/// <summary>
/// Validador para criação de usuário.
/// </summary>
public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email inválido.")
            .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres.")
            .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres.")
            .Matches(@"[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula.")
            .Matches(@"[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula.")
            .Matches(@"[0-9]").WithMessage("Senha deve conter pelo menos um número.")
            .Matches(@"[!@#$%^&*(),.?"":{}|<>]").WithMessage("Senha deve conter pelo menos um caractere especial.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Senhas não conferem.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Papel é obrigatório.")
            .Must(role => role?.ToUpperInvariant() == "ADM" || role?.ToUpperInvariant() == "PRD")
            .WithMessage("Papel deve ser ADM ou PRD.");
    }
}