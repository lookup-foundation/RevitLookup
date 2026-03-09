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

using System.Globalization;
using System.Windows.Input;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Visualization;
using ContextMenu = System.Windows.Controls.ContextMenu;
#if REVIT2023_OR_GREATER
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed partial class EdgeDescriptor : Descriptor, IDescriptorCollector, IContextMenuConnector
{
    private readonly Edge _edge;

    public EdgeDescriptor(Edge edge)
    {
        _edge = edge;
        Name = $"{edge.ApproximateLength.ToString(CultureInfo.InvariantCulture)} ft";
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
#if REVIT2023_OR_GREATER
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_edge, edge => SelectEdgeEvent.Raise(edge))
            .SetShortcut(Key.F6);

        contextMenu.AddMenuItem("ShowMenuItem")
            .SetCommand(_edge, edge => ShowEdgeEvent.Raise(edge))
            .SetShortcut(Key.F7);
#endif
        contextMenu.AddMenuItem("VisualizeMenuItem")
            .SetAvailability(_edge.ApproximateLength > 1e-6)
            .SetCommand(_edge, VisualizeEdge)
            .SetShortcut(Key.F8);

        async Task VisualizeEdge(Edge edge)
        {
            if (RevitContext.ActiveUiDocument is null) return;

            try
            {
                var dialog = serviceProvider.GetRequiredService<PolylineVisualizationDialog>();
                await dialog.ShowDialogAsync(edge);
            }
            catch (Exception exception)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<EdgeDescriptor>>();
                var notificationService = serviceProvider.GetRequiredService<INotificationService>();

                logger.LogError(exception, "Visualize Edge error");
                notificationService.ShowError("Visualization error", exception);
            }
        }
    }
#if REVIT2023_OR_GREATER

    [ExternalEvent(AllowDirectInvocation = true)]
    private static void SelectEdge(UIApplication application, Edge edge)
    {
        if (application.ActiveUIDocument is null) return;
        if (edge.Reference is null) return;

        application.ActiveUIDocument.Selection.SetReferences([edge.Reference]);
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private static void ShowEdge(UIApplication application, Edge edge)
    {
        var uiDocument = application.ActiveUIDocument;
        if (uiDocument is null) return;
        if (edge.Reference is null) return;

        var element = edge.Reference.ElementId.ToElement(uiDocument.Document);
        if (element is not null) uiDocument.ShowElements(element);

        uiDocument.Selection.SetReferences([edge.Reference]);
    }
#endif
}