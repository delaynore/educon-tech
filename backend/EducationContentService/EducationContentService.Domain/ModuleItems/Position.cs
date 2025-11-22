using CSharpFunctionalExtensions;
using EducationContentService.Domain.Shared;
using SharedKernel;

namespace EducationContentService.Domain.ModuleItems;

public sealed record Position
{
    public const decimal InitialStep = 1000;

    private Position(ItemType type, decimal value)
    {
        (Type, Value) = (type, value);
    }

    public decimal Value { get; }

    public ItemType Type { get; }

    public static Position First(ItemType type)
    {
        return new Position(type, InitialStep);
    }

    public static Position After(Position position)
    {
        return new Position(position.Type, position.Value + InitialStep);
    }

    public static Result<Position, Error> Between(Position first, Position second)
    {
        if (first.Type != second.Type)
        {
            return Error.Validation(
                new ErrorMessage(
                    "position.type",
                    "Elements type not equal",
                    "position"));
        }

        if (first.Value >= second.Value)
        {
            return Error.Validation(
                new ErrorMessage(
                    "position.value",
                    "The first must not be greater or equal to second position",
                    "position"));
        }

        return new Position(first.Type, (first.Value + second.Value) / 2m);
    }
}
