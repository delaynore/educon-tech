using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using EducationContentService.Domain.Lessons;
using EducationContentService.Domain.Shared;
using EducationContentService.Infrastructure.Postgres.Configurations;
using EducationContentService.UseCases.Features.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using SharedKernel;

namespace EducationContentService.Infrastructure.Postgres;

public sealed class LessonsRepository : ILessonsRepository
{
    private readonly EducationDbContext _educationDbContext;
    private readonly ILogger<LessonsRepository> _logger;

    public LessonsRepository(EducationDbContext educationDbContext, ILogger<LessonsRepository> logger)
    {
        _educationDbContext = educationDbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Lesson lesson, CancellationToken cancellationToken = default)
    {
        _educationDbContext.Add(lesson);

        try
        {
            await _educationDbContext.SaveChangesAsync(cancellationToken);

            return lesson.Id;
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException pgException)
        {
            if (pgException is { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: not null }
                && pgException.ConstraintName.Contains(DbIndex.LessonTitle,  StringComparison.InvariantCultureIgnoreCase))
            {
                return EducationErrors.TitleConflict(lesson.Title.Value);
            }

            _logger.LogError(e, "Database update error while creating lesson with id - {LessonId}", lesson.Id);

            return EducationErrors.DatabaseError();
        }
        catch (OperationCanceledException e)
        {
            _logger.LogError(e, "Operation was cancelled exception while creating lesson with id - {LessonId}", lesson.Id);

            return EducationErrors.OperationCancelled();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error while creating lesson with id - {LessonId}", lesson.Id);

            return EducationErrors.DatabaseError();
        }
    }

    public async Task<Result<Lesson, Error>> GetByAsync(Expression<Func<Lesson, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var lesson = await _educationDbContext.Lessons.FirstOrDefaultAsync(predicate, cancellationToken);

        if (lesson is null)
        {
            return GeneralErrors.NotFound(id: null, name: "lesson");
        }

        return lesson;
    }
}
