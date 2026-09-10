using CSharpFunctionalExtensions;
using SharedKernel;

namespace FileService.Domain;

public sealed record FileName
{
    public string Name { get; init; }

    public string Extension { get; init; }

    private FileName(string name, string extension)
    {
        Name = name;
        Extension = extension;
    }

    public static Result<FileName, Error> Create(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return GeneralErrors.ValueIsInvalid(nameof(fileName));
        }

        var lastDot = fileName.LastIndexOf('.');
        if (lastDot == -1 || lastDot == fileName.Length - 1)
        {
            return GeneralErrors.ValueIsInvalid("File must have extension");
        }

        var extension = fileName[(lastDot + 1) ..].ToLowerInvariant();

        return new FileName(fileName, extension);
    }
}
