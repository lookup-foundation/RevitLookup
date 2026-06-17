using Bogus;

namespace RevitLookup.UI.Playground.Mocks.Models;

/// <summary>
///     Demonstrates disabled members. The destructive operations are permanently disabled by
///     <c>DisabledSampleDescriptor</c> and shown as greyed-out placeholder text.
/// </summary>
[PublicAPI]
public sealed class DisabledSample
{
    public DisabledSample()
    {
        var faker = new Faker();
        FileName = faker.System.FileName();
        Owner = faker.Name.FullName();
        SizeBytes = faker.Random.Long(1_000, 50_000_000);
    }

    public string FileName { get; }
    public string Owner { get; }
    public long SizeBytes { get; }
    public bool IsReadOnly => true;

    public void Delete()
    {
        throw new InvalidOperationException("Delete must never be invoked");
    }

    public void Overwrite()
    {
        throw new InvalidOperationException("Overwrite must never be invoked");
    }
}