using CSharpFunctionalExtensions;
using SharedKernel;

namespace FileService.Domain;

public sealed record MediaData
{
    public FileName FileName { get; init; }

    public ContentType ContentType { get; init; }

    public long SizeBytes { get; init; }

    public int ExpectedChunksCount { get; init; }

    private MediaData()
    {
    }

    private MediaData(
        FileName fileName,
        ContentType contentType,
        long sizeBytes,
        int expectedChunksCount)
    {
        FileName = fileName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        ExpectedChunksCount = expectedChunksCount;
    }

    public static Result<MediaData, Error> Create(
        FileName fileName,
        ContentType contentType,
        long size,
        int expectedChunksCount)
    {
        if (size < 0)
        {
            return GeneralErrors.ValueIsInvalid(nameof(size));
        }

        if (expectedChunksCount < 0)
        {
            return GeneralErrors.ValueIsInvalid(nameof(expectedChunksCount));
        }

        return new MediaData(fileName, contentType, size, expectedChunksCount);
    }
}
