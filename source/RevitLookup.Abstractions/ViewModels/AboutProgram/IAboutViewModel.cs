using CommunityToolkit.Mvvm.Input;
using RevitLookup.Abstractions.Services.Settings;

namespace RevitLookup.Abstractions.ViewModels.AboutProgram;

/// <summary>
///     Represents the data for the About view.
/// </summary>
public interface IAboutViewModel
{
    /// <summary>
    ///     The application update state.
    /// </summary>
    SoftwareUpdateState State { get; set; }
    
    /// <summary>
    ///     The current version of the application.
    /// </summary>
    Version CurrentVersion { get; set; }
    
    /// <summary>
    ///     A new available version to download.
    /// </summary>
    string NewVersion { get; set; }
    
    /// <summary>
    ///     The error message during updating.
    /// </summary>
    string ErrorMessage { get; set; }
    
    /// <summary>
    ///     The URL to the release notes of the new version.
    /// </summary>
    string ReleaseNotesUrl { get; set; }
    
    /// <summary>
    ///     The date of the latest check for updates.
    /// </summary>
    string LatestCheckDate { get; set; }
    
    /// <summary>
    ///     The current .NET version.
    /// </summary>
    string Runtime { get; set; }

    /// <summary>
    ///     Check for updates on the server.
    /// </summary>
    IAsyncRelayCommand CheckUpdatesCommand { get; }
    
    /// <summary>
    ///     Download the update from the server.
    /// </summary>
    IAsyncRelayCommand DownloadUpdateCommand { get; }
    
    /// <summary>
    ///     Show the third-party software dialog.
    /// </summary>
    IAsyncRelayCommand ShowSoftwareDialogCommand { get; }
}