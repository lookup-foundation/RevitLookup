using RevitLookup.ServiceDefaults.FileSystem;

namespace RevitLookup.Tests;

public sealed class PathExtensionsTests
{
    [Test]
    [Arguments(@"C:\root", "child", @"C:\root\child")]
    [Arguments(@"C:\root", "", @"C:\root")]
    [Arguments(@"C:\root", @"D:\other", @"D:\other")]
    public async Task AppendPath_SingleSegment_CombinesWithSource(string source, string segment, string expected)
    {
        // Act
        var combined = source.AppendPath(segment);

        // Assert
        await Assert.That(combined).IsEqualTo(expected);
    }

    [Test]
    [Arguments(@"C:\root", new[] { "a", "b", "c" }, @"C:\root\a\b\c")]
    [Arguments(@"C:\root", new[] { "a", "", "b" }, @"C:\root\a\b")]
    [Arguments(@"C:\root", new[] { "a", @"D:\other" }, @"D:\other")]
    public async Task AppendPath_MultipleSegments_CombinesInOrder(string source, string[] segments, string expected)
    {
        // Act
        var combined = source.AppendPath(segments);

        // Assert
        await Assert.That(combined).IsEqualTo(expected);
    }
}
