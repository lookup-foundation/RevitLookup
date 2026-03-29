using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using RevitLookup.UI.Playground.Models;
using Wpf.Ui.Controls;
#if NETFRAMEWORK
using RevitLookup.UI.Framework.Extensions;
#endif

namespace RevitLookup.UI.Playground.ViewModels.Pages.DesignGuidance;

[UsedImplicitly]
public partial class SymbolIconsPageViewModel : ObservableObject
{
    public SymbolIconsPageViewModel()
    {
        var symbols = Enum.GetNames(typeof(SymbolRegular));
        Icons = symbols.Select(SymbolGlyph.Parse)
            .Select(symbol => new SymbolIconData
            {
                Name = symbol.ToString(),
                Icon = symbol,
                Code = ((int) symbol).ToString("X4")
            })
            .OrderBy(data => data.Name)
            .ToList();

        SelectedIcon = Icons.FirstOrDefault();
    }
    
    [ObservableProperty]
    private partial List<SymbolIconData> Icons { get; set; }

    [ObservableProperty]
    public partial List<SymbolIconData> FilteredIcons { get; private set; } = [];

    [ObservableProperty]
    public partial SymbolIconData? SelectedIcon { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool UseFilledIcons { get; set; }

    partial void OnIconsChanged(List<SymbolIconData> value)
    {
        FilteredIcons = value;
    }

    async partial void OnSearchTextChanged(string value)
    {
        try
        {
            FilteredIcons = await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return Icons;
                }

                var formattedText = value.Trim();
                var results = new List<SymbolIconData>();

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var setData in Icons)
                {
                    if (setData.Name.Contains(formattedText, StringComparison.OrdinalIgnoreCase))
                    {
                        results.Add(setData);
                    }
                }

                return results;
            });
        }
        catch
        {
            // ignored
        }
    }
}