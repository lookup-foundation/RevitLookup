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
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RevitLookup.UI.Framework.Presentation;

/// <summary>
///     Represents a service that displays notifications as snackbar messages on the host window.
/// </summary>
/// <remarks>
///     Queues a notification until the host window has finished loading if it raised before then.
/// </remarks>
public sealed class NotificationService(ISnackbarService snackbarService, IWindowIntercomService intercomService) : INotificationService
{
    private Action? _pendingNotifications;

    /// <inheritdoc />
    public void ShowSuccess(string title, string message)
    {
        if (intercomService.Dispatcher.CheckAccess())
        {
            PushSuccessMessage(title, message);
        }
        else
        {
            intercomService.Dispatcher.Invoke(() => PushSuccessMessage(title, message));
        }
    }

    /// <inheritdoc />
    public void ShowWarning(string title, string message)
    {
        if (intercomService.Dispatcher.CheckAccess())
        {
            PushWarningMessage(title, message);
        }
        else
        {
            intercomService.Dispatcher.Invoke(() => PushWarningMessage(title, message));
        }
    }

    /// <inheritdoc />
    public void ShowError(string title, string message)
    {
        if (intercomService.Dispatcher.CheckAccess())
        {
            PushErrorMessage(title, message);
        }
        else
        {
            intercomService.Dispatcher.Invoke(() => PushErrorMessage(title, message));
        }
    }

    /// <inheritdoc />
    public void ShowError(string title, Exception exception)
    {
        if (intercomService.Dispatcher.CheckAccess())
        {
            PushErrorMessage(title, exception.Message);
        }
        else
        {
            intercomService.Dispatcher.Invoke(() => PushErrorMessage(title, exception.Message));
        }
    }

    private void PushSuccessMessage(string title, string message)
    {
        var host = intercomService.GetHost();
        if (!host.IsLoaded)
        {
            if (_pendingNotifications is null)
            {
                host.Loaded += ShowPendingNotifications;
            }

            _pendingNotifications += () => ShowSuccessBar(title, message);
        }
        else
        {
            ShowSuccessBar(title, message);
        }
    }

    private void PushWarningMessage(string title, string message)
    {
        var host = intercomService.GetHost();
        if (!host.IsLoaded)
        {
            if (_pendingNotifications is null)
            {
                host.Loaded += ShowPendingNotifications;
            }

            _pendingNotifications += () => ShowWarningBar(title, message);
        }
        else
        {
            ShowWarningBar(title, message);
        }
    }

    private void PushErrorMessage(string title, string message)
    {
        var host = intercomService.GetHost();
        if (!host.IsLoaded)
        {
            if (_pendingNotifications is null)
            {
                host.Loaded += ShowPendingNotifications;
            }

            _pendingNotifications += () => ShowErrorBar(title, message);
        }
        else
        {
            ShowErrorBar(title, message);
        }
    }

    private void ShowSuccessBar(string title, string message)
    {
        snackbarService.Show(
            title,
            message,
            ControlAppearance.Success,
            new SymbolIcon(SymbolRegular.ChatWarning24, 24),
            snackbarService.DefaultTimeOut);
    }

    private void ShowWarningBar(string title, string message)
    {
        snackbarService.Show(
            title,
            message,
            ControlAppearance.Caution,
            new SymbolIcon(SymbolRegular.Warning24, 24),
            snackbarService.DefaultTimeOut);
    }

    private void ShowErrorBar(string title, string message)
    {
        snackbarService.Show(
            title,
            message,
            ControlAppearance.Danger,
            new SymbolIcon(SymbolRegular.ErrorCircle24, 24),
            snackbarService.DefaultTimeOut);
    }

    private void ShowPendingNotifications(object sender, RoutedEventArgs args)
    {
        var host = intercomService.GetHost();
        host.Loaded -= ShowPendingNotifications;
        if (_pendingNotifications is null)
        {
            return;
        }

        _pendingNotifications.Invoke();
        _pendingNotifications = null;
    }
}
