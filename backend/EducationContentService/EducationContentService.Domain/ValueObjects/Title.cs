using CSharpFunctionalExtensions;
using EducationContentService.Domain.Shared;

namespace EducationContentService.Domain.ValueObjects;

public sealed record Title
{
    public const int MaxLength = 200;

    private Title(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Title, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsInvalid("title");
        }

        var normalizedValue = RegexExtensions.SpaceRemoveRegex().Replace(value.Trim(), " ");

        if (normalizedValue.Length > MaxLength)
        {
            return GeneralErrors.ValueIsInvalid("title");
        }

        return new Title(normalizedValue);
    }
}
