using Bogus;
using RevitLookup.Abstractions.Services.Settings;

namespace RevitLookup.UI.Playground.Mocks.Services.Settings;

public sealed class MockSoftwareUpdateService : ISoftwareUpdateService
{
    public string? NewVersion { get; private set; }
    public string? ReleaseNotesUrl { get; private set; }
    public string? LocalFilePath { get; private set; }
    public DateTime? LatestCheckDate { get; private set; }

    public async Task<bool> CheckUpdatesAsync()
    {
        await Task.Delay(1000);
        LatestCheckDate = DateTime.Now;

        var faker = new Faker();
        var factor = faker.Random.Int(0, 100);
        if (factor < 20) throw new OperationCanceledException();
        if (factor < 50) return false;

        NewVersion = faker.System.Version().ToString(3);
        ReleaseNotesUrl = "https://github.com/";
        LocalFilePath = faker.System.FilePath().OrNull(faker);

        return true;
    }

    public async Task DownloadUpdateAsync()
    {
        await Task.Delay(1000);

        var faker = new Faker();
        var factor = faker.Random.Int(0, 100);
        if (factor < 60) throw new OperationCanceledException();
    }
}