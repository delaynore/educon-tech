using CSharpFunctionalExtensions;
using EducationContentService.Domain.Shared;
using SharedKernel;

namespace EducationContentService.Domain.ValueObjects;

public sealed record Description
{
    public const int MaxLength = 1000;

    private Description(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Description, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsInvalid("description");
        }

        var normalizedValue = RegexExtensions.SpaceRemoveRegex().Replace(value.Trim(), " ");

        if (normalizedValue.Length > MaxLength)
        {
            return GeneralErrors.ValueIsInvalid("description");
        }

        return new Description(normalizedValue);
    }
}
