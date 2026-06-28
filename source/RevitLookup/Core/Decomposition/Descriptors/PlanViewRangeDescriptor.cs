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

using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class PlanViewRangeDescriptor(PlanViewRange viewRange) : Descriptor, IDescriptorConfigurator
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(PlanViewRange.Dispose)).Disable();
        configuration.Member(nameof(PlanViewRange.GetOffset)).Resolve(ResolveGetOffset);
        configuration.Member(nameof(PlanViewRange.GetLevelId)).Resolve(ResolveGetLevelId);
        return;

        IVariant ResolveGetOffset()
        {
            var topOffset = viewRange.GetOffset(PlanViewPlane.TopClipPlane);
            var cutOffset = viewRange.GetOffset(PlanViewPlane.CutPlane);
            var bottomOffset = viewRange.GetOffset(PlanViewPlane.BottomClipPlane);
            var underlayOffset = viewRange.GetOffset(PlanViewPlane.UnderlayBottom);

            return Variants.Values<double>(4)
                .Add(topOffset, $"Top clip plane: {topOffset}")
                .Add(cutOffset, $"Cut plane: {cutOffset}")
                .Add(bottomOffset, $"Bottom clip plane: {bottomOffset}")
                .Add(underlayOffset, $"Underlay bottom: {underlayOffset}")
                .Consume();
        }

        IVariant ResolveGetLevelId()
        {
            return Variants.Values<ElementId>(4)
                .Add(viewRange.GetLevelId(PlanViewPlane.TopClipPlane), "Top clip plane")
                .Add(viewRange.GetLevelId(PlanViewPlane.CutPlane), "Cut plane")
                .Add(viewRange.GetLevelId(PlanViewPlane.BottomClipPlane), "Bottom clip plane")
                .Add(viewRange.GetLevelId(PlanViewPlane.UnderlayBottom), "Underlay bottom")
                .Consume();
        }
    }
}