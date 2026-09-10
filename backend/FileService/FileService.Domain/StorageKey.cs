using CSharpFunctionalExtensions;
using SharedKernel;

namespace FileService.Domain;

public sealed record StorageKey
{
    public string Key { get; init; }

    public string Prefix { get; init; }

    public string Location { get; init; }

    public string Value { get; init; }

    public string FullPath { get; init; }

    private StorageKey(string location, string prefix, string key)
    {
        Key = key;
        Prefix = prefix;
        Location = location;
        Value = string.IsNullOrEmpty(Prefix) ? key : $"{Prefix}/{key}";
        FullPath = $"{Location}/{Value}";
    }

    public static Result<StorageKey, Error> Create(string location, string? prefix, string key)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return GeneralErrors.ValueIsInvalid(nameof(location));
        }

        var normalizedKeyResult = NormalizeSegment(key);
        if (normalizedKeyResult.IsFailure)
        {
            return normalizedKeyResult.Error;
        }

        var normalizedPrefixResult = NormalizePrefix(prefix);
        if (normalizedPrefixResult.IsFailure)
        {
            return normalizedPrefixResult.Error;
        }

        return new StorageKey(location.Trim(), normalizedPrefixResult.Value, normalizedKeyResult.Value);
    }

    private static Result<string, Error> NormalizeSegment(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsInvalid("key");
        }

        var trimmed = value.Trim();

        if (trimmed.Contains('/', StringComparison.Ordinal) || trimmed.Contains('\\', StringComparison.Ordinal))
        {
            return GeneralErrors.ValueIsInvalid("key");
        }

        return trimmed;
    }

    private static Result<string, Error> NormalizePrefix(string? prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            return string.Empty;
        }

        var parts = prefix
            .Trim()
            .Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        List<string> normalizedParts = [];
        foreach (var part in parts)
        {
            var normalizedPartResult = NormalizeSegment(part);
            if (normalizedPartResult.IsFailure)
            {
                return normalizedPartResult;
            }

            if (!string.IsNullOrEmpty(normalizedPartResult.Value))
            {
                normalizedParts.Add(normalizedPartResult.Value);
            }
        }

        return string.Join("/", normalizedParts);
    }
}
