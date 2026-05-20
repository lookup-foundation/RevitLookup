using Build.Komac.Options;
using Microsoft.Extensions.Logging;
using ModularPipelines.Context;
using ModularPipelines.FileSystem;
using ModularPipelines.GitHub;
using ModularPipelines.Models;
using ModularPipelines.Options;
using File = ModularPipelines.FileSystem.File;

namespace Build.Komac;

public sealed class Komac(IGitHub gitHub, ICommand command, ILogger<Komac> logger)
{
    private const string KomacOwner = "russellbanks";
    private const string KomacRepository = "Komac";
    private const string WindowsSetupPrefix = "-setup-";
    private const string WindowsAssetSuffix = "x86_64-pc-windows-msvc.exe";

    private static readonly HttpClient HttpClient = new();
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);
    private readonly Folder _temporaryFolder = Folder.CreateTemporaryFolder();
    private File? _komacFile;

    public async Task<CommandResult> Update(KomacUpdateOptions options, CancellationToken cancellationToken = default)
    {
        var executable = await EnsureKomacAsync(cancellationToken);
        return await command.ExecuteCommandLineTool(options with {Tool = executable}, cancellationToken: cancellationToken);
    }

    public async Task<CommandResult> ListVersions(KomacListVersionsOptions options, CancellationToken cancellationToken = default)
    {
        var executable = await EnsureKomacAsync(cancellationToken);
        return await command.ExecuteCommandLineTool(
            options with {Tool = executable},
            new CommandExecutionOptions
            {
                ThrowOnNonZeroExitCode = false,
                LogSettings = CommandLoggingOptions.Silent
            },
            cancellationToken);
    }

    private async Task<File> EnsureKomacAsync(CancellationToken cancellationToken)
    {
        await SemaphoreSlim.WaitAsync(cancellationToken);

        try
        {
            if (_komacFile is not null) return _komacFile;

            var release = await gitHub.Client.Repository.Release.GetLatest(KomacOwner, KomacRepository);
            var asset = release.Assets.FirstOrDefault(item => item.Name.EndsWith(WindowsAssetSuffix) && !item.Name.Contains(WindowsSetupPrefix));
            if (asset is null)
            {
                throw new InvalidOperationException($"No Windows x86_64 binary found in the latest {KomacRepository} release ({release.TagName})");
            }

            var komacFile = _temporaryFolder.GetFile("komac.exe");

            await using (var stream = await HttpClient.GetStreamAsync(asset.BrowserDownloadUrl, cancellationToken))
            await using (var fileStream = System.IO.File.Create(komacFile.Path))
            {
                await stream.CopyToAsync(fileStream, cancellationToken);
            }

            logger.LogInformation("Downloaded Komac {Version} from {Url}", release.TagName, asset.BrowserDownloadUrl);

            _komacFile = komacFile;
            return komacFile;
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }
}
