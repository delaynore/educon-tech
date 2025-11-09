using EducationContentService.Domain.ValueObjects;

namespace EducationContentService.Domain.Lessons;

public sealed class Lesson
{
    public Guid Id { get; private set; }

    public Title Title { get; private set; }

    public Description Description { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAtUtc { get; private set; }

    public Lesson(Guid? id, Title title, Description description)
    {
        Id = id ?? Guid.NewGuid();
        Title = title;
        Description = description;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
        IsDeleted = false;
        DeletedAtUtc = null;
    }

    // EF core
    private Lesson() { }
}
