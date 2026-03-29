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

using System.Net.Http;
using System.Runtime;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevitLookup.Abstractions.Options;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.Abstractions.ViewModels.AboutProgram;
using RevitLookup.UI.Framework.Views.AboutProgram;

namespace RevitLookup.ViewModels.AboutProgram;

[UsedImplicitly]
public sealed partial class AboutViewModel : ObservableObject, IAboutViewModel
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISoftwareUpdateService _updateService;
    private readonly ILogger<AboutViewModel> _logger;

    public AboutViewModel(
        IServiceProvider serviceProvider,
        ISoftwareUpdateService updateService,
        IOptions<AssemblyOptions> assemblyOptions,
        ILogger<AboutViewModel> logger)
    {
        _serviceProvider = serviceProvider;
        _updateService = updateService;
        _logger = logger;

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
    
    [ObservableProperty]
    public partial SoftwareUpdateState State { get; set; } = (SoftwareUpdateState) (-1);

    [ObservableProperty]
    public partial Version? CurrentVersion { get; set; }

    [ObservableProperty]
    public partial string? NewVersion { get; set; }

    [ObservableProperty]
    public partial string? ReleaseNotesUrl { get; set; }

    [ObservableProperty]
    public partial string? LatestCheckDate { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    [ObservableProperty]
    public partial string? Runtime { get; set; }

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
        catch (HttpRequestException exception)
        {
            State = SoftwareUpdateState.UpToDate;
            _logger.LogError(exception, "Checking updates fail");
        }
        catch (Exception exception)
        {
            State = SoftwareUpdateState.Error;
            ErrorMessage = "An unknown error occurred while checking for updates";
            _logger.LogError(exception, "Checking updates fail");
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
            await _updateService.DownloadUpdate();
            State = SoftwareUpdateState.ReadyToInstall;
        }
        catch (Exception exception)
        {
            State = SoftwareUpdateState.Error;
            ErrorMessage = "An error occurred while downloading the update";
            _logger.LogError(exception, "Downloading updates fail");
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

        if (_updateService.NewVersion is null) return;

        NewVersion = _updateService.NewVersion;
        ReleaseNotesUrl = _updateService.ReleaseNotesUrl;
        State = SoftwareUpdateState.ReadyToDownload;
    }
}