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
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.Settings;
using Wpf.Ui.Appearance;

namespace RevitLookup.UI.Playground.Mocks.Presentation;

/// <summary>
///     Represents a Playground mock of <see cref="IThemeWatcherService" /> that applies the Playground settings theme directly through Wpf.Ui.
/// </summary>
/// <param name="settingsService">The service supplying the theme and backdrop applied to watched elements.</param>
public sealed class MockThemeWatcherService(ISettingsService settingsService) : IThemeWatcherService
{
    private readonly List<FrameworkElement> _observedElements = [];

    /// <inheritdoc />
    public void Initialize()
    {
    }

    /// <inheritdoc />
    public void ApplyTheme()
    {
        var theme = settingsService.ApplicationSettings.Theme;
        ApplicationThemeManager.Apply(theme, settingsService.ApplicationSettings.Background);
        UpdateBackground(theme);
    }

    /// <inheritdoc />
    public void Watch(FrameworkElement frameworkElement)
    {
        frameworkElement.Loaded -= OnWatchedElementLoaded;
        frameworkElement.Loaded += OnWatchedElementLoaded;
        frameworkElement.Unloaded -= OnWatchedElementUnloaded;
        frameworkElement.Unloaded += OnWatchedElementUnloaded;
    }

    /// <inheritdoc />
    public void Unwatch()
    {
    }

    private void OnWatchedElementLoaded(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        _observedElements.Add(element);
    }

    private void OnWatchedElementUnloaded(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        _observedElements.Remove(element);
    }

    private void UpdateBackground(ApplicationTheme theme)
    {
        foreach (var window in _observedElements.Select(Window.GetWindow).Distinct())
        {
            WindowBackgroundManager.UpdateBackground(window, theme, settingsService.ApplicationSettings.Background);
        }
    }
}
