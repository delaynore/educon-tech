using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using EducationContentService.Domain.Lessons;
using SharedKernel;

namespace EducationContentService.UseCases.Features.Lessons;

public interface ILessonsRepository
{
    Task<Result<Guid, Error>> AddAsync(Lesson lesson, CancellationToken cancellationToken = default);

    Task<Result<Lesson, Error>> GetByAsync(Expression<Func<Lesson, bool>> predicate, CancellationToken cancellationToken = default);
}
