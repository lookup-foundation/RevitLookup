namespace RevitLookup.UI.Playground.SampleData;

/// <summary>
///     Provides a sample font-icon glyph for the Playground icon gallery.
/// </summary>
[PublicAPI]
public class FontIconData
{
    /// <summary>
    ///     Gets or sets the icon's display name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the icon's Unicode code point as a hexadecimal string.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    ///     Gets the character represented by <see cref="Code" />.
    /// </summary>
    public string Character => char.ConvertFromUtf32(Convert.ToInt32(Code, 16));

    /// <summary>
    ///     Gets the C# string escape sequence for <see cref="Code" />.
    /// </summary>
    public string CodeGlyph => "\\x" + Code;

    /// <summary>
    ///     Gets the XML character entity for <see cref="Code" />.
    /// </summary>
    public string TextGlyph => "&#x" + Code + ";";
}
