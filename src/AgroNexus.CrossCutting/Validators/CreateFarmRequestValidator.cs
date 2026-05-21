using AgroNexus.Application.DTOs.Requests;
using FluentValidation;

namespace AgroNexus.CrossCutting.Validators;

/// <summary>
/// Validador para criação de fazenda com regras de área.
/// </summary>
public sealed class CreateFarmRequestValidator : AbstractValidator<CreateFarmRequest>
{
    public CreateFarmRequestValidator()
    {
        RuleFor(x => x.ProducerId)
            .NotEmpty().WithMessage("Produtor é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome da fazenda é obrigatório.")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.TotalAreaHa)
            .GreaterThan(0).WithMessage("Área total deve ser maior que zero.")
            .LessThanOrEqualTo(1_000_000).WithMessage("Área total não pode exceder 1 milhão de hectares.");

        RuleFor(x => x.AgriculturalAreaHa)
            .GreaterThanOrEqualTo(0).WithMessage("Área agricultável não pode ser negativa.");

        RuleFor(x => x.VegetationAreaHa)
            .GreaterThanOrEqualTo(0).WithMessage("Área de vegetação não pode ser negativa.");

        RuleFor(x => x.BuiltAreaHa)
            .GreaterThanOrEqualTo(0).WithMessage("Área construída não pode ser negativa.");

        // Regra de negócio: soma das áreas não pode exceder área total
        RuleFor(x => x)
            .Must(x => (x.AgriculturalAreaHa + x.VegetationAreaHa + x.BuiltAreaHa) <= x.TotalAreaHa)
            .WithMessage("A soma das áreas (agricultável + vegetação + construída) não pode exceder a área total.")
            .WithName("Areas");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue)
            .WithMessage("Latitude deve estar entre -90 e +90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue)
            .WithMessage("Longitude deve estar entre -180 e +180.");

        RuleFor(x => x.Estado)
            .MaximumLength(2).When(x => !string.IsNullOrEmpty(x.Estado))
            .WithMessage("Estado deve ter 2 caracteres (UF).");
    }
}