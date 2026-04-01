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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Visualization;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class BoundingBoxXyzDescriptor(BoundingBoxXYZ box) : Descriptor, IDescriptorResolver, IDescriptorExtension, IContextMenuConnector
{
    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            "Bounds" => ResolveBounds,
            "MinEnabled" => ResolveMinEnabled,
            "MaxEnabled" => ResolveMaxEnabled,
            "BoundEnabled" => ResolveBoundEnabled,
            _ => null
        };

        IVariant ResolveBounds()
        {
            return Variants.Values<XYZ>(2)
                .Add(box.get_Bounds(0), "Bound 0")
                .Add(box.get_Bounds(1), "Bound 1")
                .Consume();
        }

        IVariant ResolveMinEnabled()
        {
            var minEnabled0 = box.get_MinEnabled(0);
            var minEnabled1 = box.get_MinEnabled(1);
            var minEnabled2 = box.get_MinEnabled(2);

            return Variants.Values<bool>(3)
                .Add(minEnabled0, $"Dimension 0: {minEnabled0}")
                .Add(minEnabled1, $"Dimension 1: {minEnabled1}")
                .Add(minEnabled2, $"Dimension 2: {minEnabled2}")
                .Consume();
        }

        IVariant ResolveMaxEnabled()
        {
            var maxEnabled0 = box.get_MaxEnabled(0);
            var maxEnabled1 = box.get_MaxEnabled(1);
            var maxEnabled2 = box.get_MaxEnabled(2);

            return Variants.Values<bool>(3)
                .Add(maxEnabled0, $"Dimension 0: {maxEnabled0}")
                .Add(maxEnabled1, $"Dimension 1: {maxEnabled1}")
                .Add(maxEnabled2, $"Dimension 2: {maxEnabled2}")
                .Consume();
        }

        IVariant ResolveBoundEnabled()
        {
            var boundEnabled00 = box.get_BoundEnabled(0, 0);
            var boundEnabled01 = box.get_BoundEnabled(0, 1);
            var boundEnabled02 = box.get_BoundEnabled(0, 2);
            var boundEnabled10 = box.get_BoundEnabled(1, 0);
            var boundEnabled11 = box.get_BoundEnabled(1, 1);
            var boundEnabled12 = box.get_BoundEnabled(1, 2);

            return Variants.Values<bool>(6)
                .Add(boundEnabled00, $"Bound 0, dimension 0: {boundEnabled00}")
                .Add(boundEnabled01, $"Bound 0, dimension 1: {boundEnabled01}")
                .Add(boundEnabled02, $"Bound 0, dimension 2: {boundEnabled02}")
                .Add(boundEnabled10, $"Bound 1, dimension 0: {boundEnabled10}")
                .Add(boundEnabled11, $"Bound 1, dimension 1: {boundEnabled11}")
                .Add(boundEnabled12, $"Bound 1, dimension 2: {boundEnabled12}")
                .Consume();
        }
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define("Centroid").Register(() => Variants.Value((box.Min + box.Max) / 2));
        manager.Define("Vertices").Register(() => Variants.Values<XYZ>(8)
            .Add(new XYZ(box.Min.X, box.Min.Y, box.Min.Z))
            .Add(new XYZ(box.Min.X, box.Min.Y, box.Max.Z))
            .Add(new XYZ(box.Min.X, box.Max.Y, box.Min.Z))
            .Add(new XYZ(box.Min.X, box.Max.Y, box.Max.Z))
            .Add(new XYZ(box.Max.X, box.Min.Y, box.Min.Z))
            .Add(new XYZ(box.Max.X, box.Min.Y, box.Max.Z))
            .Add(new XYZ(box.Max.X, box.Max.Y, box.Min.Z))
            .Add(new XYZ(box.Max.X, box.Max.Y, box.Max.Z))
            .Consume());

        manager.Define("Volume").Register(() =>
        {
            var length = box.Max.X - box.Min.X;
            var width = box.Max.Y - box.Min.Y;
            var height = box.Max.Z - box.Min.Z;

            return Variants.Value(length * width * height);
        });

        manager.Define("SurfaceArea").Register(() =>
        {
            var length = box.Max.X - box.Min.X;
            var width = box.Max.Y - box.Min.Y;
            var height = box.Max.Z - box.Min.Z;

            var area1 = length * width;
            var area2 = length * height;
            var area3 = width * height;

            return Variants.Value(2 * (area1 + area2 + area3));
        });
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
        contextMenu.AddMenuItem("VisualizeMenuItem")
            .SetCommand(box, xyz => VisualizeFace(xyz, serviceProvider))
            .SetShortcut(Key.F8);
    }
    
    private static async Task VisualizeFace(BoundingBoxXYZ boundingBox, IServiceProvider serviceProvider)
    {
        if (RevitContext.ActiveUiDocument is null) return;

        try
        {
            var dialog = serviceProvider.GetRequiredService<BoundingBoxVisualizationDialog>();
            await dialog.ShowDialogAsync(boundingBox);
        }
        catch (Exception exception)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<BoundingBoxXyzDescriptor>>();
            var notificationService = serviceProvider.GetRequiredService<INotificationService>();

            logger.LogError(exception, "Visualize BoundingBox error");
            notificationService.ShowError("Visualization error", exception);
        }
    }
}