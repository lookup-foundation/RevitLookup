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

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Settings;

namespace RevitLookup.UI.Playground;

/// <summary>
///     Provides life cycle processes for the application.
/// </summary>
/// <param name="settingsService">The service used to load and save application settings.</param>
/// <param name="logger">The logger used to record life cycle events.</param>
public sealed partial class HostBackgroundService(ISettingsService settingsService, ILogger<HostBackgroundService> logger) : IHostedService
{
    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        LoadSettings();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        SaveSettings();
        return Task.CompletedTask;
    }

    private void SaveSettings()
    {
        LogSavingSettings(logger);
        settingsService.SaveSettings();
    }

    private void LoadSettings()
    {
        LogLoadingSettings(logger);
        settingsService.LoadSettings();
    }

    [LoggerMessage(LogLevel.Information, "Saving settings")]
    private static partial void LogSavingSettings(ILogger<HostBackgroundService> logger);

    [LoggerMessage(LogLevel.Information, "Loading settings")]
    private static partial void LogLoadingSettings(ILogger<HostBackgroundService> logger);
}
