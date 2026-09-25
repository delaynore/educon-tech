namespace EducationContentService.Contracts.Lessons;

public sealed record MediaDto(
    Guid Id,
    string Url,
    string Status);
