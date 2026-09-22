using CSharpFunctionalExtensions;
using FileService.UseCases.FilesStorage;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace FileService.Infrastructure.S3;

public sealed class ChunkSizeCalculator : IChunkSizeCalculator
{
    private readonly S3Options _options;

    public ChunkSizeCalculator(IOptions<S3Options> options)
    {
        _options = options.Value;
    }

    public Result<(int ChunkSizeBytes, int TotalChunks), Error> Calculate(
        long fileSizeBytes)
    {
        if (_options.RecommendedChunkSizeBytes <= 0 || _options.MaxChunks <= 0)
        {
            return GeneralErrors.ValueIsInvalid("chunk settings");
        }

        if (fileSizeBytes <= _options.RecommendedChunkSizeBytes)
        {
            return ((int)fileSizeBytes, 1);
        }

        var calculatedChunks = (int)Math.Ceiling((double)fileSizeBytes / _options.RecommendedChunkSizeBytes);

        var actualChunks = Math.Min(calculatedChunks, _options.MaxChunks);

        var chunkSize = (fileSizeBytes + actualChunks - 1) / actualChunks;

        return ((int)chunkSize, actualChunks);
    }
}
