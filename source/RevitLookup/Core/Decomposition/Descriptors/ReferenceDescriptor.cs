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

using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using RevitLookup.Abstractions.Decomposition;
using ContextMenu = System.Windows.Controls.ContextMenu;
#if REVIT2023_OR_GREATER
using System.Windows.Input;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using RevitLookup.UI.Framework.Extensions;
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed partial class ReferenceDescriptor : Descriptor, IDescriptorConfigurator, IDescriptorConfigurator<Document>, IContextMenuConnector
{
    private readonly Reference _reference;

    public ReferenceDescriptor(Reference reference)
    {
        _reference = reference;
        Name = reference.ElementReferenceType.ToString();
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Reference.Dispose)).Disable();
    }

    public void Configure(IMemberConfigurator<Document> configuration)
    {
        configuration.Member(nameof(Reference.ConvertToStableRepresentation)).Resolve(context => _reference.ConvertToStableRepresentation(context));
        
        configuration.Extension(nameof(CurveByPointsUtils.GetFaceRegions)).Register(context => CurveByPointsUtils.GetFaceRegions(context, _reference));
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
#if REVIT2023_OR_GREATER
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_reference, reference => SelectReferenceEvent.Raise(reference))
            .SetShortcut(Key.F6);
#endif
    }

#if REVIT2023_OR_GREATER

    [ExternalEvent(AllowDirectInvocation = true)]
    private static void SelectReference(UIApplication application, Reference reference)
    {
        if (application.ActiveUIDocument is null) return;

        application.ActiveUIDocument.Selection.SetReferences([reference]);
    }
#endif
}