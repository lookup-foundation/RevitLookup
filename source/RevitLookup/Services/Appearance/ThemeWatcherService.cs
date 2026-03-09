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
using RevitLookup.Abstractions.Services.Appearance;
using RevitLookup.Abstractions.Services.Settings;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Color = System.Windows.Media.Color;
#if REVIT2024_OR_GREATER
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
#endif

namespace RevitLookup.Services.Appearance;

public sealed partial class ThemeWatcherService(ISettingsService settingsService) : IThemeWatcherService
{
#if REVIT2024_OR_GREATER
    private bool _isWatching;
#endif

    private readonly List<FrameworkElement> _observedElements = [];

    public void Initialize()
    {
        UiApplication.Current.Resources = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/RevitLookup;component/Styles/App.Resources.xaml", UriKind.Absolute)
        };

        ApplicationThemeManager.Changed += OnApplicationThemeManagerChanged;
    }

    public void ApplyTheme()
    {
        var theme = settingsService.ApplicationSettings.Theme;
#if REVIT2024_OR_GREATER
        if (settingsService.ApplicationSettings.Theme == ApplicationTheme.Auto)
        {
            theme = GetRevitTheme();

            if (!_isWatching)
            {
                TrackThemeChangesEvent.Raise();
                _isWatching = true;
            }
        }
#endif
        ApplicationThemeManager.Apply(theme, settingsService.ApplicationSettings.Background);
        UpdateBackground(theme);
    }

    public void Watch(FrameworkElement frameworkElement)
    {
        ApplicationThemeManager.Apply(frameworkElement);
        frameworkElement.Loaded += OnWatchedElementLoaded;
        frameworkElement.Unloaded += OnWatchedElementUnloaded;
    }

    public void Unwatch()
    {
#if REVIT2024_OR_GREATER
        if (!_isWatching) return;

        UntrackThemeChangesEvent.Raise();
        _isWatching = false;
#endif
    }
#if REVIT2024_OR_GREATER

    [ExternalEvent(AllowDirectInvocation = true)]
    private void TrackThemeChanges(UIApplication application)
    {
        application.ThemeChanged += OnRevitThemeChanged;
    }
    
    [ExternalEvent(AllowDirectInvocation = true)]
    private void UntrackThemeChanges(UIApplication application)
    {
        application.ThemeChanged -= OnRevitThemeChanged;
    }
    
    private static ApplicationTheme GetRevitTheme()
    {
        return UIThemeManager.CurrentTheme switch
        {
            UITheme.Light => ApplicationTheme.Light,
            UITheme.Dark => ApplicationTheme.Dark,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void OnRevitThemeChanged(object? sender, ThemeChangedEventArgs args)
    {
        if (args.ThemeChangedType != ThemeType.UITheme) return;

        if (_observedElements.Count > 0)
        {
            _observedElements[0].Dispatcher.Invoke(ApplyTheme);
        }
    }
#endif

    private void OnApplicationThemeManagerChanged(ApplicationTheme applicationTheme, Color accent)
    {
        foreach (var frameworkElement in _observedElements)
        {
            ApplicationThemeManager.Apply(frameworkElement);
            UpdateDictionary(frameworkElement);
        }
    }

    private void OnWatchedElementLoaded(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement) sender;
        _observedElements.Add(element);

        if (element.Resources.MergedDictionaries[0].Source.OriginalString != UiApplication.Current.Resources.MergedDictionaries[0].Source.OriginalString)
        {
            ApplicationThemeManager.Apply(element);
            UpdateDictionary(element);
        }
    }

    private void OnWatchedElementUnloaded(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement) sender;
        _observedElements.Remove(element);
    }

    private static void UpdateDictionary(FrameworkElement frameworkElement)
    {
        var themedResources = frameworkElement.Resources.MergedDictionaries
            .Where(dictionary => dictionary.Source.OriginalString.Contains("LookupEngine.UI;", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        frameworkElement.Resources.MergedDictionaries.Insert(0, UiApplication.Current.Resources.MergedDictionaries[0]);
        frameworkElement.Resources.MergedDictionaries.Insert(1, UiApplication.Current.Resources.MergedDictionaries[1]);

        foreach (var themedResource in themedResources)
        {
            frameworkElement.Resources.MergedDictionaries.Remove(themedResource);
        }
    }

    private void UpdateBackground(ApplicationTheme theme)
    {
        foreach (var window in _observedElements.Select(Window.GetWindow).Distinct())
        {
            WindowBackgroundManager.UpdateBackground(window, theme, settingsService.ApplicationSettings.Background);
        }
    }
}