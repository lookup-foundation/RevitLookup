using Build.Azure.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;
using ModularPipelines.DotNet.Services;
using ModularPipelines.FileSystem;
using ModularPipelines.Models;

namespace Build.Azure;

/// <summary>
///     Represents the AzureSignTool .NET global tool.
/// </summary>
/// <param name="dotNet">The .NET CLI used to install the AzureSignTool tool.</param>
/// <param name="command">The command-line runner used to execute the installed tool.</param>
/// <remarks>
///     <para>The tool code-signs files with a certificate stored in an Azure Key Vault instance.</para>
///     <para>Concurrent calls to <see cref="Sign" /> are serialized; only one installation and signing command runs at a time.</para>
/// </remarks>
public sealed class AzureSignTool(IDotNet dotNet, ICommand command)
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);
    private readonly Folder _temporaryFolder = Folder.CreateTemporaryFolder();

    /// <summary>
    ///     Installs the AzureSignTool tool if needed and code-signs the files described by <paramref name="options" /> using its <c>sign</c> command.
    /// </summary>
    /// <param name="options">The signing options passed to the AzureSignTool <c>sign</c> command.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The result of the AzureSignTool command execution.</returns>
    public async Task<CommandResult> Sign(AzureSignToolOptions options, CancellationToken cancellationToken = default)
    {
        await SemaphoreSlim.WaitAsync(cancellationToken);

        try
        {
            await dotNet.Tool.Execute(new DotNetToolOptions
            {
                Arguments = ["install", "AzureSignTool", "--tool-path", _temporaryFolder.Path]
            }, cancellationToken: cancellationToken);

            return await command.ExecuteCommandLineTool(options with
            {
                Tool = _temporaryFolder.GetFile("AzureSignTool.exe")
            }, cancellationToken: cancellationToken);
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }
}
