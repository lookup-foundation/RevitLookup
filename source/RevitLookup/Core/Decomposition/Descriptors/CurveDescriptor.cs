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
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nice3point.Revit.Extensions.SystemExtensions;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Services.Application;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Visualization;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class CurveDescriptor : Descriptor, IDescriptorResolver, IContextMenuConnector
{
    private readonly Curve _curve;

    public CurveDescriptor(Curve curve)
    {
        _curve = curve;
        if (curve.IsBound || curve.IsCyclic) Name = $"{curve.Length.ToString(CultureInfo.InvariantCulture)} ft";
    }

    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Curve.GetEndPoint) => ResolveGetEndPoint,
            nameof(Curve.GetEndParameter) => ResolveGetEndParameter,
            nameof(Curve.GetEndPointReference) => ResolveGetEndPointReference,
            nameof(Curve.Evaluate) => ResolveEvaluate,
            _ => null
        };

        IVariant ResolveEvaluate()
        {
            var variants = Variants.Values<XYZ>(3);
            var endParameter0 = _curve.GetEndParameter(0);
            var endParameter1 = _curve.GetEndParameter(1);
            var endParameterMid = (endParameter0 + endParameter1) / 2;

            variants.Add(_curve.Evaluate(endParameter0, false), $"Parameter {endParameter0.Round(3)}");
            variants.Add(_curve.Evaluate(endParameterMid, false), $"Parameter {endParameterMid.Round(3)}");
            variants.Add(_curve.Evaluate(endParameter1, false), $"Parameter {endParameter1.Round(3)}");

            return variants.Consume();
        }

        IVariant ResolveGetEndPoint()
        {
            return Variants.Values<XYZ>(2)
                .Add(_curve.GetEndPoint(0), "Point 0")
                .Add(_curve.GetEndPoint(1), "Point 1")
                .Consume();
        }

        IVariant ResolveGetEndParameter()
        {
            return Variants.Values<double>(2)
                .Add(_curve.GetEndParameter(0), "Parameter 0")
                .Add(_curve.GetEndParameter(1), "Parameter 1")
                .Consume();
        }

        IVariant ResolveGetEndPointReference()
        {
            return Variants.Values<Reference>(2)
                .Add(_curve.GetEndPointReference(0), "Reference 0")
                .Add(_curve.GetEndPointReference(1), "Reference 1")
                .Consume();
        }
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
#if REVIT2023_OR_GREATER
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_curve, SelectCurve)
            .SetShortcut(Key.F6);

        contextMenu.AddMenuItem("ShowMenuItem")
            .SetCommand(_curve, ShowCurve)
            .SetShortcut(Key.F7);
#endif
        contextMenu.AddMenuItem("VisualizeMenuItem")
            .SetAvailability((_curve.IsBound || _curve.IsCyclic) && _curve.ApproximateLength > 1e-6)
            .SetCommand(_curve, VisualizeCurve)
            .SetShortcut(Key.F8);

        async Task VisualizeCurve(Curve curve)
        {
            if (RevitContext.ActiveUiDocument is null) return;

            try
            {
                var dialog = serviceProvider.GetRequiredService<PolylineVisualizationDialog>();
                await dialog.ShowDialogAsync(curve);
            }
            catch (Exception exception)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<CurveDescriptor>>();
                var notificationService = serviceProvider.GetRequiredService<INotificationService>();

                logger.LogError(exception, "Visualize curve error");
                notificationService.ShowError("Visualization error", exception);
            }
        }

#if REVIT2023_OR_GREATER
        void SelectCurve(Curve curve)
        {
            if (RevitContext.ActiveUiDocument is null) return;
            if (curve.Reference is null) return;

            EventHandlers.ActionEventHandler.Raise(_ => RevitContext.ActiveUiDocument.Selection.SetReferences([curve.Reference]));
        }

        void ShowCurve(Curve curve)
        {
            if (RevitContext.ActiveUiDocument is null) return;
            if (curve.Reference is null) return;

            EventHandlers.ActionEventHandler.Raise(application =>
            {
                var uiDocument = application.ActiveUIDocument;
                if (uiDocument is null) return;

                var element = curve.Reference.ElementId.ToElement(uiDocument.Document);
                if (element is not null) uiDocument.ShowElements(element);

                uiDocument.Selection.SetReferences([curve.Reference]);
            });
        }
#endif
    }
}