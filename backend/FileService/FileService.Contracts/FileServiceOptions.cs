namespace FileService.Contracts;

public sealed class FileServiceOptions
{
    public string BaseAddress { get; init; }

    public int TimeoutSeconds { get; init; } = 10;
}
