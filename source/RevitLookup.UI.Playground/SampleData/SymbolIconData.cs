using Wpf.Ui.Controls;

namespace RevitLookup.UI.Playground.SampleData;

/// <summary>
///     Provides a sample <see cref="SymbolRegular" /> icon for the Playground icon gallery.
/// </summary>
public sealed class SymbolIconData
{
    /// <summary>
    ///     Gets or sets the icon's display name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets or sets the symbol represented by this icon.
    /// </summary>
    public required SymbolRegular Icon { get; init; }

    /// <summary>
    ///     Gets or sets the icon's Unicode code point as a hexadecimal string.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    ///     Gets the XML character entity for <see cref="Code" />.
    /// </summary>
    public string TextGlyph => $"&#x{Code};";
}
