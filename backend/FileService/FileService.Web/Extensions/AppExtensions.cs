using Framework.Endpoints;
using Framework.Middlewares;
using Serilog;

namespace FileService.Web.Extensions;

public static class AppExtensions
{
    public static IApplicationBuilder Configure(this WebApplication app)
    {
        app.UseExceptionMiddleware();
        app.UseRequestCorrelationIdMiddleware();
        app.UseSerilogRequestLogging();

        app.MapOpenApi();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "File Service v1");
        });

        var apiGroup = app.MapGroup("/api").WithOpenApi();

        app.MapEndpoints(apiGroup);

        return app;
    }
}
