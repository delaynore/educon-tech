using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using SharedKernel;

namespace FileService.UseCases.HttpCommunication;

public static class HttpResponseMessageExtensions
{
    extension(HttpResponseMessage response)
    {
        public async Task<Result<TResponse, Error>> HandleResponseAsync<TResponse>(CancellationToken cancellationToken)
            where TResponse : class
        {
            var contentJson = await response.Content
                .ReadFromJsonAsync<Envelope<TResponse>>(cancellationToken);

            if (!response.IsSuccessStatusCode || contentJson is null)
            {
                return contentJson?.Error
                       ?? Error.Failure("http.error", "Error while reading response");
            }

            if (contentJson.Error is not null)
            {
                return contentJson.Error;
            }

            if (contentJson.Result is null)
            {
                return Error.Failure("http.no-result", "No result");
            }

            return contentJson.Result;
        }

        public async Task<UnitResult<Error>> HandleResponseAsync(CancellationToken cancellationToken)
        {
            var contentJson = await response.Content
                .ReadFromJsonAsync<Envelope>(cancellationToken);

            if (!response.IsSuccessStatusCode || contentJson is null)
            {
                return contentJson?.Error
                       ?? Error.Failure("http.error", "Error while reading response");
            }

            return UnitResult.FailureIf(
                contentJson.IsError,
                contentJson.Error ?? Error.Failure("http.no-error", "No error when result is possibly success"));
        }
    }
}
