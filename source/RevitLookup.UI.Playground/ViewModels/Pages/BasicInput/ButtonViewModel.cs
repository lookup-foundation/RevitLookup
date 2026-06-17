using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitLookup.UI.Playground.ViewModels.Pages.BasicInput;

[UsedImplicitly]
public partial class ButtonViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsStandardButtonEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool IsPrimaryButtonEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool IsSecondaryButtonEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool IsDangerButtonEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool IsTransparentButtonEnabled { get; set; } = true;
}