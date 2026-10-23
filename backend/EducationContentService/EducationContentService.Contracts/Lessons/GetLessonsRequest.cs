namespace EducationContentService.Contracts.Lessons;

public sealed record GetLessonsRequest(string? Search, int Page, int PageSize);
