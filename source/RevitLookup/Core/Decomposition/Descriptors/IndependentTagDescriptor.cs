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

public sealed class IndependentTagDescriptor(IndependentTag tag) : ElementDescriptor(tag)
{
    public override void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(IndependentTag.Dispose)).Disable();
        configuration.Member(nameof(IndependentTag.CanLeaderEndConditionBeAssigned)).Resolve(ResolveLeaderEndCondition);
#if REVIT2022_OR_GREATER
        configuration.Member(nameof(IndependentTag.GetLeaderElbow)).Resolve(ResolveLeaderElbow);
        configuration.Member(nameof(IndependentTag.GetLeaderEnd)).Resolve(ResolveLeaderEnd);
        configuration.Member(nameof(IndependentTag.HasLeaderElbow)).Resolve(ResolveHasLeaderElbow);
#endif
#if REVIT2023_OR_GREATER
        configuration.Member(nameof(IndependentTag.IsLeaderVisible)).Resolve(ResolveIsLeaderVisible);
#endif
        return;

        IVariant ResolveLeaderEndCondition()
        {
            var conditions = Enum.GetValues<LeaderEndCondition>();
            var variants = Variants.Values<bool>(conditions.Length);

            foreach (var condition in conditions)
            {
                var result = tag.CanLeaderEndConditionBeAssigned(condition);
                variants.Add(result, $"{condition}: {result}");
            }

            return variants.Consume();
        }
#if REVIT2022_OR_GREATER
        IVariant ResolveLeaderElbow()
        {
            var references = tag.GetTaggedReferences();
            var variants = Variants.Values<XYZ>(references.Count);

            foreach (var reference in references)
            {
#if REVIT2023_OR_GREATER
                if (!tag.IsLeaderVisible(reference)) continue;
#endif
                if (!tag.HasLeaderElbow(reference)) continue;

                variants.Add(tag.GetLeaderElbow(reference));
            }

            return variants.Consume();
        }

        IVariant ResolveLeaderEnd()
        {
            var references = tag.GetTaggedReferences();
            var variants = Variants.Values<XYZ>(references.Count);

            foreach (var reference in references)
            {
#if REVIT2023_OR_GREATER
                if (!tag.IsLeaderVisible(reference)) continue;
#endif
                variants.Add(tag.GetLeaderEnd(reference));
            }

            return variants.Consume();
        }

        IVariant ResolveHasLeaderElbow()
        {
            var references = tag.GetTaggedReferences();
            var variants = Variants.Values<bool>(references.Count);
            foreach (var reference in references)
            {
#if REVIT2023_OR_GREATER
                if (!tag.IsLeaderVisible(reference)) continue;
#endif
                variants.Add(tag.HasLeaderElbow(reference));
            }

            return variants.Consume();
        }
#endif
#if REVIT2023_OR_GREATER

        IVariant ResolveIsLeaderVisible()
        {
            var references = tag.GetTaggedReferences();
            var variants = Variants.Values<bool>(references.Count);

            foreach (var reference in references)
            {
                variants.Add(tag.IsLeaderVisible(reference));
            }

            return variants.Consume();
        }
#endif
    }
}