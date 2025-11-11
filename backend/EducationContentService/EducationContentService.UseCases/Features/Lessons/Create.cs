using CSharpFunctionalExtensions;
using EducationContentService.Domain.Lessons;
using EducationContentService.Domain.Shared;
using EducationContentService.Domain.ValueObjects;
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
        builder.MapPost("/lessons", async (
            [FromBody] CreateLessonRequest createLessonRequest,
            CreateHandler handler) =>
        {
            return await handler.Handle(createLessonRequest);
        });
    }
}

public sealed class CreateHandler
{
    private readonly ILogger<CreateHandler> _logger;
    private readonly ILessonsRepository _lessonsRepository;

    public CreateHandler(
        ILogger<CreateHandler> logger,
        ILessonsRepository lessonsRepository)
    {
        _logger = logger;
        _lessonsRepository = lessonsRepository;
    }

    public async Task<Result<Guid, Error>> Handle(CreateLessonRequest createLessonRequest)
    {
        _logger.LogInformation(
            "CreateLessonRequest: {Title}, {Description}",
            createLessonRequest.Title,
            createLessonRequest.Description);

        var titleResult = Title.Create(createLessonRequest.Title);
        if (titleResult.IsFailure)
        {
            return titleResult.Error;
        }

        var description = Description.Create(createLessonRequest.Description);
        if (description.IsFailure)
        {
            return description.Error;
        }

        var lesson = new Lesson(Guid.NewGuid(), titleResult.Value, description.Value);
        var result = await _lessonsRepository.AddAsync(lesson);
        if (result.IsFailure)
        {
            return result.Error;
        }

        _logger.LogInformation("Created lesson {Id}", lesson.Id);

        return lesson.Id;
    }
}
