using System.Net.Http.Json;
using System.Text.Json;
using CSharpFunctionalExtensions;
using SharedKernel;

namespace FileService.Contracts;

public static class HttpResponseMessageExtensions
{
    private static Error InvalidResponse => Error.Failure(
        "http.invalid-response",
        "Failed to deserialize response");

    private static Error RequestFailed(string? reasonPhrase = null) => Error.Failure(
        "http.request-failed",
        $"Request failed. Reason: {reasonPhrase ?? "Unknown"}.");

    private static Error EmptyResponse => Error.Failure(
        "http.empty-response",
        "Request succeeded but the response body was empty or could not be deserialized.");

    private static Error MissingResult => Error.Failure(
        "http.missing-result",
        "Request succeeded but the response did not contain a result.");

    extension(HttpResponseMessage response)
    {
        public async Task<Result<TResponse, Error>> HandleResponseAsync<TResponse>(CancellationToken cancellationToken)
            where TResponse : class
        {
            Envelope<TResponse>? contentJson;
            try
            {
                contentJson = await response.Content.ReadFromJsonAsync<Envelope<TResponse>>(cancellationToken);
            }
            catch (JsonException)
            {
                return InvalidResponse;
            }

            if (!response.IsSuccessStatusCode)
            {
                return contentJson?.Error ?? RequestFailed(response.ReasonPhrase);
            }

            if (contentJson is null)
            {
                return EmptyResponse;
            }

            if (contentJson.Error is not null)
            {
                return contentJson.Error;
            }

            if (contentJson.Result is null)
            {
                return MissingResult;
            }

            return contentJson.Result;
        }

        public async Task<UnitResult<Error>> HandleResponseAsync(CancellationToken cancellationToken)
        {
            Envelope? contentJson;
            try
            {
                contentJson = await response.Content.ReadFromJsonAsync<Envelope>(cancellationToken);
            }
            catch (JsonException)
            {
                return InvalidResponse;
            }

            if (!response.IsSuccessStatusCode)
            {
                return contentJson?.Error ?? RequestFailed(response.ReasonPhrase);
            }

            if (contentJson is null)
            {
                return EmptyResponse;
            }

            if (contentJson.Error is not null)
            {
                return contentJson.Error;
            }

            return UnitResult.Success<Error>();
        }
    }
}
