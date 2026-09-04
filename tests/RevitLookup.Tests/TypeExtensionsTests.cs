using RevitLookup.UI.Framework.Extensions;

namespace RevitLookup.Tests.Unit;

public sealed class TypeExtensionsTests
{
    [Test]
    [Arguments(typeof(int), true)]
    [Arguments(typeof(double), true)]
    [Arguments(typeof(bool), true)]
    [Arguments(typeof(char), true)]
    [Arguments(typeof(nint), true)]
    [Arguments(typeof(string), true)]
    [Arguments(typeof(DayOfWeek), true)]
    [Arguments(typeof(object), false)]
    [Arguments(typeof(decimal), false)]
    [Arguments(typeof(DateTime), false)]
    [Arguments(typeof(int?), false)]
    [Arguments(typeof(int[]), false)]
    public async Task IsPrimitiveType_VariousTypes_MatchesExpectation(Type type, bool expected)
    {
        // Act
        var result = type.IsPrimitiveType();

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }
}
