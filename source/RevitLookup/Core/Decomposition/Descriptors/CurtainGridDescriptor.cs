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

public sealed class CurtainGridDescriptor(CurtainGrid curtainGrid) : Descriptor, IDescriptorConfigurator
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(CurtainGrid.GetCell)).Resolve(ResolveCells);
        configuration.Member(nameof(CurtainGrid.GetPanel)).Resolve(ResolvePanels);
        return;

        IVariant ResolveCells()
        {
            var uLinesIds = (List<ElementId>) curtainGrid.GetUGridLineIds();
            var vLinesIds = (List<ElementId>) curtainGrid.GetVGridLineIds();
            uLinesIds.Add(ElementId.InvalidElementId);
            vLinesIds.Add(ElementId.InvalidElementId);
            var capacity = uLinesIds.Count * vLinesIds.Count;

            var variants = Variants.Values<CurtainCell>(capacity);
            foreach (var uLineId in uLinesIds)
            {
                foreach (var vLineId in vLinesIds)
                {
                    var cell = curtainGrid.GetCell(uLineId, vLineId);
                    variants.Add(cell, $"U {uLineId}, V {vLineId}");
                }
            }

            return variants.Consume();
        }

        IVariant ResolvePanels()
        {
            var uLinesIds = (List<ElementId>) curtainGrid.GetUGridLineIds();
            var vLinesIds = (List<ElementId>) curtainGrid.GetVGridLineIds();
            uLinesIds.Add(ElementId.InvalidElementId);
            vLinesIds.Add(ElementId.InvalidElementId);
            var capacity = uLinesIds.Count * vLinesIds.Count;

            var variants = Variants.Values<Panel>(capacity);
            foreach (var uLineId in uLinesIds)
            {
                foreach (var vLineId in vLinesIds)
                {
                    var panel = curtainGrid.GetPanel(uLineId, vLineId);
                    variants.Add(panel, $"U {uLineId}, V {vLineId} - {panel.Name}, ID{panel.Id}");
                }
            }

            return variants.Consume();
        }
    }
}