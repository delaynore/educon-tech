namespace EducationContentService.Contracts.Lessons;

public sealed record PaginatedLessonsDto(IReadOnlyList<LessonDto> Items, int TotalCount);
