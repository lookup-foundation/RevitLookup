using System.Text.Json.Serialization;

namespace RevitLookup.Abstractions.Updater;

/// <summary>
///     Represents a release asset from the GitHub releases API.
/// </summary>
[PublicAPI]
public sealed class GitHubResponseAsset
{
    /// <summary>
    ///     Gets or sets the file name of the release asset.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the URL to download the release asset.
    /// </summary>
    [JsonPropertyName("browser_download_url")]
    public string? DownloadUrl { get; set; }
}
