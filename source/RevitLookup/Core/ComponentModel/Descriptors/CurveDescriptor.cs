// Copyright 2003-2023 by Autodesk, Inc.
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
using System.Windows;
using System.Windows.Controls;
#if R23_OR_GREATER
using System.Windows.Input;
using RevitLookup.Views.Extensions;
#endif
using Autodesk.Revit.DB;
using Nice3point.Revit.Extensions.Runtime;
using RevitLookup.Core.Contracts;
using RevitLookup.Core.Objects;

namespace RevitLookup.Core.ComponentModel.Descriptors;

public sealed class CurveDescriptor : Descriptor, IDescriptorResolver, IDescriptorConnector
{
    private readonly Curve _curve;

    public CurveDescriptor(Curve curve)
    {
        _curve = curve;
        if (curve.IsBound || curve.IsCyclic) Name = $"{curve.Length.ToString(CultureInfo.InvariantCulture)} ft";
    }

    public void RegisterMenu(ContextMenu contextMenu, UIElement bindableElement)
    {
#if R23_OR_GREATER
        contextMenu.AddMenuItem()
            .SetHeader("Show curve")
            .SetCommand(_curve, curve =>
            {
                Application.ActionEventHandler.Raise(_ =>
                {
                    if (RevitApi.UiDocument is null) return;
                    if (curve.Reference is null) return;
                    var element = curve.Reference.ElementId.ToElement(RevitApi.Document);
                    if (element is not null) RevitApi.UiDocument.ShowElements(element);
                    RevitApi.UiDocument.Selection.SetReferences(new List<Reference>(1) {curve.Reference});
                });
            })
            .SetShortcut(bindableElement, ModifierKeys.Alt, Key.F7);
#endif
    }

    public ResolveSet Resolve(Document context, string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Curve.GetEndPoint) => ResolveSet
                .Append(_curve.GetEndPoint(0), "Point 0")
                .AppendVariant(_curve.GetEndPoint(1), "Point 1"),
            nameof(Curve.GetEndParameter) => ResolveSet
                .Append(_curve.GetEndParameter(0), "Parameter 0")
                .AppendVariant(_curve.GetEndParameter(1), "Parameter 1"),
            nameof(Curve.GetEndPointReference) => ResolveSet
                .Append(_curve.GetEndPointReference(0), "Reference 0")
                .AppendVariant(_curve.GetEndPointReference(1), "Reference 1"),
            nameof(Curve.Evaluate) => ResolveEvaluate(),
            _ => null
        };

        ResolveSet ResolveEvaluate()
        {
            var resolveSummary = new ResolveSet(3);
            var endParameter0 = _curve.GetEndParameter(0);
            var endParameter1 = _curve.GetEndParameter(1);
            var endParameterMid = (endParameter0 + endParameter1) / 2;

            resolveSummary.AppendVariant(_curve.Evaluate(endParameter0, false), $"Parameter {endParameter0.Round(3)}");
            resolveSummary.AppendVariant(_curve.Evaluate(endParameterMid, false), $"Parameter {endParameterMid.Round(3)}");
            resolveSummary.AppendVariant(_curve.Evaluate(endParameter1, false), $"Parameter {endParameter1.Round(3)}");

            return resolveSummary;
        }
    }
}