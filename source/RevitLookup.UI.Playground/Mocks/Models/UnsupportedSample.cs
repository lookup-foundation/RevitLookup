using Bogus;

namespace RevitLookup.UI.Playground.Mocks.Models;

/// <summary>
///     Demonstrates unsupported members. <c>UnsupportedSampleDescriptor</c> registers members the engine
///     cannot evaluate. They appear only when the "Unsupported" filter is enabled in the grid context menu.
/// </summary>
[PublicAPI]
public sealed class UnsupportedSample
{
    public UnsupportedSample()
    {
        var faker = new Faker();
        Host = faker.Internet.DomainName();
        Port = faker.Random.Int(1024, 65535);
    }

    public string Host { get; }
    public int Port { get; }
    public string Protocol => "legacy-rpc";
}