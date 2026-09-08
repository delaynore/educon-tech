using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FileService.UseCases.Features;

public sealed class GetDownloadUrl : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/files/url", async Task<IResult>(
            string bucket,
            string key,
            [FromServices] IS3Provider provider) =>
        {
            var result = await provider.GenerateDownloadUrlAsync(bucket, key);

            return Results.Ok(result);
        }).DisableAntiforgery();
    }
}
