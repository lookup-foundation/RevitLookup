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
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class DatumPlaneDescriptor(DatumPlane datumPlane) : ElementDescriptor(datumPlane)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(DatumPlane.CanBeVisibleInView) => ResolveCanBeVisibleInView,
            nameof(DatumPlane.GetPropagationViews) => ResolvePropagationViews,
            nameof(DatumPlane.GetDatumExtentTypeInView) => ResolveDatumExtentTypeInView,
            nameof(DatumPlane.HasBubbleInView) => ResolveHasBubbleInView,
            nameof(DatumPlane.IsBubbleVisibleInView) => ResolveBubbleVisibleInView,
            nameof(DatumPlane.GetCurvesInView) => ResolveGetCurvesInView,
            nameof(DatumPlane.GetLeader) => ResolveGetLeader,
            _ => null
        };

        IVariant ResolveCanBeVisibleInView()
        {
            var views = datumPlane.Document.CollectElements()
                .Instances()
                .OfClass<View>()
                .ToElements();
            
            var variants = Variants.Values<bool>(views.Count);

            foreach (var element in views)
            {
                var view = (View)element;
                var result = datumPlane.CanBeVisibleInView(view);
                variants.Add(result, $"{view.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolvePropagationViews()
        {
            var views = datumPlane.Document.CollectElements()
                .Instances()
                .OfClass<View>()
                .ToArray();

            var variants = Variants.Values<ISet<ElementId>>(views.Length);

            foreach (var element in views)
            {
                var view = (View)element;
                if (!datumPlane.CanBeVisibleInView(view)) continue;

                var result = datumPlane.GetPropagationViews(view);
                variants.Add(result, view.Name);
            }

            return variants.Consume();
        }

        IVariant ResolveDatumExtentTypeInView()
        {
            var resultEnd0 = datumPlane.GetDatumExtentTypeInView(DatumEnds.End0, RevitContext.ActiveView);
            var resultEnd1 = datumPlane.GetDatumExtentTypeInView(DatumEnds.End1, RevitContext.ActiveView);

            return Variants.Values<DatumExtentType>(2)
                .Add(resultEnd0, $"End 0, Active view: {resultEnd0}")
                .Add(resultEnd1, $"End 1, Active view: {resultEnd1}")
                .Consume();
        }

        IVariant ResolveHasBubbleInView()
        {
            var resultEnd0 = datumPlane.HasBubbleInView(DatumEnds.End0, RevitContext.ActiveView);
            var resultEnd1 = datumPlane.HasBubbleInView(DatumEnds.End1, RevitContext.ActiveView);

            return Variants.Values<bool>(2)
                .Add(resultEnd0, $"End 0, Active view: {resultEnd0}")
                .Add(resultEnd1, $"End 1, Active view: {resultEnd1}")
                .Consume();
        }

        IVariant ResolveBubbleVisibleInView()
        {
            var resultEnd0 = datumPlane.IsBubbleVisibleInView(DatumEnds.End0, RevitContext.ActiveView);
            var resultEnd1 = datumPlane.IsBubbleVisibleInView(DatumEnds.End1, RevitContext.ActiveView);

            return Variants.Values<bool>(2)
                .Add(resultEnd0, $"End 0, Active view: {resultEnd0}")
                .Add(resultEnd1, $"End 1, Active view: {resultEnd1}")
                .Consume();
        }

        IVariant ResolveGetCurvesInView()
        {
            return Variants.Values<IList<Curve>>(2)
                .Add(datumPlane.GetCurvesInView(DatumExtentType.Model, RevitContext.ActiveView), "Model, Active view")
                .Add(datumPlane.GetCurvesInView(DatumExtentType.ViewSpecific, RevitContext.ActiveView), "ViewSpecific, Active view")
                .Consume();
        }

        IVariant ResolveGetLeader()
        {
            return Variants.Values<Leader>(2)
                .Add(datumPlane.GetLeader(DatumEnds.End0, RevitContext.ActiveView), "End 0, Active view")
                .Add(datumPlane.GetLeader(DatumEnds.End1, RevitContext.ActiveView), "End 1, Active view")
                .Consume();
        }
    }
}