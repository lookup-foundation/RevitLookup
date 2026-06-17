using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitLookup.UI.Playground.ViewModels.Pages.BasicInput;

[UsedImplicitly]
public sealed partial class RadioButtonViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsStandardRadioButtonEnabled { get; set; } = true;
}