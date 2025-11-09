using EducationContentService.Domain;
using EducationContentService.Domain.Lessons;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace EducationContentService.UseCases.Features.Lessons;

public record CreateLessonRequest(string Title, string Description);

public sealed class CreateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/lessons", async ([FromBody] CreateLessonRequest createLessonRequest, CreateHandler handler) =>
        {
            await handler.Handle(createLessonRequest);
        });
    }
}

public sealed class CreateHandler
{
    private readonly ILogger<CreateHandler> _logger;

    public CreateHandler(ILogger<CreateHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CreateLessonRequest createLessonRequest)
    {
        _logger.LogInformation("CreateLessonRequest: {Title}, {Description}", createLessonRequest.Title, createLessonRequest.Description);

        return Task.CompletedTask;
    }
}
