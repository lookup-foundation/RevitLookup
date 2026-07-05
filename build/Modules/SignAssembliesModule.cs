using Build.Azure;
using Build.Azure.Options;
using Build.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Shouldly;
using Sourcy.DotNet;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

/// <summary>
///     Sing compiled assemblies.
/// </summary>
[DependsOn<CompileProjectModule>]
public sealed partial class SignAssembliesModule(IOptions<SigningOptions> signingOptions) : Module<CommandResult>
{
    protected override ModuleConfiguration Configure() => ModuleConfiguration.Create()
        .WithSkipWhen(() =>
        {
            var signing = signingOptions.Value;
            if (string.IsNullOrEmpty(signing.VaultUri) ||
                string.IsNullOrEmpty(signing.TenantId) ||
                string.IsNullOrEmpty(signing.ClientId) ||
                string.IsNullOrEmpty(signing.ClientSecret) ||
                string.IsNullOrEmpty(signing.CertificateName))
            {
                return SkipDecision.Skip("Signing credentials are not provided");
            }

            return SkipDecision.DoNotSkip;
        })
        .Build();

    protected override async Task<CommandResult?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var targetProject = new File(Projects.RevitLookup.FullName);

        var targetFiles = targetProject.Folder!
            .GetFolder("bin")
            .GetFolders(folder => folder.Name == "publish")
            .SelectMany(folder => folder.GetFiles(file =>
            {
                if (file.Extension is not ".dll") return false;
                if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromHours(1)) return false;

                return true;
            }))
            .Select(file => file.Path)
            .ToArray();

        targetFiles.ShouldNotBeEmpty("No files were found to sign");

        var inputFile = File.GetNewTemporaryFilePath();
        await inputFile.WriteAsync(targetFiles, cancellationToken);

        LogSigningFiles(context.Logger, targetFiles.Length);

        return await context.Azure().Sign(new AzureSignToolOptions
        {
            KeyVaultUrl = signingOptions.Value.VaultUri,
            KeyVaultTenantId = signingOptions.Value.TenantId,
            KeyVaultClientId = signingOptions.Value.ClientId,
            KeyVaultClientSecret = signingOptions.Value.ClientSecret,
            KeyVaultCertificateName = signingOptions.Value.CertificateName,
            TimestampRfc3161Url = "http://timestamp.digicert.com",
            SkipSigned = true,
            ContinueOnError = true,
            InputFileList = inputFile
        }, cancellationToken);
    }

    [LoggerMessage(LogLevel.Information, "Signing {Count} files")]
    private static partial void LogSigningFiles(ILogger logger, int count);
}