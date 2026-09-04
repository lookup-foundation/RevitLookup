using RevitLookup.UI.Framework.Colors;
using Color = System.Drawing.Color;

namespace RevitLookup.Tests.Unit;

public sealed class ColorRepresentationUtilsTests
{
    [Test]
    [Arguments(255, 0, 0, "ff0000")]
    [Arguments(0, 0, 0, "000000")]
    [Arguments(255, 255, 255, "ffffff")]
    [Arguments(10, 20, 30, "0a141e")]
    public async Task ColorToHex_Color_ReturnsLowercaseHex(int red, int green, int blue, string expected)
    {
        // Arrange
        var color = Color.FromArgb(red, green, blue);

        // Act
        var hex = ColorRepresentationUtils.ColorToHex(color);

        // Assert
        await Assert.That(hex).IsEqualTo(expected);
    }

    [Test]
    [Arguments(255, 0, 0, "0xFFFF0000")]
    [Arguments(0, 0, 0, "0xFF000000")]
    [Arguments(255, 255, 255, "0xFFFFFFFF")]
    [Arguments(10, 20, 30, "0xFF0A141E")]
    public async Task ColorToHexInteger_Color_ReturnsPrefixedUppercaseHex(int red, int green, int blue, string expected)
    {
        // Arrange
        var color = Color.FromArgb(red, green, blue);

        // Act
        var hex = ColorRepresentationUtils.ColorToHexInteger(color);

        // Assert
        await Assert.That(hex).IsEqualTo(expected);
    }

    [Test]
    [Arguments(255, 255, 255, "16777215")]
    [Arguments(0, 0, 0, "0")]
    [Arguments(255, 0, 0, "16711680")]
    [Arguments(10, 20, 30, "660510")]
    public async Task ColorToDecimal_Color_ReturnsPackedValue(int red, int green, int blue, string expected)
    {
        // Arrange
        var color = Color.FromArgb(red, green, blue);

        // Act
        var value = ColorRepresentationUtils.ColorToDecimal(color);

        // Assert
        await Assert.That(value).IsEqualTo(expected);
    }

    [Test]
    [Arguments(10, 20, 30, "10, 20, 30")]
    [Arguments(0, 0, 0, "0, 0, 0")]
    [Arguments(255, 255, 255, "255, 255, 255")]
    public async Task ColorToRgb_Color_ReturnsCommaSeparatedComponents(int red, int green, int blue, string expected)
    {
        // Arrange
        var color = Color.FromArgb(red, green, blue);

        // Act
        var rgb = ColorRepresentationUtils.ColorToRgb(color);

        // Assert
        await Assert.That(rgb).IsEqualTo(expected);
    }

    [Test]
    [Arguments(128, 128, 128, "0.5f, 0.5f, 0.5f, 1f")]
    [Arguments(255, 255, 255, "1f, 1f, 1f, 1f")]
    [Arguments(0, 0, 0, "0f, 0f, 0f, 1f")]
    [Arguments(64, 0, 255, "0.25f, 0f, 1f, 1f")]
    public async Task ColorToFloat_Color_ReturnsNormalizedComponents(int red, int green, int blue, string expected)
    {
        // Arrange
        var color = Color.FromArgb(red, green, blue);

        // Act
        var floats = ColorRepresentationUtils.ColorToFloat(color);

        // Assert
        await Assert.That(floats).IsEqualTo(expected);
    }

    [Test]
    [Arguments(255, 0, 0, "Red")]
    [Arguments(0, 0, 0, "Black")]
    [Arguments(255, 255, 255, "White")]
    [Arguments(0, 0, 255, "Blue")]
    [Arguments(255, 255, 0, "Yellow")]
    [Arguments(0, 128, 128, "Teal")]
    public async Task GetColorName_KnownColor_ReturnsName(int red, int green, int blue, string expected)
    {
        // Arrange
        var color = Color.FromArgb(red, green, blue);

        // Act
        var name = ColorRepresentationUtils.GetColorName(color);

        // Assert
        await Assert.That(name).IsEqualTo(expected);
    }
}
