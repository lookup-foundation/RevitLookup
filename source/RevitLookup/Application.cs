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

using System.Windows.Interop;
using System.Windows.Media;
using Nice3point.Revit.Toolkit.External;
using RevitLookup.Abstractions.Services.Appearance;
using RevitLookup.Abstractions.Services.Application;
using RevitLookup.Abstractions.Services.Settings;

namespace RevitLookup;

[UsedImplicitly]
public partial class Application : AsyncExternalApplication
{
    public override async Task OnStartupAsync()
    {
        await Host.StartAsync();

        EnableThemes();
        EnableHardwareRendering();
    }

    public override async Task OnShutdownAsync()
    {
        await Host.StopAsync();
    }

    private static void EnableThemes()
    {
        var uiService = Host.GetService<IUiOrchestratorService>();
        uiService.RunService<IThemeWatcherService>(themeService =>
        {
            themeService.Initialize();
            themeService.ApplyTheme();
        });
    }

    public static void EnableHardwareRendering()
    {
        var settingsService = Host.GetService<ISettingsService>();
        if (!settingsService.ApplicationSettings.UseHardwareRendering) return;

        //Revit overrides render mode during initialization
        //ExternalEvent is called after initialization
        EnableHardwareRenderingAfterInitializationEvent.Raise();
    }

    public static void DisableHardwareRendering()
    {
        var settingsService = Host.GetService<ISettingsService>();
        if (settingsService.ApplicationSettings.UseHardwareRendering) return;

        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
    }

    [ExternalEvent]
    private static void EnableHardwareRenderingAfterInitialization()
    {
        RenderOptions.ProcessRenderMode = RenderMode.Default;
    }
}