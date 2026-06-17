using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitLookup.UI.Playground.ViewModels.Pages.BasicInput;

[UsedImplicitly]
public sealed partial class ToggleButtonViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsStandardToggleButtonEnabled { get; set; } = true;
}