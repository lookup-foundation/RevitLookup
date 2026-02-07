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
using RevitLookup.Services.Application;

namespace RevitLookup;

[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        Host.Start();
        EventHandlers.RegisterHandlers();

        EnableThemes();
        EnableHardwareRendering();

        var ribbonService = Host.GetService<RevitRibbonService>();
        ribbonService.CreateRibbon();
    }

    public override void OnShutdown()
    {
        Host.Stop();
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
        //EventHandler is called after initialization
        EventHandlers.ActionEventHandler.Raise(_ => RenderOptions.ProcessRenderMode = RenderMode.Default);
    }

    public static void DisableHardwareRendering()
    {
        var settingsService = Host.GetService<ISettingsService>();
        if (settingsService.ApplicationSettings.UseHardwareRendering) return;

        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
    }
}