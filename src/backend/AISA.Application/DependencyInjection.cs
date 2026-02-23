using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AISA.Application;

/// <summary>
/// Extensie pentru înregistrarea serviciilor din Application Layer în containerul DI.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR — auto-descoperă handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation — auto-descoperă validators
        services.AddValidatorsFromAssembly(assembly);

        // AutoMapper — auto-descoperă profiles
        services.AddAutoMapper(assembly);

        // Pipeline behavior pentru validare automată
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.ValidationBehavior<,>));

        return services;
    }
}
