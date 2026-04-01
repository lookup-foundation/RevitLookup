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
using System.ComponentModel;
using System.Windows;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.PointClouds;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using LookupEngine.Abstractions.Decomposition;
using LookupEngine.Descriptors;
using RevitLookup.Core.Decomposition.Descriptors;
using Document = Autodesk.Revit.Creation.Document;
using EnumerableDescriptor = RevitLookup.Core.Decomposition.Descriptors.EnumerableDescriptor;
using RibbonItem = Autodesk.Revit.UI.RibbonItem;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;

namespace RevitLookup.Core.Decomposition;

public static class DescriptorsMap
{
    /// <summary>
    ///     Search for a descriptor by approximate or exact match
    /// </summary>
    /// <remarks>
    ///     Exact search is necessary for the reflection engine, to add extensions and resolve conflicts when calling methods and properties. Type is not null <p />
    ///     An approximate search is needed to describe the object, which is displayed to the user. Type is null
    /// </remarks>
    public static Descriptor FindDescriptor(object? obj, Type? type)
    {
        return obj switch
        {
            //System
            string value when type is null || type == typeof(string) => new StringDescriptor(value),
            bool value when type is null || type == typeof(bool) => new BooleanDescriptor(value),
            IEnumerable value and not CurveLoop => new EnumerableDescriptor(value),
            Exception value when type is null || type == typeof(Exception) => new ExceptionDescriptor(value),

            //Root
            ElementId value when type is null || type == typeof(ElementId) => new ElementIdDescriptor(value),
            GuidEnum value when type is null || type == typeof(GuidEnum) => new GuidEnumDescriptor(value),
            ExternalDefinition value when type is null || type == typeof(ExternalDefinition) => new ExternalDefinitionDescriptor(value),
            Definition value when type is null || type == typeof(Definition) => new DefinitionDescriptor(value),
            XYZ value when type is null || type == typeof(XYZ) => new XyzDescriptor(value),

            //Enumerator
            DefinitionBindingMapIterator value => new DefinitionBindingMapIteratorDescriptor(value),
            // IEnumerator value => new EnumeratorDescriptor(value),

            //APIObjects
            BoundingBoxXYZ value when type is null || type == typeof(BoundingBoxXYZ) => new BoundingBoxXyzDescriptor(value),
            Category value when type is null || type == typeof(Category) => new CategoryDescriptor(value),
            Parameter value when type is null || type == typeof(Parameter) => new ParameterDescriptor(value),
            FamilyParameter value when type is null || type == typeof(FamilyParameter) => new FamilyParameterDescriptor(value),
            Reference value when type is null || type == typeof(Reference) => new ReferenceDescriptor(value),
            Color value when type is null || type == typeof(Color) => new ColorDescriptor(value),
            Curve value when type is null || type == typeof(Curve) => new CurveDescriptor(value),
            Edge value when type is null || type == typeof(Edge) => new EdgeDescriptor(value),
            EdgeEndPoint value when type is null || type == typeof(EdgeEndPoint) => new EdgeEndPointDescriptor(value),
            Solid value when type is null || type == typeof(Solid) => new SolidDescriptor(value),
            Surface value when type is null || type == typeof(Surface) => new SurfaceDescriptor(value),
            Mesh value when type is null || type == typeof(Mesh) => new MeshDescriptor(value),
            CylindricalFace value when type is null || type == typeof(CylindricalFace) => new CylindricalFaceDescriptor(value),
            CurveLoop value when type is null || type == typeof(CurveLoop) => new CurveLoopDescriptor(value),
            Face value when type is null || type == typeof(Face) => new FaceDescriptor(value),
            GeometryObject value when type is null || type == typeof(GeometryObject) => new GeometryObjectDescriptor(value),
            City value when type is null || type == typeof(City) => new CityDescriptor(value),
            PaperSize value when type is null || type == typeof(PaperSize) => new PaperSizeDescriptor(value),
            PrintManager value when type is null || type == typeof(PrintManager) => new PrintManagerDescriptor(value),
            DefinitionFile value when type is null || type == typeof(DefinitionFile) => new DefinitionFileDescriptor(value),
            DefinitionGroup value when type is null || type == typeof(DefinitionGroup) => new DefinitionGroupDescriptor(value),
            FamilyManager value when type is null || type == typeof(FamilyManager) => new FamilyManagerDescriptor(value),
            MEPSection value when type is null || type == typeof(MEPSection) => new MepSectionDescriptor(value),
            LocationCurve value when type is null || type == typeof(LocationCurve) => new LocationCurveDescriptor(value),
            CurtainGrid value when type is null || type == typeof(CurtainGrid) => new CurtainGridDescriptor(value),
            Document value when type is null || type == typeof(Document) => new DocumentCreationDescriptor(value),
            APIObject when type is null || type == typeof(APIObject) => new ApiObjectDescriptor(),

            //Elements
            Panel value when type is null || type == typeof(Panel) => new PanelDescriptor(value),
            FamilyInstance value when type is null || type == typeof(FamilyInstance) => new FamilyInstanceDescriptor(value),
            FamilySymbol value when type is null || type == typeof(FamilySymbol) => new FamilySymbolDescriptor(value),
            Family value when type is null || type == typeof(Family) => new FamilyDescriptor(value),
            ViewSchedule value when type is null || type == typeof(ViewSchedule) => new ViewScheduleDescriptor(value),
            TableView value when type is null || type == typeof(TableView) => new TableViewDescriptor(value),
            View value when type is null || type == typeof(View) => new ViewDescriptor(value),
            Wire value when type is null || type == typeof(Wire) => new WireDescriptor(value),
            Duct value when type is null || type == typeof(Duct) => new DuctDescriptor(value),
            Pipe value when type is null || type == typeof(Pipe) => new PipeDescriptor(value),
            Wall value when type is null || type == typeof(Wall) => new WallDescriptor(value),
            HostObject value when type is null || type == typeof(HostObject) => new HostObjectDescriptor(value),
            ElevationMarker value when type is null || type == typeof(ElevationMarker) => new ElevationMarkerDescriptor(value),
            AssemblyInstance value when type is null || type == typeof(AssemblyInstance) => new AssemblyInstanceDescriptor(value),
            RevitLinkType value when type is null || type == typeof(RevitLinkType) => new RevitLinkTypeDescriptor(value),
            AnalyticalLinkType value when type is null || type == typeof(AnalyticalLinkType) => new AnalyticalLinkTypeDescriptor(value),
            SpatialElement value when type is null || type == typeof(SpatialElement) => new SpatialElementDescriptor(value),
            SunAndShadowSettings value when type is null || type == typeof(SunAndShadowSettings) => new SunAndShadowSettingsDescriptor(value),
            IndependentTag value when type is null || type == typeof(IndependentTag) => new IndependentTagDescriptor(value),
            MEPSystem value when type is null || type == typeof(MEPSystem) => new MepSystemDescriptor(value),
            BasePoint value when type is null || type == typeof(BasePoint) => new BasePointDescriptor(value),
            InternalOrigin value when type is null || type == typeof(InternalOrigin) => new InternalOriginDescriptor(value),
            StructuralSettings value when type is null || type == typeof(StructuralSettings) => new StructuralSettingsDescriptor(value),
            AreaVolumeSettings value when type is null || type == typeof(AreaVolumeSettings) => new AreaVolumeSettingsDescriptor(value),
            Rebar value when type is null || type == typeof(Rebar) => new RebarDescriptor(value),
            RebarShape value when type is null || type == typeof(RebarShape) => new RebarShapeDescriptor(value),
            CurveElement value when type is null || type == typeof(CurveElement) => new CurveElementDescriptor(value),
            GlobalParameter value when type is null || type == typeof(GlobalParameter) => new GlobalParameterDescriptor(value),
            ReferencePoint value when type is null || type == typeof(ReferencePoint) => new ReferencePointDescriptor(value),
            DatumPlane value when type is null || type == typeof(DatumPlane) => new DatumPlaneDescriptor(value),
            Part value when type is null || type == typeof(Part) => new PartDescriptor(value),
            PartMaker value when type is null || type == typeof(PartMaker) => new PartMakerDescriptor(value),
#if REVIT2022_OR_GREATER
            RevisionNumberingSequence value when type is null || type == typeof(RevisionNumberingSequence) => new RevisionNumberingSequenceDescriptor(value),
#endif
            WallType value when type is null || type == typeof(WallType) => new WallTypeDescriptor(value),
            ElementType value when type is null || type == typeof(ElementType) => new ElementTypeDescriptor(value),
            Element value when type is null || type == typeof(Element) => new ElementDescriptor(value),

            //IDisposables
            Autodesk.Revit.DB.Document value when type is null || type == typeof(Autodesk.Revit.DB.Document) => new DocumentDescriptor(value),
            PlanViewRange value when type is null || type == typeof(PlanViewRange) => new PlanViewRangeDescriptor(value),
            ForgeTypeId value when type is null || type == typeof(ForgeTypeId) => new ForgeTypeIdDescriptor(value),
            Entity value when type is null || type == typeof(Entity) => new EntityDescriptor(value),
            Field value when type is null || type == typeof(Field) => new FieldDescriptor(value),
            Schema value when type is null || type == typeof(Schema) => new SchemaDescriptor(value),
            FailureMessage value when type is null || type == typeof(FailureMessage) => new FailureMessageDescriptor(value),
            UpdaterInfo value when type is null || type == typeof(UpdaterInfo) => new UpdaterInfoDescriptor(value),
            Subelement value when type is null || type == typeof(Subelement) => new SubelementDescriptor(value),
            ExternalResourceReference value when type is null || type == typeof(ExternalResourceReference) => new ExternalResourceReferenceDescriptor(value),
            ExternalResourceType value when type is null || type == typeof(ExternalResourceType) => new ExternalResourceTypeDescriptor(value),
            ExternalService value when type is null || type == typeof(ExternalService) => new ExternalServiceDescriptor(value),
            LightFamily value when type is null || type == typeof(LightFamily) => new LightFamilyDescriptor(value),
            Autodesk.Revit.ApplicationServices.Application value when type is null || type == typeof(Autodesk.Revit.ApplicationServices.Application) => new ApplicationDescriptor(value),
            UIApplication when type is null || type == typeof(UIApplication) => new UiApplicationDescriptor(),
            PerformanceAdviser value when type is null || type == typeof(PerformanceAdviser) => new PerformanceAdviserDescriptor(value),
            SchedulableField value when type is null || type == typeof(SchedulableField) => new SchedulableFieldDescriptor(value),
            CompoundStructureLayer value when type is null || type == typeof(CompoundStructureLayer) => new CompoundStructureLayerDescriptor(value),
            ModelPath value when type is null || type == typeof(ModelPath) => new ModelPathDescriptor(value),
            Workset value when type is null || type == typeof(Workset) => new WorksetDescriptor(value),
            WorksetTable when type is null || type == typeof(WorksetTable) => new WorksetTableDescriptor(),
            BoundarySegment value when type is null || type == typeof(BoundarySegment) => new BoundarySegmentDescriptor(value),
            AssetProperties value when type is null || type == typeof(AssetProperties) => new AssetPropertiesDescriptor(value),
            AssetProperty value when type is null || type == typeof(AssetProperty) => new AssetPropertyDescriptor(value),
            Connector value when type is null || type == typeof(Connector) => new ConnectorDescriptor(value),
            ConnectorManager value when type is null || type == typeof(ConnectorManager) => new ConnectorManagerDescriptor(value),
            ScheduleDefinition value when type is null || type == typeof(ScheduleDefinition) => new ScheduleDefinitionDescriptor(value),
            TableData value when type is null || type == typeof(TableData) => new TableDataDescriptor(value),
            TableSectionData value when type is null || type == typeof(TableSectionData) => new TableSectionDataDescriptor(value),
            FamilySizeTableManager value when type is null || type == typeof(FamilySizeTableManager) => new FamilySizeTableManagerDescriptor(value),
            FamilySizeTable value when type is null || type == typeof(FamilySizeTable) => new FamilySizeTableDescriptor(value),
            FamilySizeTableColumn value when type is null || type == typeof(FamilySizeTableColumn) => new FamilySizeTableColumnDescriptor(value),
            PointCloudFilter value when type is null || type == typeof(PointCloudFilter) => new PointCloudFilterDescriptor(value),
            TriangulationInterface value when type is null || type == typeof(TriangulationInterface) => new TriangulationInterfaceDescriptor(value),
            Autodesk.Revit.DB.Units value when type is null || type == typeof(Autodesk.Revit.DB.Units) => new UnitsDescriptor(value),
            CompoundStructure value when type is null || type == typeof(CompoundStructure) => new CompoundStructureDescriptor(value),
            FailureDefinitionAccessor value when type is null || type == typeof(FailureDefinitionAccessor) => new FailureDefinitionAccessorDescriptor(value),
#if REVIT2024_OR_GREATER
            EvaluatedParameter value when type is null || type == typeof(EvaluatedParameter) => new EvaluatedParameterDescriptor(value),
#endif
            IDisposable when type is null || type == typeof(IDisposable) => new DisposableDescriptor(), //Faster then obj.GetType().Namespace == "Autodesk.Revit.DB"

            //Media
            System.Windows.Media.Color value when type is null || type == typeof(System.Windows.Media.Color) => new ColorMediaDescriptor(value),

            //ComponentManager
            UIElement value when type is null || type == typeof(UIElement) => new UiElementDescriptor(value),
            DependencyObject value when type is null || type == typeof(DependencyObject) => new DependencyObjectDescriptor(value),
            RibbonItem value when type is null || type == typeof(RibbonItem)=> new RibbonItemDescriptor(value),
            RibbonPanel value when type is null || type == typeof(RibbonPanel) => new RibbonPanelDescriptor(value),
            Autodesk.Windows.RibbonItem value when type is null || type == typeof(Autodesk.Windows.RibbonItem)=> new RibbonItemDescriptor(value),
            Autodesk.Windows.RibbonPanel value when type is null || type == typeof(Autodesk.Windows.RibbonPanel)=> new RibbonPanelDescriptor(value),
            RibbonTab value when type is null || type == typeof(RibbonTab)=> new RibbonTabDescriptor(value),
            INotifyPropertyChanged => new UiObjectDescriptor(),

            //Unknown
            _ => new ObjectDescriptor(obj)
        };
    }
}