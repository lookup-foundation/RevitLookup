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
using System.Windows.Controls;
using System.Windows.Input;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Services.Application;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Visualization;

namespace RevitLookup.Core.Decomposition.Descriptors;

public class FaceDescriptor : Descriptor, IDescriptorCollector, IContextMenuConnector
{
    private readonly Face _face;

    public FaceDescriptor(Face face)
    {
        _face = face;
        Name = $"{face.Area.ToString(CultureInfo.InvariantCulture)} ft²";
    }

    public virtual void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
#if REVIT2023_OR_GREATER

        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_face, SelectFace)
            .SetShortcut(Key.F6);

        contextMenu.AddMenuItem("ShowMenuItem")
            .SetCommand(_face, ShowFace)
            .SetShortcut(Key.F7);
#endif
        contextMenu.AddMenuItem("VisualizeMenuItem")
            .SetAvailability(_face.Area > 1e-6)
            .SetCommand(_face, VisualizeFace)
            .SetShortcut(Key.F8);

        async Task VisualizeFace(Face face)
        {
            if (RevitContext.ActiveUiDocument is null) return;

            try
            {
                var dialog = serviceProvider.GetRequiredService<FaceVisualizationDialog>();
                await dialog.ShowDialogAsync(face);
            }
            catch (Exception exception)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<FaceDescriptor>>();
                var notificationService = serviceProvider.GetRequiredService<INotificationService>();

                logger.LogError(exception, "Visualize Face error");
                notificationService.ShowError("Visualization error", exception);
            }
        }
#if REVIT2023_OR_GREATER

        void SelectFace(Face face)
        {
            if (RevitContext.ActiveUiDocument is null) return;
            if (face.Reference is null) return;

            EventHandlers.ActionEventHandler.Raise(_ => RevitContext.ActiveUiDocument.Selection.SetReferences([face.Reference]));
        }

        void ShowFace(Face face)
        {
            if (RevitContext.ActiveUiDocument is null) return;
            if (face.Reference is null) return;

            EventHandlers.ActionEventHandler.Raise(application =>
            {
                var uiDocument = application.ActiveUIDocument;
                if (uiDocument is null) return;

                var element = face.Reference.ElementId.ToElement(uiDocument.Document);
                if (element is not null) uiDocument.ShowElements(element);

                uiDocument.Selection.SetReferences([face.Reference]);
            });
        }
#endif
    }
}