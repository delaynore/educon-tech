using Microsoft.Extensions.DependencyInjection;

namespace Framework.Swagger;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiSpec(this IServiceCollection services, string title, string version)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title = title;
                document.Info.Version = version;

                return Task.CompletedTask;
            });
        });

        // services.AddSwaggerGen(options =>
        // {
        //     options.SwaggerDoc(version, new OpenApiInfo
        //     {
        //         Title = title,
        //         Version = version,
        //     });
        // });

        return services;
    }
}
