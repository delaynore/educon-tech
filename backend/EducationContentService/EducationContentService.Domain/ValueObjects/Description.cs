using CSharpFunctionalExtensions;
using EducationContentService.Domain.Shared;

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
        if (string.IsNullOrWhiteSpace(value) || value.Length > MaxLength)
        {
            return GeneralErrors.ValueIsInvalid("description");
        }

        return new Description(value);
    }
}
