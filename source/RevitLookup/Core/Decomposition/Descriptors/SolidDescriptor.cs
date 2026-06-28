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

public sealed partial class SolidDescriptor : Descriptor, IDescriptorConfigurator, IContextMenuConnector
{
    private readonly Solid _solid;

    public SolidDescriptor(Solid solid)
    {
        _solid = solid;
        Name = $"{solid.Volume.ToString(CultureInfo.InvariantCulture)} ft?";
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Solid.Dispose)).Disable();
        configuration.Extension(nameof(SolidUtils.SplitVolumes)).Register(() => SolidUtils.SplitVolumes(_solid));
        configuration.Extension(nameof(SolidUtils.IsValidForTessellation)).Register(() => SolidUtils.IsValidForTessellation(_solid));
        configuration.Extension(nameof(SolidUtils.TessellateSolidOrShell)).Register(ResolveTessellateSolidOrShell);
        configuration.Extension(nameof(SolidUtils.Clone)).Defer(() => _solid.Clone());
        configuration.Extension(nameof(SolidUtils.CreateTransformed)).NotSupported();
        configuration.Extension(nameof(GeometryCreationUtilities.CreateBlendGeometry)).AsStatic().NotSupported();
        configuration.Extension(nameof(GeometryCreationUtilities.CreateExtrusionGeometry)).AsStatic().NotSupported();
        configuration.Extension(nameof(GeometryCreationUtilities.CreateFixedReferenceSweptGeometry)).AsStatic().NotSupported();
        configuration.Extension(nameof(GeometryCreationUtilities.CreateLoftGeometry)).AsStatic().NotSupported();
        configuration.Extension(nameof(GeometryCreationUtilities.CreateRevolvedGeometry)).AsStatic().NotSupported();
        configuration.Extension(nameof(GeometryCreationUtilities.CreateSweptBlendGeometry)).AsStatic().NotSupported();
        configuration.Extension(nameof(GeometryCreationUtilities.CreateSweptGeometry)).AsStatic().NotSupported();
        configuration.Extension("CutWithHalfSpace").Map(nameof(BooleanOperationsUtils.CutWithHalfSpace)).NotSupported();
        configuration.Extension("CutWithHalfSpaceModifyingOriginalSolid").Map(nameof(BooleanOperationsUtils.CutWithHalfSpaceModifyingOriginalSolid)).NotSupported();
        configuration.Extension("ExecuteBooleanOperation").Map(nameof(BooleanOperationsUtils.ExecuteBooleanOperation)).NotSupported();
        configuration.Extension("ExecuteBooleanOperationModifyingOriginalSolid").Map(nameof(BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid)).NotSupported();
#if REVIT2026_OR_GREATER
        configuration.Extension(nameof(SolidUtils.ComputeIsGeometricallyClosed)).Register(() => SolidUtils.ComputeIsGeometricallyClosed(_solid));
        configuration.Extension(nameof(SolidUtils.ComputeIsTopologicallyClosed)).Register(() => SolidUtils.ComputeIsTopologicallyClosed(_solid));
#endif
        return;

        IVariant ResolveTessellateSolidOrShell()
        {
            return Variants.Values<TriangulatedSolidOrShell>(2)
                .Add(SolidUtils.TessellateSolidOrShell(_solid, new SolidOrShellTessellationControls {LevelOfDetail = 0}), "Coarse")
                .Add(SolidUtils.TessellateSolidOrShell(_solid, new SolidOrShellTessellationControls {LevelOfDetail = 1}), "Fine")
                .Consume();
        }
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
#if REVIT2023_OR_GREATER
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_solid, solid => SelectSolidEvent.Raise(solid))
            .SetShortcut(Key.F6);

        contextMenu.AddMenuItem("ShowMenuItem")
            .SetCommand(_solid, solid => ShowSolidEvent.Raise(solid))
            .SetShortcut(Key.F7);
#endif
        contextMenu.AddMenuItem("VisualizeMenuItem")
            .SetAvailability(_solid.IsValidForTessellation)
            .SetCommand(_solid, solid => VisualizeSolidAsync(solid, serviceProvider))
            .SetShortcut(Key.F8);
    }

    private static async Task VisualizeSolidAsync(Solid solid, IServiceProvider serviceProvider)
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

            LogVisualizeSolidError(logger, exception);
            notificationService.ShowError("Visualization error", exception);
        }
    }
#if REVIT2023_OR_GREATER

    [ExternalEvent(AllowDirectInvocation = true)]
    private static void SelectSolid(UIApplication application, Solid solid)
    {
        if (application.ActiveUIDocument is null) return;

        var references = solid.Faces.Cast<Face>()
            .Select(static face => face.Reference)
            .Where(static reference => reference is not null)
            .ToList();

        if (references.Count == 0) return;

        application.ActiveUIDocument.Selection.SetReferences(references);
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private static void ShowSolid(UIApplication application, Solid solid)
    {
        var uiDocument = application.ActiveUIDocument;
        if (uiDocument is null) return;

        var references = solid.Faces.Cast<Face>()
            .Select(static face => face.Reference)
            .Where(static reference => reference is not null)
            .ToList();

        if (references.Count == 0) return;

        var element = references[0].ElementId.ToElement(uiDocument.Document);
        if (element is not null) uiDocument.ShowElements(element);

        uiDocument.Selection.SetReferences(references);
    }
#endif

    [LoggerMessage(LogLevel.Error, "Visualize solid error")]
    private static partial void LogVisualizeSolidError(ILogger<SolidDescriptor> logger, Exception exception);
}