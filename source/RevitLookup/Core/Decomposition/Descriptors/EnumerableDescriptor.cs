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

using System.Collections;
using System.Reflection;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Macros;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Visual;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class EnumerableDescriptor : Descriptor, IDescriptorEnumerator, IDescriptorResolver
{
    public EnumerableDescriptor(IEnumerable value)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        Enumerator = value.GetEnumerator();

        //Checking types to reduce memory allocation when creating an iterator and increase performance
        IsEmpty = value switch
        {
            string => true,
            ICollection enumerable => enumerable.Count == 0,

            // Parameters
            ParameterSet enumerable => enumerable.IsEmpty,
            ParameterMap enumerable => enumerable.IsEmpty,
            FamilyParameterSet enumerable => enumerable.IsEmpty,

            // Elements
            ElementArray enumerable => enumerable.IsEmpty,
            ElementSet enumerable => enumerable.IsEmpty,
            CombinableElementArray enumerable => enumerable.IsEmpty,

            // Geometry
            CurveArray enumerable => enumerable.IsEmpty,
            CurveArrArray enumerable => enumerable.IsEmpty,
            FaceArray enumerable => enumerable.IsEmpty,
            EdgeArray enumerable => enumerable.IsEmpty,
            EdgeArrayArray enumerable => enumerable.IsEmpty,
            ReferenceArray enumerable => enumerable.IsEmpty,
            ReferenceArrayArray enumerable => enumerable.IsEmpty,
            DoubleArray enumerable => enumerable.IsEmpty,
            IntersectionResultArray enumerable => enumerable.IsEmpty,
            VertexIndexPairArray enumerable => enumerable.IsEmpty,
            GeomCombinationSet enumerable => enumerable.IsEmpty,

            // Categories
            CategorySet enumerable => enumerable.IsEmpty,
            CategoryNameMap enumerable => enumerable.IsEmpty,

            // Definitions
            DefinitionBindingMap enumerable => enumerable.IsEmpty,
            DefinitionGroups enumerable => enumerable.IsEmpty,

            // Views, documents
            ViewSet enumerable => enumerable.IsEmpty,
            DocumentSet enumerable => enumerable.IsEmpty,

            // Curves
            ModelCurveArray enumerable => enumerable.IsEmpty,
            ModelCurveArrArray enumerable => enumerable.IsEmpty,
            DetailCurveArray enumerable => enumerable.IsEmpty,
            SymbolicCurveArray enumerable => enumerable.IsEmpty,
            CurveByPointsArray enumerable => enumerable.IsEmpty,

            // Annotations
            DimensionSegmentArray enumerable => enumerable.IsEmpty,
            LeaderArray enumerable => enumerable.IsEmpty,

            // Project
            PhaseArray enumerable => enumerable.IsEmpty,
            ProjectLocationSet enumerable => enumerable.IsEmpty,
            GroupSet enumerable => enumerable.IsEmpty,

            // Shape editing
            SlabShapeVertexArray enumerable => enumerable.IsEmpty,
            SlabShapeCreaseArray enumerable => enumerable.IsEmpty,

            // Conceptual design
            ReferencePointArray enumerable => enumerable.IsEmpty,
            FormArray enumerable => enumerable.IsEmpty,

            // Curtain walls
            CurtainGridSet enumerable => enumerable.IsEmpty,
            MullionTypeSet enumerable => enumerable.IsEmpty,
            PanelTypeSet enumerable => enumerable.IsEmpty,

            // Families
            FamilyTypeSet enumerable => enumerable.IsEmpty,

            // MEP
            ConnectorSet enumerable => enumerable.IsEmpty,
            PlanTopologySet enumerable => enumerable.IsEmpty,
            PlanCircuitSet enumerable => enumerable.IsEmpty,
            SpaceSet enumerable => enumerable.IsEmpty,
            MEPBuildingConstructionSet enumerable => enumerable.IsEmpty,

            // Electrical
            VoltageTypeSet enumerable => enumerable.IsEmpty,
            WireTypeSet enumerable => enumerable.IsEmpty,
            WireSet enumerable => enumerable.IsEmpty,
            WireConduitTypeSet enumerable => enumerable.IsEmpty,
            DistributionSysTypeSet enumerable => enumerable.IsEmpty,
#if !REVIT2027_OR_GREATER
            InsulationTypeSet enumerable => enumerable.IsEmpty,
            TemperatureRatingTypeSet enumerable => enumerable.IsEmpty,
            WireSizeSet enumerable => enumerable.IsEmpty,
            WireMaterialTypeSet enumerable => enumerable.IsEmpty,
            GroundConductorSizeSet enumerable => enumerable.IsEmpty,
            CorrectionFactorSet enumerable => enumerable.IsEmpty,
#endif

            // Rendering
            AssetSet enumerable => enumerable.IsEmpty,
            CitySet enumerable => enumerable.IsEmpty,

            // Printing
            PaperSizeSet enumerable => enumerable.IsEmpty,
            PaperSourceSet enumerable => enumerable.IsEmpty,

            // Other
            HashSet<ElementId> enumerable => enumerable.Count == 0,
            HashSet<ElectricalSystem> enumerable => enumerable.Count == 0,
            MacroManager enumerable => enumerable.Count == 0,
            _ => !Enumerator.MoveNext()
        };
    }

    public IEnumerator Enumerator { get; }
    public bool IsEmpty { get; }

    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(IEnumerable.GetEnumerator) => ResolveGetEnumerator,
            _ => null
        };

        IVariant ResolveGetEnumerator()
        {
            return Variants.Empty<IEnumerator>();
        }
    }
}