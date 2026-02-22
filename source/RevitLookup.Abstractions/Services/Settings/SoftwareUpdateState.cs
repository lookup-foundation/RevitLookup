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
///     The state of the application update.
/// </summary>
public enum SoftwareUpdateState
{
    /// <summary>
    ///     The application is up to date.
    /// </summary>
    UpToDate,

    /// <summary>
    ///     The new version is available on the server.
    /// </summary>
    ReadyToDownload,

    /// <summary>
    ///     The new version is downloaded and ready to install.
    /// </summary>
    ReadyToInstall,

    /// <summary>
    ///     Error occurred during the update process.
    /// </summary>
    Error
}