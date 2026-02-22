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

using System.Windows.Controls;
using System.Windows.Input;
using LookupEngine.Abstractions.Decomposition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Visualization;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class XyzDescriptor : Descriptor, IContextMenuConnector
{
    private readonly XYZ _point;

    public XyzDescriptor(XYZ point)
    {
        _point = point;
        Name = point.ToString();
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
        contextMenu.AddMenuItem("VisualizeMenuItem")
            .SetAvailability(!_point.IsUnitLength())
            .SetCommand(_point, VisualizeXyz)
            .SetShortcut(Key.F8);

        async Task VisualizeXyz(XYZ point)
        {
            if (RevitContext.ActiveUiDocument is null) return;

            try
            {
                var dialog = serviceProvider.GetRequiredService<XyzVisualizationDialog>();
                await dialog.ShowDialogAsync(point);
            }
            catch (Exception exception)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<XyzDescriptor>>();
                var notificationService = serviceProvider.GetRequiredService<INotificationService>();

                logger.LogError(exception, "Visualize XYZ error");
                notificationService.ShowError("Visualization error", exception);
            }
        }
    }
}