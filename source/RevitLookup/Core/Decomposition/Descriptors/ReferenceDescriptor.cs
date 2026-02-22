// Copyright (c) Lookup Foundation and Contributors
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// THIS PROGRAM IS PROVIDED "AS IS" AND WITH ALL FAULTS.
// NO IMPLIED WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE IS PROVIDED.
// THERE IS NO GUARANTEE THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Services.Application;
using RevitLookup.UI.Framework.Extensions;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ReferenceDescriptor : Descriptor, IDescriptorResolver<Document>, IDescriptorExtension<Document>, IContextMenuConnector
{
    private readonly Reference _reference;

    public ReferenceDescriptor(Reference reference)
    {
        _reference = reference;
        Name = reference.ElementReferenceType.ToString();
    }

    public Func<Document, IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Reference.ConvertToStableRepresentation) => ResolveConvertToStableRepresentation,
            _ => null
        };

        IVariant ResolveConvertToStableRepresentation(Document context)
        {
            return Variants.Value(_reference.ConvertToStableRepresentation(context));
        }
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
#if REVIT2023_OR_GREATER
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_reference, SelectReference)
            .SetShortcut(Key.F6);

        void SelectReference(Reference reference)
        {
            if (RevitContext.ActiveUiDocument is null) return;

            EventHandlers.ActionEventHandler.Raise(_ => RevitContext.ActiveUiDocument.Selection.SetReferences([reference]));
        }
#endif
    }

    public void RegisterExtensions(IExtensionManager<Document> manager)
    {
        manager.Register(nameof(CurveByPointsUtils.GetFaceRegions), context => Variants.Value(CurveByPointsUtils.GetFaceRegions(context, _reference)));
    }
}