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

using System.Runtime;
using System.Text;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RevitLookup.Abstractions.Application;
using RevitLookup.Abstractions.Updater;
using RevitLookup.Abstractions.ViewModels.AboutProgram;
using RevitLookup.UI.Framework.Views.AboutProgram;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.AboutProgram;

/// <summary>
///     Represents a Playground mock of <see cref="IAboutViewModel" /> that fabricates update check and download outcomes through <see cref="ISoftwareUpdateService" />.
/// </summary>
[UsedImplicitly]
public sealed partial class MockAboutViewModel : ObservableObject, IAboutViewModel
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISoftwareUpdateService _updateService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MockAboutViewModel" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve the open-source software dialog.</param>
    /// <param name="updateService">The service that checks for, downloads, and reports application updates.</param>
    /// <param name="assemblyOptions">The assembly metadata used to populate the current version and runtime information.</param>
    public MockAboutViewModel(IServiceProvider serviceProvider, ISoftwareUpdateService updateService, IOptions<AssemblyOptions> assemblyOptions)
    {
        _serviceProvider = serviceProvider;
        _updateService = updateService;

        CurrentVersion = assemblyOptions.Value.Version;
        Runtime = new StringBuilder()
            .Append(assemblyOptions.Value.Framework)
            .Append(' ')
            .Append(Environment.Is64BitProcess ? "x64" : "x86")
            .Append(" (")
            .Append(GCSettings.IsServerGC ? "Server" : "Workstation")
            .Append(" GC)")
            .ToString();

        LatestCheckDate = _updateService.LatestCheckDate?.ToString("yyyy.MM.dd HH:mm:ss");
        UpdateSoftwareState();
    }

    /// <inheritdoc />
    [ObservableProperty]
    public partial SoftwareUpdateState State { get; set; } = (SoftwareUpdateState)(-1);

    /// <inheritdoc />
    [ObservableProperty]
    public partial Version CurrentVersion { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial string? NewVersion { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial string? ReleaseNotesUrl { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial string? LatestCheckDate { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial string Runtime { get; set; }

    [RelayCommand]
    private async Task CheckUpdatesAsync()
    {
        try
        {
            var result = await _updateService.CheckUpdatesAsync();

            if (!result)
            {
                State = SoftwareUpdateState.UpToDate;
                return;
            }

            UpdateSoftwareState();
        }
        catch
        {
            State = SoftwareUpdateState.Error;
            ErrorMessage = new Faker().Lorem.Sentence();
        }
        finally
        {
            LatestCheckDate = _updateService.LatestCheckDate?.ToString("yyyy.MM.dd HH:mm:ss");
        }
    }

    [RelayCommand]
    private async Task DownloadUpdateAsync()
    {
        try
        {
            await _updateService.DownloadUpdateAsync();
            State = SoftwareUpdateState.ReadyToInstall;
        }
        catch
        {
            State = SoftwareUpdateState.Error;
            ErrorMessage = new Faker().Lorem.Sentence();
        }
    }

    [RelayCommand]
    private async Task ShowSoftwareDialogAsync()
    {
        var dialog = _serviceProvider.GetRequiredService<OpenSourceDialog>();
        await dialog.ShowAsync();
    }

    private void UpdateSoftwareState()
    {
        if (_updateService.LocalFilePath is not null)
        {
            State = SoftwareUpdateState.ReadyToInstall;
            return;
        }

        if (_updateService.NewVersion is null)
        {
            return;
        }

        NewVersion = _updateService.NewVersion;
        ReleaseNotesUrl = _updateService.ReleaseNotesUrl;
        State = SoftwareUpdateState.ReadyToDownload;
    }
}
