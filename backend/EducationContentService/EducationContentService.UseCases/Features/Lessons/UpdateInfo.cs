using CSharpFunctionalExtensions;
using EducationContentService.Contracts.Lessons;
using EducationContentService.Domain.Shared;
using EducationContentService.Domain.ValueObjects;
using EducationContentService.UseCases.Database;
using EducationContentService.UseCases.Endpoints;
using EducationContentService.UseCases.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace EducationContentService.UseCases.Features.Lessons;

public sealed class UpdateLessonInfoRequestValidator : AbstractValidator<UpdateLessonInfoRequest>
{
    public UpdateLessonInfoRequestValidator()
    {
        RuleFor(x => x.Title).MustBeValueObject(Title.Create);
        RuleFor(x => x.Description).MustBeValueObject(Description.Create);
    }
}

public sealed class UpdateInfoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut(
            "/lessons/{lessonId}",
            async Task<EndpointResult>(
                [FromRoute] Guid lessonId,
                [FromBody] UpdateLessonInfoRequest request,
                [FromServices] UpdateInfoHandler handler,
                CancellationToken cancellationToken) => await handler.Handle(lessonId, request, cancellationToken));
    }
}

public sealed class UpdateInfoHandler
{
    private readonly ILogger<UpdateInfoHandler> _logger;
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IValidator<UpdateLessonInfoRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public UpdateInfoHandler(
        ILogger<UpdateInfoHandler> logger,
        ILessonsRepository lessonsRepository,
        IValidator<UpdateLessonInfoRequest> validator,
        ITransactionManager transactionManager)
    {
        _logger = logger;
        _lessonsRepository = lessonsRepository;
        _validator = validator;
        _transactionManager = transactionManager;
    }

    public async Task<UnitResult<Error>> Handle(
        Guid lessonId,
        UpdateLessonInfoRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        var (_, isFailure, lesson, error) = await _lessonsRepository.GetByAsync(x => x.Id == lessonId, cancellationToken);
        if (isFailure)
        {
            return error;
        }

        var newTitle = Title.Create(request.Title).Value;
        var newDescription = Description.Create(request.Description).Value;

        lesson.UpdateInfo(newTitle, newDescription);

        // todo: should handle exception
        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{LessonId} - Lesson info was updated", lessonId);

        return UnitResult.Success<Error>();
    }
}
