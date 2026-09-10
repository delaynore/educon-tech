using FileService.Infrastructure.Postgres;
using FileService.Infrastructure.S3;
using FileService.UseCases;
using Framework.Endpoints;
using Framework.Logging;
using Framework.Swagger;

namespace FileService.Web.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddSerilogLogging(configuration, "File Service")
            .AddOpenApiSpec("FileService", " v1")
            .AddUseCases()
            .AddS3(configuration)
            .AddInfrastructurePostgres(configuration)
            .AddEndpoints(typeof(DependencyInjectionUseCasesExtensions).Assembly);
    }
}
