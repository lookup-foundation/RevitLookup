using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;

namespace RevitLookup.UI.Playground.ViewModels.Pages.BasicInput;

[UsedImplicitly]
public sealed partial class ComboBoxViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsStandardComboBoxEnabled { get; set; } = true;
    
    public ObservableCollection<string> Items { get; } =
    [
        "Item 1",
        "Item 2",
        "Item 3",
        "Item 4",
        "Item 5"
    ];
}