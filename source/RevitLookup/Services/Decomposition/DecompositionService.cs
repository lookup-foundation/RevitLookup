using System.Collections;
using System.Diagnostics.CodeAnalysis;
using LookupEngine;
using LookupEngine.Options;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.Core.Decomposition;
using RevitLookup.Mappers;
using RevitLookup.Services.Application;

namespace RevitLookup.Services.Decomposition;

[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
public sealed class DecompositionService(ISettingsService settingsService) : IDecompositionService
{
    public List<ObservableDecomposedObject> DecompositionStackHistory { get; } = [];

    public async Task<ObservableDecomposedObject> DecomposeAsync(object? obj)
    {
        var options = CreateDecomposeMembersOptions();
        return await EventHandlers.AsyncObjectHandler.RaiseAsync(_ =>
        {
            if (TryFindRevitContext(obj, out var context))
            {
                options.Context = context;
            }

            var result = LookupComposer.Decompose(obj, options);
            return DecompositionResultMapper.Convert(result);
        });
    }

    public async Task<List<ObservableDecomposedObject>> DecomposeAsync(IEnumerable objects)
    {
        return await EventHandlers.AsyncObjectsHandler.RaiseAsync(_ =>
        {
            var options = CreateDecomposeOptions();
            var capacity = objects is ICollection collection ? collection.Count : 4;
            var decomposedObjects = new List<ObservableDecomposedObject>(capacity);

            foreach (var obj in objects)
            {
                if (TryFindRevitContext(obj, out var context))
                {
                    options.Context = context;
                }

                var decomposedObject = LookupComposer.DecomposeObject(obj, options);
                decomposedObjects.Add(DecompositionResultMapper.Convert(decomposedObject));
            }

            return decomposedObjects;
        });
    }

    public async Task<List<ObservableDecomposedMember>> DecomposeMembersAsync(ObservableDecomposedObject decomposedObject)
    {
        var options = CreateDecomposeMembersOptions();
        return await EventHandlers.AsyncMembersHandler.RaiseAsync(_ =>
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
        });
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