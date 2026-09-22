using CSharpFunctionalExtensions;
using SharedKernel;

namespace FileService.UseCases.FilesStorage;

public interface IChunkSizeCalculator
{
    Result<(int ChunkSizeBytes, int TotalChunks), Error> Calculate(long fileSizeBytes);
}
