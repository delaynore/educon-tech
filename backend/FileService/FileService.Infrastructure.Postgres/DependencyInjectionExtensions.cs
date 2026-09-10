using FileService.Infrastructure.Postgres.Repositories;
using FileService.UseCases;
using FileService.UseCases.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileService.Infrastructure.Postgres;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructurePostgres(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
                               ?? throw new ArgumentException("connection string");

        services.AddDbContextPool<FileServiceDbContext>((sp, builder) =>
        {
            builder.UseNpgsql(connectionString);

            var hostEnvironment = sp.GetRequiredService<IHostEnvironment>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            if (hostEnvironment.IsDevelopment())
            {
                builder.EnableSensitiveDataLogging();
                builder.EnableDetailedErrors();
            }

            builder.UseLoggerFactory(loggerFactory);
        });

        services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();
        services.AddScoped<IFileReadDbContext, FileServiceDbContext>();
        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }
}
