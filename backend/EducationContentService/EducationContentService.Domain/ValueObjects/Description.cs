using CSharpFunctionalExtensions;
using EducationContentService.Domain.Shared;

namespace EducationContentService.Domain.ValueObjects;

public sealed record Description
{
    public const int MaxLength = 1000;

    public string Value { get; }

    private Description(string value)
        => Value = value;

    public static Result<Description, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > MaxLength)
        {
            return Error.Validation("description.invalid", "Description is too long or empty", "title");
        }

        return new Description(value);
    }
}
