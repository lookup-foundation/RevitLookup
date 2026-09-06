using Bogus;
using RevitLookup.Abstractions.Updater;

namespace RevitLookup.UI.Playground.Mocks.Updater;

/// <summary>
///     Represents a Playground mock of <see cref="ISoftwareUpdateService" /> that fabricates an update outcome with <c>Bogus</c> instead of calling GitHub.
/// </summary>
public sealed class MockSoftwareUpdateService : ISoftwareUpdateService
{
    /// <inheritdoc />
    public string? NewVersion { get; private set; }

    /// <inheritdoc />
    public string? ReleaseNotesUrl { get; private set; }

    /// <inheritdoc />
    public string? LocalFilePath { get; private set; }

    /// <inheritdoc />
    public DateTime? LatestCheckDate { get; private set; }

    /// <inheritdoc />
    public async Task<bool> CheckUpdatesAsync()
    {
        await Task.Delay(1000);
        LatestCheckDate = DateTime.Now;

        var faker = new Faker();
        var factor = faker.Random.Int(0, 100);
        if (factor < 20)
        {
            throw new OperationCanceledException();
        }

        if (factor < 50)
        {
            return false;
        }

        NewVersion = faker.System.Version().ToString(3);
        ReleaseNotesUrl = "https://github.com/";
        LocalFilePath = faker.System.FilePath().OrNull(faker);

        return true;
    }

    /// <inheritdoc />
    public async Task DownloadUpdateAsync()
    {
        await Task.Delay(1000);

        var faker = new Faker();
        var factor = faker.Random.Int(0, 100);
        if (factor < 60)
        {
            throw new OperationCanceledException();
        }
    }
}
