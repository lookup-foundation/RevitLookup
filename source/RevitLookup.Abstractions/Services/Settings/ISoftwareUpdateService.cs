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

namespace RevitLookup.Abstractions.Services.Settings;

/// <summary>
///     Software update provider.
/// </summary>
public interface ISoftwareUpdateService
{
    /// <summary>
    ///     A new available version to download.
    /// </summary>
    string? NewVersion { get; }
    
    /// <summary>
    ///     The URL to the release notes of the new version.
    /// </summary>
    string? ReleaseNotesUrl { get; }
    
    /// <summary>
    ///     The local file path to the downloaded update.
    /// </summary>
    string? LocalFilePath { get; }
    
    /// <summary>
    ///     The date of the latest check for updates.
    /// </summary>
    DateTime? LatestCheckDate { get; }

    /// <summary>
    ///     Check for updates on the server.
    /// </summary>
    Task<bool> CheckUpdatesAsync();
    
    /// <summary>
    ///     Download the update from the server.
    /// </summary>
    Task DownloadUpdate();
}