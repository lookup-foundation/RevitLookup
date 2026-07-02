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

using System.Windows.Input;
using Microsoft.Extensions.Logging;
using RevitLookup.UI.Framework.Views.Windows;
using Wpf.Ui.Abstractions.Controls;

namespace RevitLookup.UI.Framework.Views.Decomposition;

public partial class SummaryViewBase : INavigationAware
{
    /// <summary>
    ///     Callback when navigating to the current page
    /// </summary>
    public Task OnNavigatedToAsync()
    {
        var host = _intercomService.GetHost();
        host.PreviewKeyDown += OnPageKeyPressed;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Callback when navigating from this page
    /// </summary>
    public Task OnNavigatedFromAsync()
    {
        var host = _intercomService.GetHost();
        host.PreviewKeyDown -= OnPageKeyPressed;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Page shortcuts
    /// </summary>
    private void OnPageKeyPressed(object sender, KeyEventArgs args)
    {
        HandleRefreshShortcut(args);
        if (args.Handled) return;

        HandleFocusSearchShortcut(sender, args);
    }

    private async void HandleRefreshShortcut(KeyEventArgs args)
    {
        if (args.Key != Key.F5) return;

        args.Handled = true;

        try
        {
            await ViewModel.RefreshMembersAsync();
        }
        catch (Exception exception)
        {
            LogRefreshMembersFailed(_logger, exception);
            _notificationService.ShowError("Failed to refresh members", exception);
        }
    }

    private void HandleFocusSearchShortcut(object sender, KeyEventArgs args)
    {
        if (SearchBoxControl.IsKeyboardFocused) return;
        if (args.KeyboardDevice.Modifiers != ModifierKeys.None) return;

        var rootWindow = (RevitLookupView) sender;
        if (rootWindow.DialogHost.Content is not null) return;

        if (args.Key is >= Key.D0 and <= Key.Z or >= Key.NumPad0 and <= Key.NumPad9)
        {
            SearchBoxControl.Focus();
        }
    }

    [LoggerMessage(LogLevel.Error, "Failed to refresh members")]
    private static partial void LogRefreshMembersFailed(ILogger<SummaryViewBase> logger, Exception exception);
}