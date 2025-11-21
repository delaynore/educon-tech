namespace EducationContentService.Contracts.Lessons;

public sealed record GetLessonRequest(string? Search, int Page, int PageSize);
