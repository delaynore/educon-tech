using CSharpFunctionalExtensions;
using EducationContentService.Domain.Shared;

namespace EducationContentService.Domain.ModuleItems;

public sealed record Position
{
    public const decimal InitialStep = 1000;

    public decimal Value { get; }

    public ItemType Type { get; }

    private Position(ItemType type, decimal value) => (Type, Value) = (type, value);

    public static Position First(ItemType type) => new(type, InitialStep);

    public static Position After(Position position) => new(position.Type, position.Value + InitialStep);

    public static Result<Position, Error> Between(Position first, Position second)
    {
        if (first.Type != second.Type)
        {
            return Error.Validation("position.type", "Elements type not equal", "position");
        }

        if (first.Value >= second.Value)
        {
            return Error.Validation("position.value", "The first must not be greater or equal to second position",  "position");
        }

        return new Position(first.Type, (first.Value + second.Value) / 2m);
    }
}
