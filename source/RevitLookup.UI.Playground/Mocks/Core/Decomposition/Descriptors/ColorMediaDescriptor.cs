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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.Common.Utils;
using RevitLookup.UI.Framework.Extensions;
using Color = System.Windows.Media.Color;

namespace RevitLookup.UI.Playground.Mocks.Core.Decomposition.Descriptors;

public sealed class ColorMediaDescriptor : Descriptor, IDescriptorExtension, IContextMenuConnector
{
    private readonly Color _color;

    public ColorMediaDescriptor(Color color)
    {
        _color = color;
        Name = $"RGB: {color.R} {color.G} {color.B}";
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define("HEX").Register(() => Variants.Value(ColorRepresentationUtils.ColorToHex(_color.GetDrawingColor())));
        manager.Define("HEX int").Register(() => Variants.Value(ColorRepresentationUtils.ColorToHexInteger(_color.GetDrawingColor())));
        manager.Define("RGB").Register(() => Variants.Value(ColorRepresentationUtils.ColorToRgb(_color.GetDrawingColor())));
        manager.Define("HSL").Register(() => Variants.Value(ColorRepresentationUtils.ColorToHsl(_color.GetDrawingColor())));
        manager.Define("HSV").Register(() => Variants.Value(ColorRepresentationUtils.ColorToHsv(_color.GetDrawingColor())));
        manager.Define("CMYK").Register(() => Variants.Value(ColorRepresentationUtils.ColorToCmyk(_color.GetDrawingColor())));
        manager.Define("HSB").Register(() => Variants.Value(ColorRepresentationUtils.ColorToHsb(_color.GetDrawingColor())));
        manager.Define("HSI").Register(() => Variants.Value(ColorRepresentationUtils.ColorToHsi(_color.GetDrawingColor())));
        manager.Define("HWB").Register(() => Variants.Value(ColorRepresentationUtils.ColorToHwb(_color.GetDrawingColor())));
        manager.Define("NCol").Register(() => Variants.Value(ColorRepresentationUtils.ColorToNCol(_color.GetDrawingColor())));
        manager.Define("CIELAB").Register(() => Variants.Value(ColorRepresentationUtils.ColorToCielab(_color.GetDrawingColor())));
        manager.Define("CIEXYZ").Register(() => Variants.Value(ColorRepresentationUtils.ColorToCieXyz(_color.GetDrawingColor())));
        manager.Define("VEC4").Register(() => Variants.Value(ColorRepresentationUtils.ColorToFloat(_color.GetDrawingColor())));
        manager.Define("Decimal").Register(() => Variants.Value(ColorRepresentationUtils.ColorToDecimal(_color.GetDrawingColor())));
        manager.Define("Name").Register(() => Variants.Value(ColorRepresentationUtils.GetColorName(_color.GetDrawingColor())));
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
        contextMenu.AddMenuItem("DeleteMenuItem")
            .SetCommand(_color, DeleteElement)
            .SetShortcut(Key.Delete);

        void DeleteElement(Color element)
        {
            var notificationService = serviceProvider.GetRequiredService<INotificationService>();
            try
            {
                var summaryViewModel = serviceProvider.GetRequiredService<IDecompositionSummaryViewModel>();
                var placementTarget = (FrameworkElement) contextMenu.PlacementTarget;
                summaryViewModel.RemoveItem(placementTarget.DataContext);

                notificationService.ShowSuccess("Success", $"Element successfully removed");
            }
            catch (OperationCanceledException exception)
            {
                notificationService.ShowWarning("Warning", exception.Message);
            }
            catch (Exception exception)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<ColorMediaDescriptor>>();

                logger.LogError(exception, "Color deletion error");
                notificationService.ShowError("Color deletion error", exception.Message);
            }
        }
    }
}