using RevitLookup.Common.Extensions;

namespace RevitLookup.Tests.Unit.Common;

public sealed class EnumerableExtensionsTests
{
    [Test]
    public async Task Random_SingleElement_ReturnsThatElement()
    {
        // Arrange
        var source = new[] {42};

        // Act
        var picked = source.Random();

        // Assert
        await Assert.That(picked).IsEqualTo(42);
    }

    [Test]
    public async Task Random_PopulatedCollection_ReturnsContainedElement()
    {
        // Arrange
        var source = new[] {1, 2, 3, 4, 5};

        // Act
        var picked = source.Random();

        // Assert
        await Assert.That(source.Contains(picked)).IsTrue();
    }

    [Test]
    public async Task Random_EmptyCollection_Throws()
    {
        // Arrange
        var source = Array.Empty<int>();

        // Act
        var threw = false;
        try
        {
            source.Random();
        }
        catch (InvalidOperationException)
        {
            threw = true;
        }

        // Assert
        await Assert.That(threw).IsTrue();
    }

    [Test]
    public async Task Randomize_Collection_PreservesAllElements()
    {
        // Arrange
        var source = new List<int> {1, 2, 3, 4, 5};

        // Act
        var result = source.Randomize();

        // Assert
        await Assert.That(result.Count).IsEqualTo(5);
        await Assert.That(result.OrderBy(value => value).SequenceEqual(new[] {1, 2, 3, 4, 5})).IsTrue();
    }
}
