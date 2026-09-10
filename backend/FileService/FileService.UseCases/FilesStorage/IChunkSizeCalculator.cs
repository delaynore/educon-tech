using CSharpFunctionalExtensions;
using SharedKernel;

namespace FileService.UseCases.FilesStorage;

public interface IChunkSizeCalculator
{
    Result<(long ChunkSizeBytes, int TotalChunks), Error> Calculate(long fileSizeBytes);
}
