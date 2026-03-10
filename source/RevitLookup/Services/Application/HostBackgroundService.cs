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

using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.Common.Utils;

namespace RevitLookup.Services.Application;

/// <summary>
///     Provides life cycle processes for the application
/// </summary>
public sealed class HostBackgroundService(
    ISettingsService settingsService,
    ISoftwareUpdateService updateService,
    RevitRibbonService ribbonService,
    ILogger<HostBackgroundService> logger)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        LoadSettings();
        CreateRibbon();
        _ = CheckUpdatesAsync();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        SaveSettings();
        UpdateSoftware();
        return Task.CompletedTask;
    }

    private async Task CheckUpdatesAsync()
    {
        try
        {
            var hasUpdates = await updateService.CheckUpdatesAsync();
            if (!hasUpdates) return;

            logger.LogInformation("RevitLookup {Version} is available to download", updateService.NewVersion);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Update service error");
        }
    }

    private void UpdateSoftware()
    {
        if (!File.Exists(updateService.LocalFilePath)) return;

        logger.LogInformation("Installing RevitLookup {Version} version", updateService.NewVersion);
        ProcessTasks.StartShell(updateService.LocalFilePath!);
    }

    private void CreateRibbon()
    {
        try
        {
            ribbonService.CreateRibbon();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Creating ribbon error");
        }
    }

    private void SaveSettings()
    {
        logger.LogInformation("Saving settings");
        settingsService.SaveSettings();
    }

    private void LoadSettings()
    {
        logger.LogInformation("Loading settings");
        settingsService.LoadSettings();
    }
}