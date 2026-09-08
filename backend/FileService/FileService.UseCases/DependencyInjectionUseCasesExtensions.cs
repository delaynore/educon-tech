using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.UseCases;

public static class DependencyInjectionUseCasesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
