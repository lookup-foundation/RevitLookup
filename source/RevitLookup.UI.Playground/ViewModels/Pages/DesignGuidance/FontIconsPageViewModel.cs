using System.IO;
using System.Reflection;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.UI.Playground.Models;
#if NETFRAMEWORK
using RevitLookup.UI.Framework.Extensions;
#endif

namespace RevitLookup.UI.Playground.ViewModels.Pages.DesignGuidance;

[UsedImplicitly]
public partial class FontIconsPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial List<FontIconData> Icons { get; set; } = [];

    [ObservableProperty]
    public partial List<FontIconData> FilteredIcons { get; set; } = [];

    [ObservableProperty]
    public partial FontIconData? SelectedIcon { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    public FontIconsPageViewModel()
    {
        var jsonText = ReadIconData();
        Icons = JsonSerializer.Deserialize<List<FontIconData>>(jsonText)!
            .OrderBy(data => data.Name)
            .ToList();

        SelectedIcon = Icons.FirstOrDefault();
    }

    private static string ReadIconData()
    {
        const string resourceName = "RevitLookup.UI.Playground.Models.FontIcons.json";

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
}