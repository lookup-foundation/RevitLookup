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
using RevitLookup.Abstractions.Configuration;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Services.Application;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Visualization;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class SolidDescriptor : Descriptor, IDescriptorExtension, IContextMenuConnector
{
    private readonly Solid _solid;

    public SolidDescriptor(Solid solid)
    {
        _solid = solid;
        Name = $"{solid.Volume.ToString(CultureInfo.InvariantCulture)} ft³";
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(nameof(SolidUtils.SplitVolumes), () => Variants.Value(SolidUtils.SplitVolumes(_solid)));
        manager.Register(nameof(SolidUtils.IsValidForTessellation), () => Variants.Value(SolidUtils.IsValidForTessellation(_solid)));
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
#if REVIT2023_OR_GREATER
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_solid, SelectSolid)
            .SetShortcut(Key.F6);

        contextMenu.AddMenuItem("ShowMenuItem")
            .SetCommand(_solid, ShowSolid)
            .SetShortcut(Key.F7);
#endif
        contextMenu.AddMenuItem("VisualizeMenuItem")
            .SetAvailability(_solid.IsValidForTessellation)
            .SetCommand(_solid, VisualizeSolid)
            .SetShortcut(Key.F8);

        async Task VisualizeSolid(Solid solid)
        {
            if (RevitContext.ActiveUiDocument is null) return;

            try
            {
                var dialog = serviceProvider.GetRequiredService<SolidVisualizationDialog>();
                await dialog.ShowDialogAsync(solid);
            }
            catch (Exception exception)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<SolidDescriptor>>();
                var notificationService = serviceProvider.GetRequiredService<INotificationService>();

                logger.LogError(exception, "Visualize solid error");
                notificationService.ShowError("Visualization error", exception);
            }
        }

#if REVIT2023_OR_GREATER
        void SelectSolid(Solid solid)
        {
            try
            {
                if (RevitContext.ActiveUiDocument is null) return;

                var references = solid.Faces.Cast<Face>()
                    .Select(face => face.Reference)
                    .Where(reference => reference is not null)
                    .ToList();

                if (references.Count == 0) return;

                EventHandlers.ActionEventHandler.Raise(_ => RevitContext.ActiveUiDocument.Selection.SetReferences(references));
            }
            catch (Exception exception)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<SolidDescriptor>>();
                var notificationService = serviceProvider.GetRequiredService<INotificationService>();

                logger.LogError(exception, "Select solid error");
                notificationService.ShowError("Selection error", exception);
            }
        }

        void ShowSolid(Solid solid)
        {
            try
            {
                if (RevitContext.ActiveUiDocument is null) return;

                var references = solid.Faces.Cast<Face>()
                    .Select(face => face.Reference)
                    .Where(reference => reference is not null)
                    .ToList();

                if (references.Count == 0) return;

                EventHandlers.ActionEventHandler.Raise(application =>
                {
                    var uiDocument = application.ActiveUIDocument;
                    if (uiDocument is null) return;

                    var element = references[0].ElementId.ToElement(uiDocument.Document);
                    if (element is not null) uiDocument.ShowElements(element);

                    uiDocument.Selection.SetReferences(references);
                });
            }
            catch (Exception exception)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<SolidDescriptor>>();
                var notificationService = serviceProvider.GetRequiredService<INotificationService>();

                logger.LogError(exception, "Show solid error");
                notificationService.ShowError("Showing error", exception);
            }
        }
#endif
    }
}