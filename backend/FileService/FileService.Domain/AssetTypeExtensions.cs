namespace FileService.Domain;

public static class AssetTypeExtensions
{
    public static AssetType ToAssetType(this string assetType)
    {
        return assetType.ToLowerInvariant() switch
        {
            "video" => AssetType.Video,
            "preview" => AssetType.Preview,
            "avatar" => AssetType.Avatar,
            _ => throw new ArgumentException("Invalid asset type", nameof(assetType)),
        };
    }
}
