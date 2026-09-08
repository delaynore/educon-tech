using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FileService.UseCases.Features;

public sealed class UploadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/files", async Task (
            [FromForm] IFormFile formFile,
            [FromServices] IS3Provider provider,
            CancellationToken token) =>
        {
            var key = $"raw/{Guid.NewGuid()}";
            await provider.UploadFileAsync(formFile.OpenReadStream(), "pictures", key, formFile.ContentType, token);
        }).DisableAntiforgery();
    }
}
