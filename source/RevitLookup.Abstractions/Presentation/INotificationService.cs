namespace RevitLookup.Abstractions.Presentation;

/// <summary>
///     Defines a contract that displays notifications to the user.
/// </summary>
public interface INotificationService
{
    /// <summary>
    ///     Shows a success notification.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The notification message.</param>
    void ShowSuccess(string title, string message);

    /// <summary>
    ///     Shows a warning notification.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The notification message.</param>
    void ShowWarning(string title, string message);

    /// <summary>
    ///     Shows an error notification.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The notification message.</param>
    void ShowError(string title, string message);

    /// <summary>
    ///     Shows an error notification for <paramref name="exception" />.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="exception">The exception whose message is displayed.</param>
    void ShowError(string title, Exception exception);
}
