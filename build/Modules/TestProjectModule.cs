using ModularPipelines.Attributes;
using ModularPipelines.Conditions;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Sourcy.DotNet;

namespace Build.Modules;

/// <summary>
///     Test the add-in for each supported Revit configuration.
/// </summary>
[SkipIf<IsCI>]
[DependsOn<ResolveConfigurationsModule>]
[DependsOn<CompileProjectModule>(Optional = true)]
public sealed class TestProjectModule : Module
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
    ///     Test the add-in project for the specified configuration.
    /// </summary>
    private static async Task<CommandResult> CompileAsync(IModuleContext context, string configuration, CancellationToken cancellationToken)
    {
        return await context.DotNet().Test(new DotNetTestOptions
        {
            Solution = Solutions.RevitLookup.FullName,
            Configuration = configuration,
            Properties =
            [
                ("IsRepackable", "false")
            ]
        }, cancellationToken: cancellationToken);
    }
}