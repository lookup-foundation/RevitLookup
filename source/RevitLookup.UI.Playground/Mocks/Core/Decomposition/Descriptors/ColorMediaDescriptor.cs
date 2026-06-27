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

public sealed partial class ColorMediaDescriptor : Descriptor, IDescriptorConfigurator, IContextMenuConnector
{
    private readonly Color _color;

    public ColorMediaDescriptor(Color color)
    {
        _color = color;
        Name = $"#{ColorRepresentationUtils.ColorToHex(color.GetDrawingColor()).ToUpperInvariant()}";
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Extension("HEX").Register(() => ColorRepresentationUtils.ColorToHex(_color.GetDrawingColor()));
        configuration.Extension("HEX int").Register(() => ColorRepresentationUtils.ColorToHexInteger(_color.GetDrawingColor()));
        configuration.Extension("RGB").Register(() => ColorRepresentationUtils.ColorToRgb(_color.GetDrawingColor()));
        configuration.Extension("HSL").Register(() => ColorRepresentationUtils.ColorToHsl(_color.GetDrawingColor()));
        configuration.Extension("HSV").Register(() => ColorRepresentationUtils.ColorToHsv(_color.GetDrawingColor()));
        configuration.Extension("CMYK").Register(() => ColorRepresentationUtils.ColorToCmyk(_color.GetDrawingColor()));
        configuration.Extension("HSB").Register(() => ColorRepresentationUtils.ColorToHsb(_color.GetDrawingColor()));
        configuration.Extension("HSI").Register(() => ColorRepresentationUtils.ColorToHsi(_color.GetDrawingColor()));
        configuration.Extension("HWB").Register(() => ColorRepresentationUtils.ColorToHwb(_color.GetDrawingColor()));
        configuration.Extension("NCol").Register(() => ColorRepresentationUtils.ColorToNCol(_color.GetDrawingColor()));
        configuration.Extension("CIELAB").Register(() => ColorRepresentationUtils.ColorToCielab(_color.GetDrawingColor()));
        configuration.Extension("CIEXYZ").Register(() => ColorRepresentationUtils.ColorToCieXyz(_color.GetDrawingColor()));
        configuration.Extension("VEC4").Register(() => ColorRepresentationUtils.ColorToFloat(_color.GetDrawingColor()));
        configuration.Extension("Decimal").Register(() => ColorRepresentationUtils.ColorToDecimal(_color.GetDrawingColor()));
        configuration.Extension("Name").Register(() => ColorRepresentationUtils.GetColorName(_color.GetDrawingColor()));
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
        contextMenu.AddMenuItem("DeleteMenuItem")
            .SetCommand(_color, DeleteElement)
            .SetShortcut(Key.Delete);
        return;

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

                LogColorDeletionError(logger, exception);
                notificationService.ShowError("Color deletion error", exception.Message);
            }
        }
    }

    [LoggerMessage(LogLevel.Error, "Color deletion error")]
    private static partial void LogColorDeletionError(ILogger<ColorMediaDescriptor> logger, Exception exception);
}