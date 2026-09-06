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
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.Settings;
using RevitLookup.Abstractions.Updater;
using RevitLookup.Commands;
using RevitLookup.UI.Framework.Processes;

namespace RevitLookup;

/// <summary>
///     Provides life cycle processes for the application.
/// </summary>
/// <param name="settingsService">The service that loads and saves the application settings.</param>
/// <param name="updateService">The service that checks for and downloads software updates.</param>
/// <param name="orchestratorService">The service that runs other services on the UI thread.</param>
/// <param name="ribbonService">The service that creates the Revit ribbon.</param>
/// <param name="logger">The logger this service writes diagnostic records to.</param>
public sealed partial class HostBackgroundService(
    ISettingsService settingsService,
    ISoftwareUpdateService updateService,
    IUiOrchestratorService orchestratorService,
    RevitRibbonService ribbonService,
    ILogger<HostBackgroundService> logger)
    : IHostedService
{
    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        LoadSettings();
        InitializeThemes();
        CreateRibbon();
        _ = CheckUpdatesAsync();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
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
            if (!hasUpdates)
            {
                return;
            }

            LogUpdateAvailable(logger, updateService.NewVersion);
        }
        catch (Exception exception)
        {
            LogUpdateServiceError(logger, exception);
        }
    }

    private void UpdateSoftware()
    {
        if (!File.Exists(updateService.LocalFilePath))
        {
            return;
        }

        LogInstallingVersion(logger, updateService.NewVersion);
        ProcessTasks.StartShell(updateService.LocalFilePath!);
    }

    /// <summary>
    ///     Initializes and applies theme services for the application UI
    /// </summary>
    private void InitializeThemes()
    {
        try
        {
            orchestratorService.RunService<IThemeWatcherService>(themeService =>
            {
                themeService.Initialize();
                themeService.ApplyTheme();
            });
        }
        catch (Exception exception)
        {
            LogThemeInitializationError(logger, exception);
        }
    }

    /// <summary>
    ///     Creates the Revit ribbon.
    /// </summary>
    private void CreateRibbon()
    {
        try
        {
            ribbonService.CreateRibbon();
        }
        catch (Exception exception)
        {
            LogCreatingRibbonError(logger, exception);
        }
    }

    /// <summary>
    ///     Saves the current application settings to the storage.
    /// </summary>
    private void SaveSettings()
    {
        LogSavingSettings(logger);
        settingsService.SaveSettings();
    }

    /// <summary>
    ///     Loads application settings into the application context.
    /// </summary>
    private void LoadSettings()
    {
        LogLoadingSettings(logger);
        settingsService.LoadSettings();
    }

    [LoggerMessage(LogLevel.Information, "RevitLookup {Version} is available to download")]
    private static partial void LogUpdateAvailable(ILogger<HostBackgroundService> logger, string? version);

    [LoggerMessage(LogLevel.Error, "Update service error")]
    private static partial void LogUpdateServiceError(ILogger<HostBackgroundService> logger, Exception exception);

    [LoggerMessage(LogLevel.Information, "Installing RevitLookup {Version} version")]
    private static partial void LogInstallingVersion(ILogger<HostBackgroundService> logger, string? version);

    [LoggerMessage(LogLevel.Error, "Theme initialization error")]
    private static partial void LogThemeInitializationError(ILogger<HostBackgroundService> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Creating ribbon error")]
    private static partial void LogCreatingRibbonError(ILogger<HostBackgroundService> logger, Exception exception);

    [LoggerMessage(LogLevel.Information, "Saving settings")]
    private static partial void LogSavingSettings(ILogger<HostBackgroundService> logger);

    [LoggerMessage(LogLevel.Information, "Loading settings")]
    private static partial void LogLoadingSettings(ILogger<HostBackgroundService> logger);
}
