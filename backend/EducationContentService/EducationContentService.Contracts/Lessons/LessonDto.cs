namespace EducationContentService.Contracts.Lessons;

public sealed record LessonDto(
    Guid Id,
    string Title,
    string Description,
    MediaDto? Media,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
