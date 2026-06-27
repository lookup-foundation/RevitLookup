using System.IO;
using RevitLookup.Common.Extensions;

namespace RevitLookup.Tests.Unit.Common;

public sealed class SystemExtensionsTests
{
    [Test]
    [Arguments(typeof(int), true)]
    [Arguments(typeof(double), true)]
    [Arguments(typeof(bool), true)]
    [Arguments(typeof(string), true)]
    [Arguments(typeof(DayOfWeek), true)]
    [Arguments(typeof(object), false)]
    public async Task IsPrimitiveType_VariousTypes_MatchesExpectation(Type type, bool expected)
    {
        // Act
        var result = type.IsPrimitiveType();

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task AppendPath_MultipleSegments_CombinesInOrder()
    {
        // Arrange
        const string root = "root";

        // Act
        var combined = root.AppendPath("a", "b", "c");

        // Assert
        await Assert.That(combined).IsEqualTo(Path.Combine("root", "a", "b", "c"));
    }

    [Test]
    public async Task AppendPath_SingleSegment_CombinesWithSource()
    {
        // Arrange
        const string root = "root";

        // Act
        var combined = root.AppendPath("child");

        // Assert
        await Assert.That(combined).IsEqualTo(Path.Combine("root", "child"));
    }
}
