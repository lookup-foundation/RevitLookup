﻿using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Tools;

namespace RevitLookup.UI.Playground.Mockups.ViewModels.Tools;

[UsedImplicitly]
public sealed partial class MockSearchElementsViewModel(
    INotificationService notificationService,
    IVisualDecompositionService decompositionService)
    : ObservableObject, ISearchElementsViewModel
{
    [ObservableProperty] private string _searchText = string.Empty;

    public async Task<bool> SearchElementsAsync()
    {
        var result = SearchText != string.Empty;
        if (result)
        {
            await decompositionService.VisualizeDecompositionAsync((object) SearchText);
        }
        else
        {
            notificationService.ShowWarning("Search elements", "There are no elements found for your request");
        }

        return result;
    }
}