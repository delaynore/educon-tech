using CSharpFunctionalExtensions;
using EducationContentService.Domain.Lessons;
using EducationContentService.Domain.Shared;

namespace EducationContentService.UseCases.Features.Lessons;

public interface ILessonsRepository
{
    Task<Result<Guid, Error>> AddAsync(Lesson lesson, CancellationToken cancellationToken = default);
}
