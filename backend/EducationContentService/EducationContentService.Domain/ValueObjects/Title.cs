using CSharpFunctionalExtensions;
using EducationContentService.Domain.Shared;

namespace EducationContentService.Domain.ValueObjects;

public sealed record Title
{
    public const int MaxLength = 200;

    public string Value { get; }

    private Title(string value)
        => Value = value;

    public static Result<Title, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > MaxLength)
        {
            return Error.Validation("title.invalid", "Title is too long or empty", "title");
        }

        return new Title(value);
    }
}
