using CSharpFunctionalExtensions;
using SharedKernel;

namespace FileService.Domain;

public sealed class VideoAsset : MediaAsset
{
    public const long MaxSizeBytes = 5_368_709_120;
    public const string Location = "videos";
    public const string RawPrefix = "raw";

    // public const string HlsFolder = "hls";
    // public const string MasterPlaylistName = "master.m3u8";
    public const string AllowedContentType = "video";

    public static readonly string[] AllowedExtensions = ["mp4", "mkv", "avi", "mov"];

    private VideoAsset(
        Guid id,
        MediaData mediaData,
        MediaStatus mediaStatus,
        StorageKey storageKey)
        : base(id, mediaData, mediaStatus, AssetType.Video, storageKey)
    {
    }

    public static UnitResult<Error> Validate(MediaData mediaData)
    {
        if (!AllowedExtensions.Contains(mediaData.FileName.Extension))
        {
            return Error.Validation(
                "video.invalid.extension",
                $"File extension must be one of: {string.Join(" ", AllowedExtensions)}");
        }

        if (mediaData.ContentType.Category != MediaType.Video)
        {
            return Error.Validation(
                "video.invalid.content-type",
                $"File content must be {AllowedContentType}");
        }

        if (mediaData.Size > MaxSizeBytes)
        {
            return Error.Validation(
                "video.invalid.size",
                $"File size must be less than {MaxSizeBytes} bytes");
        }

        return UnitResult.Success<Error>();
    }

    public static Result<VideoAsset, Error> CreateForUpload(Guid id, MediaData mediaData)
    {
        var validationResult = Validate(mediaData);
        if (validationResult.IsFailure)
        {
            return validationResult.Error;
        }

        var keyResult = StorageKey.Create(Location, null, id.ToString());
        if (keyResult.IsFailure)
        {
            return keyResult.Error;
        }

        return new VideoAsset(id, mediaData, MediaStatus.Uploading, keyResult.Value);
    }
}
