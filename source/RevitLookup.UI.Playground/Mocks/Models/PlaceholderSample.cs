using Bogus;

namespace RevitLookup.UI.Playground.Mocks.Models;

/// <summary>
///     Demonstrates placeholder values: members that evaluate to content with nothing to display
///     (such as <c>null</c> or an empty string) and are shown with a placeholder instead of a value.
/// </summary>
[PublicAPI]
public sealed class PlaceholderSample
{
    public PlaceholderSample()
    {
        var faker = new Faker();
        FullName = faker.Name.FullName();
        Email = faker.Internet.Email();
    }

    public string FullName { get; }
    public string Email { get; }
    public string MiddleName => string.Empty;
    public string Notes => string.Empty;
    public string? Nickname => null;
    public Uri? Website => null;
}