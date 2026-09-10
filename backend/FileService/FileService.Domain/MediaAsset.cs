using System.Runtime.InteropServices.JavaScript;
using System.Security.AccessControl;
using CSharpFunctionalExtensions;
using SharedKernel;

namespace FileService.Domain;

public abstract class MediaAsset
{
    public Guid Id { get; protected set; }

    public MediaData MediaData { get; protected set; }

    public AssetType AssetType { get; protected set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public StorageKey Key { get; protected set; }

    public MediaOwner Owner { get; protected set; }

    public MediaStatus Status { get; protected set; }

    protected MediaAsset()
    {
    }

    protected MediaAsset(
        Guid id,
        MediaData mediaData,
        MediaStatus status,
        AssetType assetAssetType,
        StorageKey key)
    {
        Id = id;
        MediaData = mediaData;
        AssetType = assetAssetType;
        Status = status;
        Key = key;
    }

    public static Result<MediaAsset, Error> CreateForUpload(MediaData mediaData, AssetType assetType)
    {
        var mediaAssetId = Guid.CreateVersion7();

        switch (assetType)
        {
            case AssetType.Video:
                var videoResult = VideoAsset.CreateForUpload(mediaAssetId, mediaData);
                return videoResult.IsFailure ? videoResult.Error : videoResult.Value;
            case AssetType.Preview:
                var previewResult = PreviewAsset.CreateForUpload(mediaAssetId, mediaData);
                return previewResult.IsFailure ? previewResult.Error : previewResult.Value;

            case AssetType.Avatar:
            default:
                throw new ArgumentOutOfRangeException(nameof(assetType), assetType, null);
        }
    }

    public UnitResult<Error> MarkUploaded()
    {
        if (Status != MediaStatus.Uploading)
        {
            return UnitResult.Success<Error>();
        }

        Status = MediaStatus.Uploaded;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    public void MarkFailed()
    {
        Status = MediaStatus.Uploaded;
        UpdatedAt = DateTime.UtcNow;
    }
}
