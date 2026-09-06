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
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.ViewModels.Tools;
using RevitLookup.UI.Framework.Views.Decomposition;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RevitLookup.UI.Framework.Views.Tools;

/// <summary>
///     Represents a dialog that searches the active document for elements matching a query and navigates to the results.
/// </summary>
public sealed partial class SearchElementsDialog
{
    private readonly ILogger<SearchElementsDialog> _logger;
    private readonly INavigationService _navigationService;
    private readonly INotificationService _notificationService;
    private readonly ISearchElementsViewModel _viewModel;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SearchElementsDialog" /> class.
    /// </summary>
    /// <param name="dialogService">The service that supplies the dialog host this dialog is shown on.</param>
    /// <param name="viewModel">The view model that provides the data for the Search Elements view.</param>
    /// <param name="navigationService">The service used to navigate to the decomposition summary after a successful search.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this dialog.</param>
    /// <param name="notificationService">The service used to notify the user of errors encountered while searching.</param>
    /// <param name="logger">The logger used to record search failures.</param>
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

    /// <inheritdoc />
    /// <remarks>
    ///     Keeps the dialog open when the primary button is clicked and the search finds no matching elements.
    /// </remarks>
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
