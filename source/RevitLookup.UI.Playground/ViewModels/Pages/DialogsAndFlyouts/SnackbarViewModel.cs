using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RevitLookup.UI.Playground.ViewModels.Pages.DialogsAndFlyouts;

[UsedImplicitly]
public partial class SnackbarViewModel(ISnackbarService snackbarService) : ObservableObject
{
    private ControlAppearance _snackbarAppearance = ControlAppearance.Secondary;
    private IconElement _icon = new SymbolIcon { Symbol = SymbolRegular.Info24, FontSize = 24 };

    [ObservableProperty]
    public partial int SnackbarTimeout { get; set; } = 2;

    [ObservableProperty]
    public partial int SnackbarAppearanceComboBoxSelectedIndex { get; set; } = 1;

    [RelayCommand]
    private void OnOpenSnackbar(object? sender)
    {
        snackbarService.Show(
            "Notification",
            "This is a sample notification message.",
            _snackbarAppearance,
            _icon,
            TimeSpan.FromSeconds(SnackbarTimeout)
        );
    }

    partial void OnSnackbarAppearanceComboBoxSelectedIndexChanged(int value)
    {
        _snackbarAppearance = value switch
        {
            1 => ControlAppearance.Secondary,
            2 => ControlAppearance.Info,
            3 => ControlAppearance.Success,
            4 => ControlAppearance.Caution,
            5 => ControlAppearance.Danger,
            6 => ControlAppearance.Light,
            7 => ControlAppearance.Dark,
            8 => ControlAppearance.Transparent,
            _ => ControlAppearance.Primary,
        };

        _icon = _snackbarAppearance switch
        {
            ControlAppearance.Danger => new SymbolIcon { Symbol = SymbolRegular.ErrorCircle24, FontSize = 24 },
            _ => new SymbolIcon { Symbol = SymbolRegular.Info24, FontSize = 24 }
        };
    }
}