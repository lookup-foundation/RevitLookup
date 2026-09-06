using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.UI.Playground.SampleData;
using Wpf.Ui.Controls;
#if NETFRAMEWORK
using RevitLookup.UI.Framework.Menus;
#endif

namespace RevitLookup.UI.Playground.ViewModels.Pages.DesignGuidance;

/// <summary>
///     Represents the sample data for the Fluent icon gallery page.
/// </summary>
[UsedImplicitly]
public partial class SymbolIconsPageViewModel : ObservableObject
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SymbolIconsPageViewModel" /> class.
    /// </summary>
    public SymbolIconsPageViewModel()
    {
        var symbols = Enum.GetNames(typeof(SymbolRegular));
        Icons = symbols.Select(SymbolGlyph.Parse)
            .Select(symbol => new SymbolIconData
            {
                Name = symbol.ToString(),
                Icon = symbol,
                Code = ((int)symbol).ToString("X4")
            })
            .OrderBy(data => data.Name)
            .ToList();

        SelectedIcon = Icons.FirstOrDefault();
    }

    [ObservableProperty] private partial List<SymbolIconData> Icons { get; set; }

    /// <summary>
    ///     Gets the icons that match the current <see cref="SearchText" /> filter.
    /// </summary>
    [ObservableProperty]
    public partial List<SymbolIconData> FilteredIcons { get; private set; } = [];

    /// <summary>
    ///     Gets or sets the icon currently selected in the gallery.
    /// </summary>
    [ObservableProperty]
    public partial SymbolIconData? SelectedIcon { get; set; }

    /// <summary>
    ///     Gets or sets the text used to filter the icon gallery.
    /// </summary>
    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets a value indicating whether filled icon glyphs are shown instead of outlined ones.
    /// </summary>
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
