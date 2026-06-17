using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RevitLookup.UI.Playground.ViewModels.Pages.DialogsAndFlyouts;

[UsedImplicitly]
public partial class FlyoutViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsStandardFlyoutOpen { get; set; }

    [ObservableProperty]
    public partial bool IsRightFlyoutOpen { get; set; }

    [RelayCommand]
    private void OnStandardButtonClick()
    {
        if (!IsStandardFlyoutOpen)
        {
            IsStandardFlyoutOpen = true;
        }
    }

    [RelayCommand]
    private void OnRightButtonClick()
    {
        if (!IsRightFlyoutOpen)
        {
            IsRightFlyoutOpen = true;
        }
    }
}