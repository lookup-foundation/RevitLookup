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

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.Settings;
using RevitLookup.Abstractions.ViewModels.Settings;
using RevitLookup.Commands;
using RevitLookup.UI.Framework.Views.Settings;
using RevitLookup.UI.Framework.Views.Windows;
using Wpf.Ui;
using Wpf.Ui.Animations;
using Wpf.Ui.Controls;
using ApplicationTheme = Wpf.Ui.Appearance.ApplicationTheme;

namespace RevitLookup.ViewModels.Settings;

/// <summary>
///     Represents the view model for the Settings view, applying appearance and behavior changes to the running application.
/// </summary>
[UsedImplicitly]
public sealed partial class SettingsViewModel : ObservableObject, ISettingsViewModel
{
    private readonly bool _initialized;
    private readonly IWindowIntercomService _intercomService;
    private readonly INavigationService _navigationService;
    private readonly INotificationService _notificationService;
    private readonly RevitRibbonService _ribbonService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISettingsService _settingsService;
    private readonly IThemeWatcherService _themeWatcherService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SettingsViewModel" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve the reset settings dialog.</param>
    /// <param name="navigationService">The service used to access the navigation control's transition setting.</param>
    /// <param name="notificationService">The service used to report settings reset failures.</param>
    /// <param name="settingsService">The service that persists application settings.</param>
    /// <param name="intercomService">The service used to reach the hosting window when size tracking changes.</param>
    /// <param name="themeWatcherService">The service that applies and watches the current theme.</param>
    /// <param name="ribbonService">The service that rebuilds the Revit ribbon when the Modify tab setting changes.</param>
    public SettingsViewModel(
        IServiceProvider serviceProvider,
        INavigationService navigationService,
        INotificationService notificationService,
        ISettingsService settingsService,
        IWindowIntercomService intercomService,
        IThemeWatcherService themeWatcherService,
        RevitRibbonService ribbonService)
    {
        _serviceProvider = serviceProvider;
        _navigationService = navigationService;
        _notificationService = notificationService;
        _settingsService = settingsService;
        _intercomService = intercomService;
        _themeWatcherService = themeWatcherService;
        _ribbonService = ribbonService;

        ApplySettings();
        _initialized = true;
    }

    /// <inheritdoc />
    [ObservableProperty]
    public partial ApplicationTheme Theme { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial WindowBackdropType Background { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool UseTransition { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool UseHardwareRendering { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool UseSizeRestoring { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool UseModifyTab { get; set; }

    /// <inheritdoc />
    public List<ApplicationTheme> Themes { get; } =
    [
#if REVIT2024_OR_GREATER
        ApplicationTheme.Auto,
#endif
        ApplicationTheme.Light,
        ApplicationTheme.Dark,
        ApplicationTheme.HighContrast
    ];

    /// <inheritdoc />
    public List<WindowBackdropType> BackgroundEffects { get; } =
    [
        WindowBackdropType.None,
        WindowBackdropType.Acrylic,
        WindowBackdropType.Tabbed,
        WindowBackdropType.Mica
    ];

    [RelayCommand]
    private async Task ResetSettingsAsync()
    {
        try
        {
            var dialog = _serviceProvider.GetRequiredService<ResetSettingsDialog>();
            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            if (dialog.CanResetApplicationSettings)
            {
                _settingsService.ResetApplicationSettings();
            }

            if (dialog.CanResetDecompositionSettings)
            {
                _settingsService.ResetDecompositionSettings();
            }

            if (dialog.CanResetVisualizationSettings)
            {
                _settingsService.ResetVisualizationSettings();
            }

            ApplySettings();

            _notificationService.ShowSuccess("Reset settings", "Settings successfully reset to default");
        }
        catch (Exception exception)
        {
            _notificationService.ShowError("Reset settings error", exception);
        }
    }

    partial void OnThemeChanged(ApplicationTheme value)
    {
        if (!_initialized)
        {
            return;
        }

        _settingsService.ApplicationSettings.Theme = value;
        _themeWatcherService.ApplyTheme();
    }

    partial void OnThemeChanged(ApplicationTheme oldValue, ApplicationTheme newValue)
    {
        if (!_initialized)
        {
            return;
        }

        if (oldValue == ApplicationTheme.Auto)
        {
            _themeWatcherService.Unwatch();
        }
    }

    partial void OnBackgroundChanged(WindowBackdropType value)
    {
        if (!_initialized)
        {
            return;
        }

        _settingsService.ApplicationSettings.Background = value;
        _themeWatcherService.ApplyTheme();
    }

    partial void OnUseTransitionChanged(bool value)
    {
        if (!_initialized)
        {
            return;
        }

        var navigationControl = _navigationService.GetNavigationControl();
        var transition = _settingsService.ApplicationSettings.Transition = value
            ? (Transition)NavigationView.TransitionProperty.DefaultMetadata.DefaultValue
            : Transition.None;

        _settingsService.ApplicationSettings.Transition = transition;
        navigationControl.Transition = transition;
    }

    partial void OnUseHardwareRenderingChanged(bool value)
    {
        if (!_initialized)
        {
            return;
        }

        _settingsService.ApplicationSettings.UseHardwareRendering = value;
        if (value)
        {
            Application.EnableHardwareRendering();
        }
        else
        {
            Application.DisableHardwareRendering();
        }
    }

    partial void OnUseSizeRestoringChanged(bool value)
    {
        if (!_initialized)
        {
            return;
        }

        _settingsService.ApplicationSettings.UseSizeRestoring = value;
        if (_intercomService.GetHost() is not RevitLookupView lookupView)
        {
            Debug.Fail("Settings page running inside invalid host");
            return;
        }

        if (value)
        {
            lookupView.EnableSizeTracking();
        }
        else
        {
            lookupView.DisableSizeTracking();
        }
    }

    partial void OnUseModifyTabChanged(bool value)
    {
        if (!_initialized)
        {
            return;
        }

        _settingsService.ApplicationSettings.UseModifyTab = value;
        _ribbonService.CreateRibbon();
    }

    private void ApplySettings()
    {
        Theme = _settingsService.ApplicationSettings.Theme;
        Background = _settingsService.ApplicationSettings.Background;
        UseTransition = _settingsService.ApplicationSettings.Transition != Transition.None;
        UseHardwareRendering = _settingsService.ApplicationSettings.UseHardwareRendering;
        UseSizeRestoring = _settingsService.ApplicationSettings.UseSizeRestoring;
        UseModifyTab = _settingsService.ApplicationSettings.UseModifyTab;
    }
}
