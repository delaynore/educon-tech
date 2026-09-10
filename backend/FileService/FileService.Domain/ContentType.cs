using CSharpFunctionalExtensions;
using SharedKernel;

namespace FileService.Domain;

public sealed record ContentType
{
    public string Value { get; init; }

    public MediaType Category { get; init; }

    private ContentType(string value, MediaType category)
    {
        Value = value;
        Category = category;
    }

    public static Result<ContentType, Error> Create(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return GeneralErrors.ValueIsInvalid(nameof(contentType));
        }

        var category = contentType switch
        {
            _ when contentType.Contains("video", StringComparison.InvariantCultureIgnoreCase) => MediaType.Video,
            _ when contentType.Contains("image", StringComparison.InvariantCultureIgnoreCase) => MediaType.Image,
            _ when contentType.Contains("audio", StringComparison.InvariantCultureIgnoreCase) => MediaType.Audio,
            _ when contentType.Contains("document", StringComparison.InvariantCultureIgnoreCase) => MediaType.Document,
            _ => Domain.MediaType.Unknown,
        };

        return new ContentType(contentType, category);
    }
}
