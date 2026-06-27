using RevitLookup.Common.Utils;
using Color = System.Drawing.Color;

namespace RevitLookup.Tests.Unit.Common;

public sealed class ColorRepresentationUtilsTests
{
    [Test]
    public async Task ColorToHex_PrimaryColor_ReturnsLowercaseHex()
    {
        // Arrange
        var color = Color.FromArgb(255, 0, 0);

        // Act
        var hex = ColorRepresentationUtils.ColorToHex(color);

        // Assert
        await Assert.That(hex).IsEqualTo("ff0000");
    }

    [Test]
    public async Task ColorToHexInteger_PrimaryColor_ReturnsPrefixedUppercaseHex()
    {
        // Arrange
        var color = Color.FromArgb(255, 0, 0);

        // Act
        var hex = ColorRepresentationUtils.ColorToHexInteger(color);

        // Assert
        await Assert.That(hex).IsEqualTo("0xFFFF0000");
    }

    [Test]
    public async Task ColorToDecimal_White_ReturnsPackedValue()
    {
        // Arrange
        var color = Color.FromArgb(255, 255, 255);

        // Act
        var value = ColorRepresentationUtils.ColorToDecimal(color);

        // Assert
        await Assert.That(value).IsEqualTo("16777215");
    }

    [Test]
    public async Task ColorToRgb_ArbitraryColor_ReturnsCommaSeparatedComponents()
    {
        // Arrange
        var color = Color.FromArgb(10, 20, 30);

        // Act
        var rgb = ColorRepresentationUtils.ColorToRgb(color);

        // Assert
        await Assert.That(rgb).IsEqualTo("10, 20, 30");
    }

    [Test]
    public async Task ColorToFloat_Gray_ReturnsNormalizedComponents()
    {
        // Arrange
        var color = Color.FromArgb(128, 128, 128);

        // Act
        var floats = ColorRepresentationUtils.ColorToFloat(color);

        // Assert
        await Assert.That(floats).IsEqualTo("0.5f, 0.5f, 0.5f, 1f");
    }

    [Test]
    [Arguments(255, 0, 0, "Red")]
    [Arguments(0, 0, 0, "Black")]
    [Arguments(255, 255, 255, "White")]
    [Arguments(0, 0, 255, "Blue")]
    [Arguments(255, 255, 0, "Yellow")]
    public async Task GetColorName_ExactKnownColor_ReturnsName(int red, int green, int blue, string expected)
    {
        // Arrange
        var color = Color.FromArgb(red, green, blue);

        // Act
        var name = ColorRepresentationUtils.GetColorName(color);

        // Assert
        await Assert.That(name).IsEqualTo(expected);
    }
}
