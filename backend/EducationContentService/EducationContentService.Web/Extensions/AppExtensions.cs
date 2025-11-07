using EducationContentService.Web.Middlewares;
using Serilog;

namespace EducationContentService.Web.Extensions;

public static class AppExtensions
{
    public static IApplicationBuilder Configure(this WebApplication app)
    {
        app.UseRequestCorrelationIdMiddleware();
        app.UseSerilogRequestLogging();

        app.MapOpenApi();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Education Content Service v1");
        });

        var apiGroup = app.MapGroup("/api").WithOpenApi();

        app.MapEndpoints(apiGroup);

        return app;
    }
}
