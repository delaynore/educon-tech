using System.Reflection;
using FileService.UseCases.Features;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.UseCases;

public static class DependencyInjectionUseCasesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<StartMultipartUploadHandler>();
        services.AddScoped<CompleteMultipartUploadHandler>();
        services.AddScoped<GetMediaAssetInfoHandler>();
        services.AddScoped<GetMediaAssetsHandler>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
