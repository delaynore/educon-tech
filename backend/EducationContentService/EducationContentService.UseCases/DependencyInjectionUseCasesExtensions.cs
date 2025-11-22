using System.Reflection;
using EducationContentService.UseCases.Features.Lessons;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EducationContentService.UseCases;

public static class DependencyInjectionUseCasesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<CreateHandler>();
        services.AddScoped<GetHandler>();
        services.AddScoped<SoftDeleteHandler>();
        services.AddScoped<UpdateInfoHandler>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
