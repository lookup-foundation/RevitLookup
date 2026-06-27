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

using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Services.Appearance;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Tools;
using RevitLookup.UI.Framework.Views.Decomposition;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RevitLookup.UI.Framework.Views.Tools;

public sealed partial class SearchElementsDialog
{
    private readonly ISearchElementsViewModel _viewModel;
    private readonly INavigationService _navigationService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SearchElementsDialog> _logger;

    public SearchElementsDialog(
        IContentDialogService dialogService,
        ISearchElementsViewModel viewModel,
        INavigationService navigationService,
        IThemeWatcherService themeWatcherService,
        INotificationService notificationService,
        ILogger<SearchElementsDialog> logger)
        : base(dialogService.GetDialogHostEx())
    {
        _viewModel = viewModel;
        _navigationService = navigationService;
        _notificationService = notificationService;
        _logger = logger;

        DataContext = viewModel;
        InitializeComponent();

        themeWatcherService.Watch(this);
    }

    protected override async void OnButtonClick(ContentDialogButton button)
    {
        try
        {
            if (button == ContentDialogButton.Primary)
            {
                var success = await _viewModel.SearchElementsAsync();
                if (!success)
                {
                    return;
                }

                _navigationService.Navigate(typeof(DecompositionSummaryPage));
            }

            base.OnButtonClick(button);
        }
        catch (Exception exception)
        {
            LogSearchElementsFailed(_logger, exception);
            _notificationService.ShowError("Search error", exception.Message);
        }
    }

    [LoggerMessage(LogLevel.Error, "Error while searching elements")]
    private static partial void LogSearchElementsFailed(ILogger<SearchElementsDialog> logger, Exception exception);
}