using Build.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Shouldly;
using Sourcy.DotNet;

namespace Build.Modules;

/// <summary>
///     Compile the add-in for each supported Revit configuration.
/// </summary>
[DependsOn<ResolveVersioningModule>]
[DependsOn<ResolveConfigurationsModule>]
[DependsOn<CleanProjectModule>(Optional = true)]
public sealed class CompileProjectModule(IOptions<BuildOptions> buildOptions, IHostEnvironment environment) : Module
{
    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var configurationsResult = await context.GetModule<ResolveConfigurationsModule>();
        var configurations = configurationsResult.ValueOrDefault!;

        foreach (var configuration in configurations)
        {
            await context.SubModule(configuration, async () => await CompileAsync(context, configuration, cancellationToken));
        }
    }

    /// <summary>
    ///     Compile the add-in project for the specified configuration.
    /// </summary>
    private async Task<CommandResult> CompileAsync(IModuleContext context, string configuration, CancellationToken cancellationToken)
    {
        buildOptions.Value.Versions
            .TryGetValue(configuration, out var version)
            .ShouldBeTrue($"Can't map version for configuration: {configuration}");

        return await context.DotNet().Build(new DotNetBuildOptions
        {
            ProjectSolution = Projects.RevitLookup.FullName,
            Configuration = configuration,
            Properties = new List<KeyValue>
            {
                ("Version", version),
                ("Environment", environment.EnvironmentName.ToUpperInvariant())
            }
        }, cancellationToken: cancellationToken);
    }
}