using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline module that resolves the solution configurations required to compile the add-in for every supported Revit version.
/// </summary>
public sealed class ResolveConfigurationsModule : Module<string[]>
{
    protected override async Task<string[]?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var solutionModel = await LoadSolutionModelAsync(context, cancellationToken);
        var configurations = solutionModel.BuildTypes
            .Where(configuration => configuration.Contains("Release.R", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        configurations.ShouldNotBeEmpty("No solution configurations have been found");

        return configurations;
    }

    private static async Task<SolutionModel> LoadSolutionModelAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var solution = context.Git().RootDirectory.FindFile(file => file.Extension == ".slnx");
        if (solution is not null)
        {
            await using var slnxStream = solution.GetStream();
            return await SolutionSerializers.SlnXml.OpenAsync(slnxStream, cancellationToken);
        }

        solution = context.Git().RootDirectory.FindFile(file => file.Extension == ".sln");
        solution.ShouldNotBeNull("Solution file not found.");

        await using var slnStream = solution.GetStream();
        return await SolutionSerializers.SlnFileV12.OpenAsync(slnStream, cancellationToken);
    }
}
