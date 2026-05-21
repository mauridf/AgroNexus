using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AgroNexus.CrossCutting.Extensions;

/// <summary>
/// Extensões para configuração de injeção de dependência.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos os validadores do FluentValidation no assembly do CrossCutting.
    /// </summary>
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        // Escaneia o assembly atual e registra todos os AbstractValidator<T>
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}