using EducationContentService.Infrastructure.Postgres;
using EducationContentService.UseCases;
using EducationContentService.UseCases.Endpoints;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;

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
            .AddSerilogLogging(configuration)
            .AddOpenApiSpec()
            .AddEndpoints(typeof(IEndpoint).Assembly);
    }

    private static IServiceCollection AddOpenApiSpec(this IServiceCollection services)
    {
        services.AddOpenApi();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Education Content Service API",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Due Software",
                },
            });
        });

        return services;
    }

    private static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((serviceProvider, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(configuration)
                .ReadFrom.Services(serviceProvider)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("ServiceName", "LessonsService"));

        return services;
    }
}
