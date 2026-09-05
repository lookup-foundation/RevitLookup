using RevitLookup.UI.Framework.Extensions;

namespace RevitLookup.Tests;

public sealed class EnumerableExtensionsTests
{
    [Test]
    [Arguments(new[] { 42 })]
    [Arguments(new[] { 1, 2, 3, 4, 5 })]
    [Arguments(new[] { 7, 7, 7 })]
    public async Task Random_PopulatedCollection_ReturnsContainedElement(int[] source)
    {
        // Act
        var picked = source.Random();

        // Assert
        await Assert.That(source).Contains(picked);
    }

    [Test]
    public async Task Random_EmptyCollection_ThrowsInvalidOperationException()
    {
        // Arrange
        var source = Array.Empty<int>();

        // Act, Assert
        await Assert.That(source.Random).Throws<InvalidOperationException>();
    }

    [Test]
    [Arguments(new[] { 1 })]
    [Arguments(new[] { 1, 2, 3, 4, 5 })]
    [Arguments(new[] { 5, 5, 6 })]
    public async Task Randomize_Collection_PreservesAllElements(int[] source)
    {
        // Act
        var randomized = source.Randomize();

        // Assert
        await Assert.That(randomized).IsEquivalentTo(source);
    }

    [Test]
    public async Task Randomize_List_ReordersTheSameInstance()
    {
        // Arrange
        var source = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        var randomized = source.Randomize();

        // Assert
        await Assert.That(randomized).IsSameReferenceAs(source);
    }
}
