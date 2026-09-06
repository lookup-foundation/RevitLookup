using System.IO;
using System.Reflection;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.UI.Playground.SampleData;
#if NETFRAMEWORK
using RevitLookup.UI.Framework.Menus;
#endif

namespace RevitLookup.UI.Playground.ViewModels.Pages.DesignGuidance;

/// <summary>
///     Represents the sample data for the Segoe icon gallery page.
/// </summary>
[UsedImplicitly]
public partial class FontIconsPageViewModel : ObservableObject
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FontIconsPageViewModel" /> class.
    /// </summary>
    public FontIconsPageViewModel()
    {
        var jsonText = ReadIconData();
        Icons = JsonSerializer.Deserialize<List<FontIconData>>(jsonText)!
            .OrderBy(data => data.Name)
            .ToList();

        SelectedIcon = Icons.FirstOrDefault();
    }

    /// <summary>
    ///     Gets or sets the full set of Segoe icons available to browse.
    /// </summary>
    [ObservableProperty]
    public partial List<FontIconData> Icons { get; set; } = [];

    /// <summary>
    ///     Gets or sets the icons that match the current <see cref="SearchText" /> filter.
    /// </summary>
    [ObservableProperty]
    public partial List<FontIconData> FilteredIcons { get; set; } = [];

    /// <summary>
    ///     Gets or sets the icon currently selected in the gallery.
    /// </summary>
    [ObservableProperty]
    public partial FontIconData? SelectedIcon { get; set; }

    /// <summary>
    ///     Gets or sets the text used to filter the icon gallery.
    /// </summary>
    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    private static string ReadIconData()
    {
        const string resourceName = "RevitLookup.UI.Playground.SampleData.FontIcons.json";

        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    partial void OnIconsChanged(List<FontIconData> value)
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
                var results = new List<FontIconData>();

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
        catch (Exception)
        {
            //ignored
        }
    }
}
