using System.Collections;
using System.Diagnostics.CodeAnalysis;
using LookupEngine;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Settings;

namespace RevitLookup.UI.Playground.Mocks.Decomposition;

/// <summary>
///     Represents a Playground mock of <see cref="IDecompositionService" /> that runs <c>LookupComposer</c> against Playground sample data instead of a live Revit document.
/// </summary>
/// <param name="settingsService">The service supplying the decomposition options applied to each request.</param>
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
public sealed class MockDecompositionService(ISettingsService settingsService) : IDecompositionService
{
    /// <inheritdoc />
    public List<ObservableDecomposedObject> DecompositionStackHistory { get; } = [];

    /// <inheritdoc />
    public async Task<ObservableDecomposedObject> DecomposeAsync(object? obj)
    {
        var options = CreateDecomposeMembersOptions();
        return await Task.Run(() =>
        {
            var result = LookupComposer.Decompose(obj, options);
            return DecompositionResultMapper.Convert(result);
        });
    }

    /// <inheritdoc />
    public async Task<List<ObservableDecomposedObject>> DecomposeAsync(IEnumerable objects)
    {
        var options = CreateDecomposeOptions();
        return await Task.Run(() =>
        {
            var capacity = objects is ICollection collection ? collection.Count : 4;
            var decomposedObjects = new List<ObservableDecomposedObject>(capacity);
            foreach (var obj in objects)
            {
                var decomposedObject = LookupComposer.DecomposeObject(obj, options);
                decomposedObjects.Add(DecompositionResultMapper.Convert(decomposedObject));
            }

            return decomposedObjects;
        });
    }

    /// <inheritdoc />
    public async Task<List<ObservableDecomposedMember>> DecomposeMembersAsync(ObservableDecomposedObject decomposedObject)
    {
        var options = CreateDecomposeMembersOptions();
        return await Task.Run(() =>
        {
            var decomposedMembers = LookupComposer.DecomposeMembers(decomposedObject.RawValue, options);
            var members = new List<ObservableDecomposedMember>(decomposedMembers.Count);

            foreach (var decomposedMember in decomposedMembers)
            {
                members.Add(DecompositionResultMapper.Convert(decomposedMember));
            }

            return members;
        });
    }

    /// <inheritdoc />
    public async Task EvaluateMemberAsync(ObservableDecomposedMember decomposedMember)
    {
        if (decomposedMember.Member?.Evaluator is null)
        {
            return;
        }

        await Task.Run(() => decomposedMember.Member.Evaluate());

        DecompositionResultMapper.Update(decomposedMember.Member, decomposedMember);
    }

    /// <inheritdoc />
    public async Task EvaluateMemberWithTransactionAsync(ObservableDecomposedMember decomposedMember)
    {
        if (decomposedMember.Member?.Evaluator is null)
        {
            return;
        }

        await Task.Run(() => decomposedMember.Member.Evaluate());

        DecompositionResultMapper.Update(decomposedMember.Member, decomposedMember);
    }

    private static DecomposeOptions CreateDecomposeOptions()
    {
        return new DecomposeOptions
        {
            EnableRedirection = true,
            TypeResolver = DescriptorsMap.FindDescriptor
        };
    }

    private DecomposeOptions CreateDecomposeMembersOptions()
    {
        return new DecomposeOptions
        {
            IncludeRoot = settingsService.DecompositionSettings.IncludeRoot,
            IncludeFields = settingsService.DecompositionSettings.IncludeFields,
            IncludeEvents = settingsService.DecompositionSettings.IncludeEvents,
            IncludeUnsupported = settingsService.DecompositionSettings.IncludeUnsupported,
            IncludePrivateMembers = settingsService.DecompositionSettings.IncludePrivate,
            IncludeStaticMembers = settingsService.DecompositionSettings.IncludeStatic,
            EnableExtensions = settingsService.DecompositionSettings.IncludeExtensions,
            EnableRedirection = true,
            TypeResolver = DescriptorsMap.FindDescriptor,
            EvaluationPolicy = new MethodEvaluationPolicy
            {
                EvaluatedFilter = (method, type) =>
                {
                    if (method.ReturnType == typeof(void))
                    {
                        return false;
                    }

                    if (type.Namespace is null)
                    {
                        return true;
                    }

                    if (type.Namespace.StartsWith("System.Windows"))
                    {
                        return false;
                    }

                    if (type.Namespace.StartsWith("System"))
                    {
                        return true;
                    }

                    return false;
                }
            }
        };
    }
}
