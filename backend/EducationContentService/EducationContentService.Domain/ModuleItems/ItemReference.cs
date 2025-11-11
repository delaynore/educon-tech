namespace EducationContentService.Domain.ModuleItems;

public sealed record ItemReference
{
    private ItemReference(ItemType type, Guid itemId)
    {
        (ItemId, Type) = (itemId, type);
    }

    public Guid ItemId { get; set; }
    public ItemType Type { get; set; }

    public static ItemReference ToLesson(Guid lessonId)
    {
        return new ItemReference(ItemType.Lesson, lessonId);
    }

    public static ItemReference ToIssue(Guid issueId)
    {
        return new ItemReference(ItemType.Issue, issueId);
    }
}
