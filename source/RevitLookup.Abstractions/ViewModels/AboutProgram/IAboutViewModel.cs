using CommunityToolkit.Mvvm.Input;
using RevitLookup.Abstractions.Updater;

namespace RevitLookup.Abstractions.ViewModels.AboutProgram;

/// <summary>
///     Defines a contract that represents the data for the About view.
/// </summary>
public interface IAboutViewModel
{
    /// <summary>
    ///     Gets or sets the application update state.
    /// </summary>
    SoftwareUpdateState State { get; set; }

    /// <summary>
    ///     Gets or sets the current version of the application.
    /// </summary>
    Version CurrentVersion { get; set; }

    /// <summary>
    ///     Gets or sets the new version available to download, or <see langword="null" /> when no update is available.
    /// </summary>
    string? NewVersion { get; set; }

    /// <summary>
    ///     Gets or sets the error message produced while checking for or downloading an update, or <see langword="null" /> when no error occurred.
    /// </summary>
    string? ErrorMessage { get; set; }

    /// <summary>
    ///     Gets or sets the URL to the release notes of the new version, or <see langword="null" /> when no update is available.
    /// </summary>
    string? ReleaseNotesUrl { get; set; }

    /// <summary>
    ///     Gets or sets the date of the latest check for updates, or <see langword="null" /> when no check has been performed yet.
    /// </summary>
    string? LatestCheckDate { get; set; }

    /// <summary>
    ///     Gets or sets the current .NET runtime version.
    /// </summary>
    string Runtime { get; set; }

    /// <summary>
    ///     Gets the command that checks for updates on the server.
    /// </summary>
    IAsyncRelayCommand CheckUpdatesCommand { get; }

    /// <summary>
    ///     Gets the command that downloads the update from the server.
    /// </summary>
    IAsyncRelayCommand DownloadUpdateCommand { get; }

    /// <summary>
    ///     Gets the command that shows the third-party software dialog.
    /// </summary>
    IAsyncRelayCommand ShowSoftwareDialogCommand { get; }
}
