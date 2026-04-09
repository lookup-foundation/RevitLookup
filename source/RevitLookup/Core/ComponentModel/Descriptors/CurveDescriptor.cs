// Copyright 2003-2024 by Autodesk, Inc.
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// 
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nice3point.Revit.Extensions.Runtime;
using RevitLookup.ViewModels.Contracts;
using RevitLookup.Views.Dialogs.Visualization;
using RevitLookup.Views.Extensions;

namespace RevitLookup.Core.ComponentModel.Descriptors;

public sealed class CurveDescriptor : Descriptor, IDescriptorResolver, IDescriptorConnector
{
    private readonly Curve _curve;

    public CurveDescriptor(Curve curve)
    {
        _curve = curve;
        if (curve.IsBound || curve.IsCyclic) Name = $"{curve.Length.ToString(CultureInfo.InvariantCulture)} ft";
    }

    public void RegisterMenu(ContextMenu contextMenu)
    {
#if REVIT2023_OR_GREATER
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_curve, curve =>
            {
                if (Context.ActiveUiDocument is null) return;
                if (curve.Reference is null) return;

                RevitShell.ActionEventHandler.Raise(_ => { Context.ActiveUiDocument.Selection.SetReferences([curve.Reference]); });
            })
            .SetShortcut(Key.F6);

        contextMenu.AddMenuItem("ShowMenuItem")
            .SetCommand(_curve, curve =>
            {
                if (Context.ActiveUiDocument is null) return;
                if (curve.Reference is null) return;

                RevitShell.ActionEventHandler.Raise(_ =>
                {
                    var element = curve.Reference.ElementId.ToElement(Context.ActiveDocument);
                    if (element is not null) Context.ActiveUiDocument.ShowElements(element);
                    Context.ActiveUiDocument.Selection.SetReferences([curve.Reference]);
                });
            })
            .SetShortcut(Key.F7);
#endif

        contextMenu.AddMenuItem("VisualizeMenuItem")
            .SetAvailability((_curve.IsBound || _curve.IsCyclic) && _curve.ApproximateLength > 1e-6)
            .SetCommand(_curve, async curve =>
            {
                if (Context.ActiveUiDocument is null) return;

                var context = (ISnoopViewModel) contextMenu.DataContext;

                try
                {
                    var dialog = context.ServiceProvider.GetRequiredService<PolylineVisualizationDialog>();
                    await dialog.ShowDialogAsync(curve);
                }
                catch (Exception exception)
                {
                    var logger = context.ServiceProvider.GetRequiredService<ILogger<EdgeDescriptor>>();
                    logger.LogError(exception, "VisualizationDialog error");
                }
            })
            .SetShortcut(Key.F8);
    }

    public Func<IVariants> Resolve(Document context, string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Curve.GetEndPoint) => ResolveGetEndPoint,
            nameof(Curve.GetEndParameter) => ResolveGetEndParameter,
            nameof(Curve.GetEndPointReference) => ResolveGetEndPointReference,
            nameof(Curve.Evaluate) => ResolveEvaluate,
            _ => null
        };

        IVariants ResolveEvaluate()
        {
            var variants = new Variants<XYZ>(3);
            var endParameter0 = _curve.GetEndParameter(0);
            var endParameter1 = _curve.GetEndParameter(1);
            var endParameterMid = (endParameter0 + endParameter1) / 2;

            variants.Add(_curve.Evaluate(endParameter0, false), $"Parameter {endParameter0.Round(3)}");
            variants.Add(_curve.Evaluate(endParameterMid, false), $"Parameter {endParameterMid.Round(3)}");
            variants.Add(_curve.Evaluate(endParameter1, false), $"Parameter {endParameter1.Round(3)}");

            return variants;
        }

        IVariants ResolveGetEndPoint()
        {
            return new Variants<XYZ>(2)
                .Add(_curve.GetEndPoint(0), "Point 0")
                .Add(_curve.GetEndPoint(1), "Point 1");
        }

        IVariants ResolveGetEndParameter()
        {
            return new Variants<double>(2)
                .Add(_curve.GetEndParameter(0), "Parameter 0")
                .Add(_curve.GetEndParameter(1), "Parameter 1");
        }

        IVariants ResolveGetEndPointReference()
        {
            return new Variants<Reference>(2)
                .Add(_curve.GetEndPointReference(0), "Reference 0")
                .Add(_curve.GetEndPointReference(1), "Reference 1");
        }
    }
}