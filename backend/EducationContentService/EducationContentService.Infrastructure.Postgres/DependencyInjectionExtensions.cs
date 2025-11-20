using EducationContentService.UseCases.Features.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EducationContentService.Infrastructure.Postgres;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructurePostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
                               ?? throw new ArgumentException("connection string");
        services.AddDbContext<EducationDbContext>(builder =>
        {
            builder.UseNpgsql(connectionString);
        });

        services.AddScoped<ILessonsRepository, LessonsRepository>();

        return services;
    }
}
