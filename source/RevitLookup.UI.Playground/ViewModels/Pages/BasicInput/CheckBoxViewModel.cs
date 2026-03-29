using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;

namespace RevitLookup.UI.Playground.ViewModels.Pages.BasicInput;

[UsedImplicitly]
public sealed partial class CheckBoxViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsStandardCheckBoxEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool IsThreeStateCheckBoxEnabled { get; set; } = true;
}