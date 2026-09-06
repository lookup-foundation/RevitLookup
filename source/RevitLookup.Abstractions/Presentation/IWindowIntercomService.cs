using System.Windows;
using System.Windows.Threading;

namespace RevitLookup.Abstractions.Presentation;

/// <summary>
///     Defines a contract that manages the lifecycle of RevitLookup window instances.
/// </summary>
public interface IWindowIntercomService
{
    /// <summary>
    ///     Gets the dispatcher for the UI thread.
    /// </summary>
    Dispatcher Dispatcher { get; }

    /// <summary>
    ///     Gets all opened shared window instances.
    /// </summary>
    List<Window> OpenedWindows { get; }

    /// <summary>
    ///     Gets the current window host.
    /// </summary>
    /// <returns>The current window host.</returns>
    Window GetHost();

    /// <summary>
    ///     Sets the private window host.
    /// </summary>
    /// <param name="host">The window to set as the private host.</param>
    /// <remarks>
    ///     The window set through this method is not added to <see cref="OpenedWindows" />.
    /// </remarks>
    void SetHost(Window host);

    /// <summary>
    ///     Sets the shared window host.
    /// </summary>
    /// <param name="host">The window to set as the shared host.</param>
    /// <remarks>
    ///     The window set through this method is added to <see cref="OpenedWindows" />.
    /// </remarks>
    void SetSharedHost(Window host);
}
