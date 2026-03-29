using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;

namespace RevitLookup.UI.Playground.ViewModels.Pages.BasicInput;

[UsedImplicitly]
public sealed partial class HyperlinkButtonViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsStandardButtonEnabled { get; set; } = true;
}