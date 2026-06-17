using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RevitLookup.UI.Playground.Views.Pages;
using Wpf.Ui;

namespace RevitLookup.UI.Playground.ViewModels.Pages;

[UsedImplicitly]
public sealed partial class DashboardViewModel(INavigationService navigationService) : ObservableObject
{
    [RelayCommand]
    private void NavigateToWindowsPage()
    {
        navigationService.NavigateWithHierarchy(typeof(WindowsPage));
    }

    [RelayCommand]
    private void NavigateToPagesPage()
    {
        navigationService.NavigateWithHierarchy(typeof(PagesPage));
    }

    [RelayCommand]
    private void NavigateToDialogsPage()
    {
        navigationService.NavigateWithHierarchy(typeof(DialogsPage));
    }
}