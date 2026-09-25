using System.Reflection;
using FileService.UseCases.Features;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
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

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379";
            options.InstanceName = "FileService";
        });
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions()
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(30),
            };
        });

        return services;
    }
}
