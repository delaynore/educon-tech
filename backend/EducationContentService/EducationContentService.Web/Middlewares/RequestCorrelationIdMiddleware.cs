using Serilog.Context;

namespace EducationContentService.Web.Middlewares;

public sealed class RequestCorrelationIdMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private const string CorrelationId = "CorrelationId";

    private readonly RequestDelegate _next;

    public RequestCorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationIdValues);

        var correlationId = correlationIdValues.FirstOrDefault() ?? context.TraceIdentifier;

        using (LogContext.PushProperty(CorrelationId, correlationId))
        {
            return _next(context);
        }
    }
}

public static class RequestCorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCorrelationIdMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestCorrelationIdMiddleware>();
    }
}
