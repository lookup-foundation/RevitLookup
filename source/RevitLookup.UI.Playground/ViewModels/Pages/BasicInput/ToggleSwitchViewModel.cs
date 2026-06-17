using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitLookup.UI.Playground.ViewModels.Pages.BasicInput;

[UsedImplicitly]
public sealed partial class ToggleSwitchViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsStandardToggleSwitchEnabled { get; set; } = true;
}