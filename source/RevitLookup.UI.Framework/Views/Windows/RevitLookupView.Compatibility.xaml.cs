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
using System.Windows.Controls;
using System.Windows.Media;
using RevitLookup.UI.Framework.Utils;
using Wpf.Ui.Controls;

namespace RevitLookup.UI.Framework.Views.Windows;

public sealed partial class RevitLookupView
{
    private void FixComponentsTheme()
    {
        RootNavigation.Loaded += OnNavigationScrollLoaded;
    }

    private void OnNavigationScrollLoaded(object sender, RoutedEventArgs args)
    {
        var contentPresenter = RootNavigation.FindVisualChild<NavigationViewContentPresenter>()!;
        contentPresenter.LoadCompleted += ContentPresenterOnContentRendered;
    }

    private void ContentPresenterOnContentRendered(object? sender, EventArgs e)
    {
        var contentPresenter = (NavigationViewContentPresenter) sender!;
        if (!contentPresenter.IsDynamicScrollViewerEnabled) return;

        if (VisualTreeHelper.GetChildrenCount(contentPresenter) == 0)
        {
            contentPresenter.ApplyTemplate();
        }

        var scrollViewer = (ScrollViewer) VisualTreeHelper.GetChild(contentPresenter, 0);
        _themeWatcherService.Watch(scrollViewer);
    }
}