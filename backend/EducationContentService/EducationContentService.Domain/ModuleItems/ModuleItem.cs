namespace EducationContentService.Domain.ModuleItems;

public sealed class ModuleItem
{
    public ModuleItem(Guid? id, Guid moduleId, ItemReference itemReference, Position position)
    {
        Id = id ?? Guid.CreateVersion7();
        ModuleId = moduleId;
        ItemReference = itemReference;
        Position = position;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    // EF core
    private ModuleItem() { }

    public Guid Id { get; private set; }

    public Guid ModuleId { get; private set; }

    public ItemReference ItemReference { get; private set; }

    public DateTime CreatedAtUtc { get; }

    public DateTime UpdatedAtUtc { get; private set; }

    public Position Position { get; private set; }
}
