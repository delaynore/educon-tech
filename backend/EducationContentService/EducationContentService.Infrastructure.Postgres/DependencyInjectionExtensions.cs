using EducationContentService.UseCases.Database;
using EducationContentService.UseCases.Features.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EducationContentService.Infrastructure.Postgres;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructurePostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
                               ?? throw new ArgumentException("connection string");
        services.AddDbContextPool<EducationDbContext>((sp, builder) =>
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

        services.AddScoped<ILessonsRepository, LessonsRepository>();
        services.AddScoped<IEducationReadDbContext, EducationDbContext>();
        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }
}
