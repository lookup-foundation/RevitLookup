using System.Text.Json.Serialization;

namespace RevitLookup.Abstractions.Models.GitHub;

/// <summary>
///     Represents a GitHub response asset.
/// </summary>
[Serializable]
public sealed class GitHubResponseAsset
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("browser_download_url")] public string? DownloadUrl { get; set; }
}