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
using RevitLookup.Abstractions.Services.Settings;

namespace RevitLookup.UI.Playground.Services.Host;

/// <summary>
///     Provides life cycle processes for the application
/// </summary>
public sealed partial class RevitApplicationService(ISettingsService settingsService, ILogger<RevitApplicationService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        LoadSettings();
        return Task.CompletedTask;
    }

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
    private static partial void LogSavingSettings(ILogger<RevitApplicationService> logger);

    [LoggerMessage(LogLevel.Information, "Loading settings")]
    private static partial void LogLoadingSettings(ILogger<RevitApplicationService> logger);
}