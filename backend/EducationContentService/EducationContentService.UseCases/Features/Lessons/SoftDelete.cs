using CSharpFunctionalExtensions;
using EducationContentService.UseCases.Database;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace EducationContentService.UseCases.Features.Lessons;

public sealed class SoftDeleteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete(
            "/lessons/{lessonId}",
            async Task<EndpointResult<Guid>>(
                [FromRoute] Guid lessonId,
                [FromServices] SoftDeleteHandler handler,
                CancellationToken cancellationToken) => await handler.Handle(lessonId, cancellationToken));
    }
}

public sealed class SoftDeleteHandler
{
    private readonly ILessonsRepository _lessonsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<SoftDeleteHandler> _logger;

    public SoftDeleteHandler(
        ILessonsRepository lessonsRepository,
        ITransactionManager transactionManager,
        ILogger<SoftDeleteHandler> logger)
    {
        _lessonsRepository = lessonsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> Handle(
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        var (_, isFailure, lesson, error) = await _lessonsRepository.GetByAsync(x => x.Id == lessonId, cancellationToken);

        if (isFailure)
        {
            return error;
        }

        lesson.SoftDelete();

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Lesson {LessonId} soft deleted successful", lessonId);

        return lessonId;
    }
}
