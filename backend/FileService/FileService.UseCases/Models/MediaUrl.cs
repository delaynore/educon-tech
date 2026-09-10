using FileService.Domain;

namespace FileService.UseCases.Models;

public sealed record MediaUrl(StorageKey StorageKey, string PresignedUrl);
