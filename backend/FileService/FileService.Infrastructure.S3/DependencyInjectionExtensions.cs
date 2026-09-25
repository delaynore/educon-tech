using Amazon.S3;
using FileService.UseCases;
using FileService.UseCases.Features;
using FileService.UseCases.FilesStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.S3;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddS3(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<FileStorageOptions>>().Value;

            var config = new AmazonS3Config
            {
                ServiceURL = options.Endpoint,
                ForcePathStyle = true,
                UseHttp = !options.WithSsl,
            };

            return new AmazonS3Client(options.AccessKey, options.SecretKey, config);
        });

        services.AddScoped<IS3Provider, S3Provider>();
        services.AddTransient<IChunkSizeCalculator, ChunkSizeCalculator>();

        services.AddHostedService<S3BucketInitializationService>();

        return services;
    }
}
