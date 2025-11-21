using EducationContentService.Domain.ValueObjects;

namespace EducationContentService.Domain.Lessons;

public sealed class Lesson
{
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

    public Guid Id { get; private set; }

    public Title Title { get; private set; }

    public Description Description { get; private set; }

    public DateTime CreatedAtUtc { get; }

    public DateTime UpdatedAtUtc { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAtUtc { get; private set; }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
    }

    public void UpdateInfo(Title title, Description description)
    {
        Title = title;
        Description = description;

        UpdatedAtUtc = DateTime.UtcNow;
    }
}
