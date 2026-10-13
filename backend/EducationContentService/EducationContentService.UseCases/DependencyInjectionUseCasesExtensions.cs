using System.Reflection;
using EducationContentService.UseCases.Features.Lessons;
using FileService.Contracts;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EducationContentService.UseCases;

public static class DependencyInjectionUseCasesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CreateHandler>();
        services.AddScoped<GetHandler>();
        services.AddScoped<SoftDeleteHandler>();
        services.AddScoped<UpdateInfoHandler>();

        services.AddFileServiceHttpClient(configuration);

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
