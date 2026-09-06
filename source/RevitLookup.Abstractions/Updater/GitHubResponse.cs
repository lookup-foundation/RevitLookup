// Copyright (c) Lookup Foundation and Contributors
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// THIS PROGRAM IS PROVIDED "AS IS" AND WITH ALL FAULTS.
// NO IMPLIED WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE IS PROVIDED.
// THERE IS NO GUARANTEE THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

using System.Text.Json.Serialization;

namespace RevitLookup.Abstractions.Updater;

/// <summary>
///     Represents a release from the GitHub releases API.
/// </summary>
[Serializable]
public sealed class GitHubResponse
{
    /// <summary>
    ///     Gets or sets the URL of the release page on GitHub.
    /// </summary>
    [JsonPropertyName("html_url")]
    public string? Url { get; set; }

    /// <summary>
    ///     Gets or sets the Git tag associated with the release.
    /// </summary>
    [JsonPropertyName("tag_name")]
    public string? TagName { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the release is an unpublished draft.
    /// </summary>
    [JsonPropertyName("draft")]
    public bool Draft { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the release is marked as a prerelease.
    /// </summary>
    [JsonPropertyName("prerelease")]
    public bool PreRelease { get; set; }

    /// <summary>
    ///     Gets or sets the date and time the release was published.
    /// </summary>
    [JsonPropertyName("published_at")]
    public DateTimeOffset PublishedDate { get; set; }

    /// <summary>
    ///     Gets or sets the assets attached to the release, or <see langword="null" /> when the release has none.
    /// </summary>
    [JsonPropertyName("assets")]
    public List<GitHubResponseAsset>? Assets { get; set; }
}
