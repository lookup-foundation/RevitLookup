using System.Windows;
using System.Windows.Threading;
using RevitLookup.Abstractions.Presentation;

namespace RevitLookup.UI.Framework.Presentation;

/// <summary>
///     Represents a service that tracks the window hosting the current UI and, optionally, shares it across other open windows.
/// </summary>
public sealed class WindowIntercomService : IWindowIntercomService
{
    private static readonly List<Window> SharedWindows = [];
    private Window? _host;

    /// <inheritdoc />
    public void SetHost(Window host)
    {
        _host = host;
    }

    /// <inheritdoc />
    public void SetSharedHost(Window host)
    {
        SetHost(host);
        SharedWindows.Add(host);
        host.Closed += OnHostDisconnected;
    }

    /// <inheritdoc />
    public List<Window> OpenedWindows => SharedWindows;

    /// <inheritdoc />
    [Pure]
    public Window GetHost()
    {
        if (_host is null)
        {
            throw new InvalidOperationException("The Host was never set.");
        }

        return _host;
    }

    /// <inheritdoc />
    public Dispatcher Dispatcher
    {
        get
        {
            if (_host is null)
            {
                throw new InvalidOperationException("The Host was never set.");
            }

            return _host.Dispatcher;
        }
    }

    private static void OnHostDisconnected(object? sender, EventArgs args)
    {
        var self = (Window)sender!;
        self.Closed -= OnHostDisconnected;

        SharedWindows.Remove(self);
    }
}
