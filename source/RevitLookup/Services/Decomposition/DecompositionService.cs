using System.Collections;
using System.Diagnostics.CodeAnalysis;
using LookupEngine;
using LookupEngine.Options;
using Nice3point.Revit.Toolkit.External;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.Core.Decomposition;
using RevitLookup.Mappers;

namespace RevitLookup.Services.Decomposition;

[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
public sealed partial class DecompositionService(ISettingsService settingsService) : IDecompositionService
{
    public List<ObservableDecomposedObject> DecompositionStackHistory { get; } = [];

    public async Task<ObservableDecomposedObject> DecomposeAsync(object? target)
    {
        var options = CreateDecomposeMembersOptions();
        return await DecomposeAsyncEvent.RaiseAsync(target, options);
    }

    public async Task<List<ObservableDecomposedObject>> DecomposeAsync(IEnumerable objects)
    {
        var options = CreateDecomposeOptions();
        return await DecomposeIEnumerableAsyncEvent.RaiseAsync(objects, options);
    }

    public async Task<List<ObservableDecomposedMember>> DecomposeMembersAsync(ObservableDecomposedObject decomposedObject)
    {
        var options = CreateDecomposeMembersOptions();
        return await DecomposeMembersAsyncEvent.RaiseAsync(decomposedObject, options);
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private ObservableDecomposedObject Decompose(object? target, DecomposeOptions<Document> options)
    {
        if (TryFindRevitContext(target, out var context))
        {
            options.Context = context;
        }

        var result = LookupComposer.Decompose(target, options);
        return DecompositionResultMapper.Convert(result);
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private List<ObservableDecomposedObject> DecomposeIEnumerable(IEnumerable targets, DecomposeOptions<Document> options)
    {
        var capacity = targets is ICollection collection ? collection.Count : 4;
        var decomposedObjects = new List<ObservableDecomposedObject>(capacity);

        foreach (var target in targets)
        {
            if (TryFindRevitContext(target, out var context))
            {
                options.Context = context;
            }

            var decomposedObject = LookupComposer.DecomposeObject(target, options);
            decomposedObjects.Add(DecompositionResultMapper.Convert(decomposedObject));
        }

        return decomposedObjects;
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private List<ObservableDecomposedMember> DecomposeMembers(ObservableDecomposedObject decomposedObject, DecomposeOptions<Document> options)
    {
        if (TryFindRevitContext(decomposedObject.RawValue, out var context))
        {
            options.Context = context;
        }

        var decomposedMembers = LookupComposer.DecomposeMembers(decomposedObject.RawValue, options);
        var members = new List<ObservableDecomposedMember>(decomposedMembers.Count);

        foreach (var decomposedMember in decomposedMembers)
        {
            members.Add(DecompositionResultMapper.Convert(decomposedMember));
        }

        return members;
    }

    private bool TryFindRevitContext(object? obj, [MaybeNullWhen(false)] out Document context)
    {
        context = GetKnownContext(obj);
        if (context is not null) return true;
        if (DecompositionStackHistory.Count == 0) return false;

        for (var i = DecompositionStackHistory.Count - 1; i >= 0; i--)
        {
            var historyItem = DecompositionStackHistory[i];
            context = GetKnownContext(historyItem.RawValue);
            if (context is not null) return true;
        }

        return false;
    }

    private static Document? GetKnownContext(object? obj)
    {
        return obj switch
        {
            Element element => element.Document,
            Parameter {Element: not null} parameter => parameter.Element.Document,
            Document document => document,
            _ => null
        };
    }

    private static DecomposeOptions<Document> CreateDecomposeOptions()
    {
        return new DecomposeOptions<Document>
        {
            Context = RevitContext.ActiveDocument!,
            EnableRedirection = true,
            TypeResolver = DescriptorsMap.FindDescriptor
        };
    }

    private DecomposeOptions<Document> CreateDecomposeMembersOptions()
    {
        return new DecomposeOptions<Document>
        {
            Context = RevitContext.ActiveDocument!,
            IncludeRoot = settingsService.DecompositionSettings.IncludeRoot,
            IncludeFields = settingsService.DecompositionSettings.IncludeFields,
            IncludeEvents = settingsService.DecompositionSettings.IncludeEvents,
            IncludeUnsupported = settingsService.DecompositionSettings.IncludeUnsupported,
            IncludePrivateMembers = settingsService.DecompositionSettings.IncludePrivate,
            IncludeStaticMembers = settingsService.DecompositionSettings.IncludeStatic,
            EnableExtensions = settingsService.DecompositionSettings.IncludeExtensions,
            EnableRedirection = true,
            TypeResolver = DescriptorsMap.FindDescriptor
        };
    }
}