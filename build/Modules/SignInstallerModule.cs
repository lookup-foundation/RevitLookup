using Build.Azure;
using Build.Azure.Options;
using Build.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Shouldly;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline module that signs the MSI installer.
/// </summary>
/// <param name="signingOptions">The Azure Key Vault credentials used to sign the installer.</param>
/// <param name="buildOptions">The build settings that supply the output directory containing the installer.</param>
[DependsOn<CreateInstallerModule>]
public sealed partial class SignInstallerModule(IOptions<SigningOptions> signingOptions, IOptions<BuildOptions> buildOptions) : Module<CommandResult>
{
    protected override ModuleConfiguration Configure()
    {
        return ModuleConfiguration.Create()
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
    }

    protected override async Task<CommandResult?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var targetFiles = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory)
            .GetFiles(file => file.Extension is ".msi" or ".exe")
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
