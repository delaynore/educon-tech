using EducationContentService.Infrastructure.Postgres;
using EducationContentService.UseCases;
using Framework.Endpoints;
using Framework.Logging;
using Framework.Swagger;

namespace EducationContentService.Web.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddUseCases()
            .AddInfrastructurePostgres(configuration)
            .AddSerilogLogging(configuration, "Education Content Service")
            .AddOpenApiSpec("EducationContentService", " v1")
            .AddEndpoints(typeof(DependencyInjectionUseCasesExtensions).Assembly);
    }
}
