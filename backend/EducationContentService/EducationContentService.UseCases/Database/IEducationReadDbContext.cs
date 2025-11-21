using EducationContentService.Domain.Lessons;

namespace EducationContentService.UseCases.Database;

public interface IEducationReadDbContext
{
    IQueryable<Lesson> LessonQuery { get; }
}
