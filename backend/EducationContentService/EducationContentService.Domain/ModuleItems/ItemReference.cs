namespace EducationContentService.Domain.ModuleItems;

public sealed record ItemReference
{
    public Guid ItemId { get; set; }
    public ItemType Type { get; set; }

    private ItemReference(ItemType type, Guid itemId)
        => (ItemId, Type) = (itemId, type);

    public static ItemReference ToLesson(Guid lessonId) => new(ItemType.Lesson, lessonId);

    public static ItemReference ToIssue(Guid issueId) => new(ItemType.Issue, issueId);
}
