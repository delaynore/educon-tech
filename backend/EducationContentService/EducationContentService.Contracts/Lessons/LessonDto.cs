namespace EducationContentService.Contracts.Lessons;

public sealed record LessonDto(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
