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

namespace RevitLookup.Abstractions.Models.GitHub;

/// <summary>
///     Represents a GitHub API response.
/// </summary>
[Serializable]
public sealed class GitHubResponse
{
    [JsonPropertyName("html_url")] public string? Url { get; set; }
    [JsonPropertyName("tag_name")] public string? TagName { get; set; }
    [JsonPropertyName("draft")] public bool Draft { get; set; }
    [JsonPropertyName("prerelease")] public bool PreRelease { get; set; }
    [JsonPropertyName("published_at")] public DateTimeOffset PublishedDate { get; set; }
    [JsonPropertyName("assets")] public List<GitHubResponseAsset>? Assets { get; set; }
}