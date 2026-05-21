using AgroNexus.CrossCutting.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AgroNexus.CrossCutting.Extensions;

/// <summary>
/// Extensões para configuração de injeção de dependência.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos os validadores do FluentValidation.
    /// </summary>
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateFarmRequestValidator>();

        return services;
    }
}