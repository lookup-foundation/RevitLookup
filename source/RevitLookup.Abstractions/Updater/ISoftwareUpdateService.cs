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

namespace RevitLookup.Abstractions.Updater;

/// <summary>
///     Defines a contract that checks for and downloads software updates.
/// </summary>
public interface ISoftwareUpdateService
{
    /// <summary>
    ///     Gets the newer version available to download, or <see langword="null" /> when none was found.
    /// </summary>
    string? NewVersion { get; }

    /// <summary>
    ///     Gets the URL to the release notes of <see cref="NewVersion" />, or <see langword="null" /> when none was found.
    /// </summary>
    string? ReleaseNotesUrl { get; }

    /// <summary>
    ///     Gets the local file path of the downloaded update, or <see langword="null" /> when it has not been downloaded.
    /// </summary>
    string? LocalFilePath { get; }

    /// <summary>
    ///     Gets the date of the latest check for updates, or <see langword="null" /> when no check has run yet.
    /// </summary>
    DateTime? LatestCheckDate { get; }

    /// <summary>
    ///     Checks the server for a newer version.
    /// </summary>
    /// <returns>A task that represents the asynchronous check operation. The task result is <see langword="true" /> when a newer version is available.</returns>
    Task<bool> CheckUpdatesAsync();

    /// <summary>
    ///     Downloads the update from the server.
    /// </summary>
    /// <returns>A task that represents the asynchronous download operation.</returns>
    Task DownloadUpdateAsync();
}
