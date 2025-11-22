using Microsoft.AspNetCore.Http;
using SharedKernel;

namespace Framework.Endpoints;

public sealed class ErrorResult : IResult
{
    private readonly Error _error;

    public ErrorResult(Error error)
    {
        _error = error;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope.Fail(_error);

        httpContext.Response.StatusCode = GetStatusCodeFromErrorType(_error.Type);

        return httpContext.Response.WriteAsJsonAsync(envelope);
    }

    private static int GetStatusCodeFromErrorType(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Authentication => StatusCodes.Status401Unauthorized,
            ErrorType.Authorization => StatusCodes.Status403Forbidden,

            // ReSharper disable once PatternIsRedundant
            ErrorType.Failure or _ => StatusCodes.Status500InternalServerError,
        };
    }
}
