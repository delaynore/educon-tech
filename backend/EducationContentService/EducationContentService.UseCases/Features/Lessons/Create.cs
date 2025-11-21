using CSharpFunctionalExtensions;
using EducationContentService.Contracts.Lessons;
using EducationContentService.Domain.Lessons;
using EducationContentService.Domain.Shared;
using EducationContentService.Domain.ValueObjects;
using EducationContentService.UseCases.Endpoints;
using EducationContentService.UseCases.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace EducationContentService.UseCases.Features.Lessons;

public sealed class CreateLessonRequestValidator : AbstractValidator<CreateLessonRequest>
{
    public CreateLessonRequestValidator()
    {
        RuleFor(x => x.Title).MustBeValueObject(Title.Create);
        RuleFor(x => x.Description).MustBeValueObject(Description.Create);
    }
}

public sealed class CreateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost(
            "/lessons",
            async Task<EndpointResult<Guid>>(
                [FromBody] CreateLessonRequest createLessonRequest,
                [FromServices] CreateHandler handler,
                CancellationToken cancellationToken) => await handler.Handle(createLessonRequest, cancellationToken));
    }
}

public sealed class CreateHandler
{
    private readonly ILogger<CreateHandler> _logger;
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IValidator<CreateLessonRequest> _validator;

    public CreateHandler(
        ILogger<CreateHandler> logger,
        ILessonsRepository lessonsRepository,
        IValidator<CreateLessonRequest> validator)
    {
        _logger = logger;
        _lessonsRepository = lessonsRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, Error>> Handle(
        CreateLessonRequest createLessonRequest,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(createLessonRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation(
            "CreateLessonRequest: {Title}, {Description}",
            createLessonRequest.Title,
            createLessonRequest.Description);

        var title = Title.Create(createLessonRequest.Title).Value;

        var description = Description.Create(createLessonRequest.Description).Value;

        var lesson = new Lesson(Guid.NewGuid(), title, description);

        var result = await _lessonsRepository.AddAsync(lesson, cancellationToken);
        if (result.IsFailure)
        {
            return result.Error;
        }

        _logger.LogInformation("Created lesson {Id}", lesson.Id);

        return lesson.Id;
    }
}
