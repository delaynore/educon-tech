using System.Reflection;
using EducationContentService.UseCases.Features.Lessons;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EducationContentService.UseCases;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<CreateHandler>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
