using CSharpFunctionalExtensions;
using SharedKernel;

namespace FileService.Domain;

public sealed class PreviewAsset : MediaAsset
{
    public const long MaxSizeBytes = 5_368_709;
    public const string PreviewAssetType = "preview";
    public const string PreviewLocation = "preview";
    public const string RawPrefix = "raw";

    public const string AllowedContentType = "image";

    public static readonly string[] AllowedExtensions = ["jpg", "jpeg", "png", "webp"];

    private PreviewAsset(
        Guid id,
        MediaData mediaData,
        MediaStatus mediaStatus,
        StorageKey storageKey)
        : base(id, mediaData, mediaStatus, Domain.AssetType.Preview, storageKey)
    {
    }

    public static UnitResult<Error> Validate(MediaData mediaData)
    {
        if (!AllowedExtensions.Contains(mediaData.FileName.Extension))
        {
            return Error.Validation(
                "preview.invalid.extension",
                $"File extension must be one of: {string.Join(" ", AllowedExtensions)}");
        }

        if (mediaData.ContentType.Category != MediaType.Video)
        {
            return Error.Validation(
                "preview.invalid.content-type",
                $"File content must be {AllowedContentType}");
        }

        if (mediaData.Size > MaxSizeBytes)
        {
            return Error.Validation(
                "preview.invalid.size",
                $"File size must be less than {MaxSizeBytes} bytes");
        }

        return UnitResult.Success<Error>();
    }

    public static Result<PreviewAsset, Error> CreateForUpload(Guid id, MediaData mediaData)
    {
        var validationResult = Validate(mediaData);
        if (validationResult.IsFailure)
        {
            return validationResult.Error;
        }

        var keyResult = StorageKey.Create(PreviewLocation, null, id.ToString());
        if (keyResult.IsFailure)
        {
            return keyResult.Error;
        }

        return new PreviewAsset(id, mediaData, MediaStatus.Uploading, keyResult.Value);
    }
}
