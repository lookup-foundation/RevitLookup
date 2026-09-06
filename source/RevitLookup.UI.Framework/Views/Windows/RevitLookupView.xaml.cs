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

using System.Windows;
using System.Windows.Automation.Peers;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.Settings;
using RevitLookup.Abstractions.Updater;
using RevitLookup.UI.Framework.Controls.Automation;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace RevitLookup.UI.Framework.Views.Windows;

/// <summary>
///     Represents a window that hosts the RevitLookup navigation, dialogs, and snackbar presenter.
/// </summary>
public sealed partial class RevitLookupView
{
    private readonly IWindowIntercomService _intercomService;
    private readonly ISettingsService _settingsService;
    private readonly IThemeWatcherService _themeWatcherService;
    private readonly ISoftwareUpdateService _updateService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RevitLookupView" /> class.
    /// </summary>
    /// <param name="navigationService">The service bound to this window's navigation control.</param>
    /// <param name="dialogService">The service bound to this window's dialog host.</param>
    /// <param name="snackbarService">The service bound to this window's snackbar presenter.</param>
    /// <param name="intercomService">The service that registers this window as the shared RevitLookup host.</param>
    /// <param name="updateService">The service that reports whether a new application version is available.</param>
    /// <param name="settingsService">The service that provides the application display and size settings.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this window.</param>
    public RevitLookupView(
        INavigationService navigationService,
        IContentDialogService dialogService,
        ISnackbarService snackbarService,
        IWindowIntercomService intercomService,
        ISoftwareUpdateService updateService,
        ISettingsService settingsService,
        IThemeWatcherService themeWatcherService)
    {
        _intercomService = intercomService;
        _updateService = updateService;
        _settingsService = settingsService;
        _themeWatcherService = themeWatcherService;

        themeWatcherService.Watch(this);
        InitializeComponent();

        intercomService.SetSharedHost(this);
        navigationService.SetNavigationControl(RootNavigation);
        dialogService.SetDialogHost(DialogHost);
        snackbarService.SetSnackbarPresenter(RootSnackbar);

        ApplyEffects();
        AddShortcuts();
        AddBadges();
        ApplyWindowSize();
        FixComponentsTheme();
    }

    private void AddBadges()
    {
        if (_updateService.NewVersion is null)
        {
            return;
        }

        if (_updateService.LocalFilePath is not null)
        {
            return;
        }

        UpdatesNotifier.Visibility = Visibility.Visible;
    }

    private void ApplyEffects()
    {
        WindowBackdropType = _settingsService.ApplicationSettings.Background;
        RootNavigation.Transition = _settingsService.ApplicationSettings.Transition;
        WindowBackgroundManager.UpdateBackground(this, _settingsService.ApplicationSettings.Theme, WindowBackdropType);
    }

    private void ApplyWindowSize()
    {
        if (!_settingsService.ApplicationSettings.UseSizeRestoring)
        {
            return;
        }

        if (_settingsService.ApplicationSettings.WindowWidth >= MinWidth)
        {
            Width = _settingsService.ApplicationSettings.WindowWidth;
        }

        if (_settingsService.ApplicationSettings.WindowHeight >= MinHeight)
        {
            Height = _settingsService.ApplicationSettings.WindowHeight;
        }

        EnableSizeTracking();
    }

    /// <summary>
    ///     Starts persisting this window's size to the application settings as it changes.
    /// </summary>
    public void EnableSizeTracking()
    {
        SizeChanged += OnSizeChanged;
    }

    /// <summary>
    ///     Stops persisting this window's size to the application settings.
    /// </summary>
    public void DisableSizeTracking()
    {
        SizeChanged -= OnSizeChanged;
    }

    private static void OnSizeChanged(object sender, SizeChangedEventArgs args)
    {
        var self = (RevitLookupView)sender;
        self._settingsService.ApplicationSettings.WindowWidth = args.NewSize.Width;
        self._settingsService.ApplicationSettings.WindowHeight = args.NewSize.Height;
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new NoAutomationWindowPeer(this);
    }
}
