using Bogus;

namespace RevitLookup.UI.Playground.Mocks.Models;

/// <summary>
///     Demonstrates members that fail during evaluation. Each diagnostic property throws,
///     so the engine captures the exception as the member value and it is rendered with the
///     critical "Exception" formatting on a red row.
/// </summary>
[PublicAPI]
public sealed class ExceptionSample
{
    public ExceptionSample()
    {
        var faker = new Faker();
        Endpoint = faker.Internet.Url();
        TimeoutSeconds = faker.Random.Int(1, 30);
    }

    public string Endpoint { get; }
    public int TimeoutSeconds { get; }
    public string SensorReading => throw new InvalidOperationException("The sensor returned a malformed payload");
    public string LastResponse => throw new TimeoutException("The remote endpoint did not respond in time");
}