# Changelog

# **2027.0.3-preview.1**

## On-demand member evaluation

Earlier versions evaluated every member as soon as you opened the RevitLookup window.
That was risky, because RevitLookup would call terminal methods and methods that modify the object without asking, and for large objects you had to wait while hundreds of slow calls ran. 
Methods that return `void` could not be executed at all.
You could see them in the list, but RevitLookup had no way to run them, so you could never invoke one or confirm it worked.

Evaluation now happens on demand https://github.com/lookup-foundation/RevitLookup/issues/298. </br>
When an object opens, RevitLookup evaluates only the members it considers safe to call, now it's members from `System` and `Autodesk.Revit` namespaces. </br>
Everything else is **deferred** until you ask to evaluate. Opening even heavy objects stays fast, and you decide which expensive or model-changing members actually run https://github.com/lookup-foundation/RevitLookup/issues/239.

Methods that return `void` are now runnable too, handled as deferred members like everything else, so you can finally invoke `View.Print` or similar methods.

To evaluate a deferred member, two new commands were added to the row context menu:

- **Evaluate** (`F8`) runs the member.
- **Evaluate with transaction** (`Alt + F8`) runs the member inside a Revit transaction, which some members require in order to execute. Any changes the member makes are committed to the document.

If you hit a member that should be evaluated automatically, or one that should not, please report it in the **Discussions** for this release.

![image](https://github.com/user-attachments/assets/2ac6d7b9-05ec-452b-9269-8d0cbd872f91)

## Readable value states

The values table used to render everything as plain text.
A member that throws an Exception showed a red row spanning the whole width, which was easy to misread and looked broken for anyone running a red system accent color, while special states like "\<null\>" were just more text that blended in with real values. 
Each state now has its own icon and label, so you can tell at a glance what a row actually is:

- **Awaiting**. A deferred member that has not been evaluated yet.
- **No return value**. A `void` method that has been successfully evaluated.
- **Disabled**. A member whose evaluation is permanently disabled.
- **Unsupported**. A member the engine cannot evaluate (the overload is not supported, for example).
- **Exception**. A member that threw, marked with an error icon and the message instead of a full red row.

![image](https://github.com/user-attachments/assets/10a8a69f-8a20-4847-846d-25fae14d9da1)

## Improvements

- **Colors shown as HEX**. Color values are now displayed as `#FF7F00` instead of `RGB: 255 127 0`.
- **Faster Views**. `View.CanCategoryBeHidden`, `View.CanCategoryBeHiddenTemporary`, and `View.IsCategoryOverridable` are now deferred, so opening a View no longer waits on these slow calls.
- **Smoother geometry visualization**. Mesh vertex normals are now precomputed in a single pass and curve loop vertices are de-duplicated.
- **Lower visualization memory use**. Rendering buffers are now released when a visualization is removed or remapped.
- **Faster settings filtering**. The settings filter and configuration lookup were reworked to avoid recompiling expressions and linear scans.
- **Updated LookupEngine** with the new on-demand evaluation pipeline and engine syntax.

## Documentation

- **Wiki sources moved into the repository**. [`Wiki`](https://github.com/lookup-foundation/RevitLookup/tree/develop/wiki) is available for public editing with your pull requests. [The public wiki](https://github.com/lookup-foundation/RevitLookup/wiki) is automatically synchronized with the repository.

# **2027.0.2**

- Added `Rebar.GetFullGeometryForView` support https://github.com/lookup-foundation/RevitLookup/issues/413.
- Added **WinGet** installation method https://github.com/lookup-foundation/RevitLookup/wiki/versions#installation.
- Changed the "Project website" link to the new Autodesk blog https://github.com/lookup-foundation/RevitLookup/issues/396.
- Changed the installer manufacturer from "Autodesk" to "Lookup Foundation".

# **2027.0.1**

- Fixed extensions. CoordinationModelLinkUtils extension is now available starting from Revit 2026.3 patch only.
- Fixed the Delete element command using the wrong thread.
- Fixed the visualization buffer for Solid and Mesh geometry.
- Fixed the visualization thread lock for Solid and Mesh geometry.
- Fixed the default visualization solid scale.

# **2027.0.0**

Revit 2027 is here, and so is a major update for RevitLookup!

The Revit API has hundreds of static utility methods that were previously hard to find. We've now linked these "Utils" directly to the elements and types they affect. You can now see all related utility data for an object in one place, without having to search for static methods elsewhere.

## General

- **Revit 2027** support.
- **Enabled addin isolation** starting from Revit 2027. Finally, Autodesk fixed it.
- **Async startup** makes Revit to continue loading while RevitLookup initializes in the background.
- **Event Monitor** is now faster when inspecting incoming events.
- **Shortcuts** no longer conflict with user settings. The default shortcut is only applied if the user has not configured one. 
    There are now two hotkeys to open the Snoop Selection window: `F12` and `SS`. https://github.com/lookup-foundation/RevitLookup/issues/316.

## Postable commands

Postable commands are Revit's built-in UI actions — toolbar buttons, menu items, dialog triggers — that can be invoked programmatically via the PostCommand API. 
They're the go-to tool when you need to automate UI behavior that has no direct API equivalent.
RevitLookup now exposes the full list in one place: browse, search, and execute any Postable command directly.

![image](https://github.com/user-attachments/assets/2441f8b7-eeb5-4787-879d-55309c2f94d1)

## API Coverage

This release covers most of the Revit API Utils methods. Hundreds of new members are now visible directly in the inspector.

### New supported types

- `AssemblyInstance`
- `Connector`
- `DefinitionFile`
- `DocumentCreation`
- `Duct`
- `EdgeEndPoint`
- `ExternalDefinition`
- `ExternalResourceReference`
- `ExternalResourceType`
- `FailureDefinitionAccessor`
- `FamilySymbol`
- `GeometryObject`
- `GlobalParameter`
- `ModelPath`
- `PointCloudFilter`
- `Rebar`
- `RebarShape`
- `ReferencePoint`
- `SketchPlane`
- `Subelement`
- `Surface`
- `TriangulationInterface`
- `Units`
- `WallType`

### Application extensions

- `PointCloudEngineRegistry.GetSupportedEngines`
- `OptionalFunctionalityUtils.IsDGNExportAvailable`
- `OptionalFunctionalityUtils.IsDGNImportLinkAvailable`
- `OptionalFunctionalityUtils.IsDWFExportAvailable`
- `OptionalFunctionalityUtils.IsDWGExportAvailable`
- `OptionalFunctionalityUtils.IsDWGImportLinkAvailable`
- `OptionalFunctionalityUtils.IsDXFExportAvailable`
- `OptionalFunctionalityUtils.IsFBXExportAvailable`
- `OptionalFunctionalityUtils.IsGraphicsAvailable`
- `OptionalFunctionalityUtils.IsIFCAvailable`
- `OptionalFunctionalityUtils.IsNavisworksExporterAvailable`
- `OptionalFunctionalityUtils.IsSATImportLinkAvailable`
- `OptionalFunctionalityUtils.IsShapeImporterAvailable`
- `OptionalFunctionalityUtils.IsSKPImportLinkAvailable`
- `OptionalFunctionalityUtils.Is3DMImportLinkAvailable`
- `OptionalFunctionalityUtils.IsOBJImportLinkAvailable`
- `OptionalFunctionalityUtils.IsSTLImportLinkAvailable`
- `OptionalFunctionalityUtils.IsSTEPImportLinkAvailable`
- `OptionalFunctionalityUtils.IsMaterialLibraryAvailable`
- `ModelPathUtils.GetAllCloudRegions`
- `UIThemeManager.GetThemeName`
- `LabelUtils.GetFailureSeverityName`
- `LabelUtils.GetStructuralSectionShapeName`

### Document extensions

- `GlobalParametersManager.AreGlobalParametersAllowed`
- `GlobalParametersManager.GetGlobalParametersOrdered`
- `ExportUtils.GetGBXMLDocumentId`
- `UpdaterRegistry.GetRegisteredUpdaterInfos`
- `CoordinationModelLinkUtils.GetAllCoordinationModelInstanceIds`
- `CoordinationModelLinkUtils.GetAllCoordinationModelTypeIds`
- `ExternalFileUtils.GetAllExternalFileReferences`
- `ExternalResourceUtils.GetAllExternalResourceReferences`
- `AssemblyCodeTable.GetAssemblyCodeTable`
- `KeynoteTable.GetKeynoteTable`

### Element extensions

- `DirectContext3DDocumentUtils.IsADirectContext3DHandleInstance`
- `DirectContext3DDocumentUtils.IsADirectContext3DHandleType`
- `ElementIntersectsFilter.IsCategorySupported`
- `ElementIntersectsFilter.IsElementSupported`
- `ExportUtils.GetExportId`
- `ExternalFileUtils.GetExternalFileReference`
- `ExternalFileUtils.IsExternalFileReference`
- `InstanceVoidCutUtils.CanBeCutWithVoid`
- `InstanceVoidCutUtils.GetCuttingVoidInstances`
- `DetailElementOrderUtils.IsDetailElement`
- `RebarBendingDetail.IsBendingDetail`
- `RebarBendingDetail.GetHost`
- `RebarBendingDetail.GetPosition`
- `RebarBendingDetail.GetRotation`
- `RebarBendingDetail.IsRealisticBendingDetail`
- `RebarBendingDetail.IsSchematicBendingDetail`
- `RebarBendingDetail.GetHosts`
- `RebarBendingDetail.GetTagRelativePosition`
- `RebarBendingDetail.GetTagRelativeRotation`
- `AnnotationMultipleAlignmentUtils.IsMultiAlignSupported`
- `AnnotationMultipleAlignmentUtils.GetAnnotationOutlineWithoutLeaders`
- `CoordinationModelLinkUtils.IsCoordinationModelInstance`
- `CoordinationModelLinkUtils.IsCoordinationModelType`

### ElementType extensions

- `CoordinationModelLinkUtils.GetCoordinationModelTypeData`
- `CoordinationModelLinkUtils.GetTransparencyOverride`
- `RebarCrankTypeUtils.GetCrankLengthMultiplier`
- `RebarCrankTypeUtils.GetCrankOffsetMultiplier`
- `RebarCrankTypeUtils.GetCrankRatio`
- `RebarSpliceTypeUtils.GetLapLengthMultiplier`
- `RebarSpliceTypeUtils.GetShiftOption`
- `RebarSpliceTypeUtils.GetStaggerLengthMultiplier`

### FamilyInstance extensions

- `MassLevelData.IsMassFamilyInstance`
- `MassInstanceUtils.GetMassLevelDataIds`
- `MassInstanceUtils.GetMassLevelIds`
- `MassInstanceUtils.GetGrossFloorArea`
- `MassInstanceUtils.GetGrossSurfaceArea`
- `MassInstanceUtils.GetGrossVolume`
- `MassInstanceUtils.GetJoinedElementIds`
- `StructuralSectionUtils.GetStructuralSection`
- `StructuralSectionUtils.GetStructuralElementDefinitionData`
- `AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds`
- `AdaptiveComponentInstanceUtils.GetInstanceShapeHandlePointElementRefIds`
- `AdaptiveComponentInstanceUtils.HasAdaptiveFamilySymbol`
- `AdaptiveComponentInstanceUtils.IsInstanceFlipped`
- `StructuralFramingUtils.CanFlipEnds`
- `StructuralFramingUtils.IsJoinAllowedAtEnd`

### Family extensions

- `FamilyUtils.GetProfileSymbols`
- `FamilyUtils.FamilyCanConvertToFaceHostBased`
- `AdaptiveComponentFamilyUtils.IsAdaptiveComponentFamily`
- `AdaptiveComponentFamilyUtils.GetNumberOfAdaptivePoints`
- `AdaptiveComponentFamilyUtils.GetNumberOfPlacementPoints`
- `AdaptiveComponentFamilyUtils.GetNumberOfShapeHandlePoints`
- `LoadedFamilyIntegrityCheck.CheckFamily`

### View extensions

- `ReferenceableViewUtils.GetReferencedViewId`
- `View.GetInstances`

### Category extensions

- `SSEPointVisibilitySettings.GetVisibility`
- `DirectContext3DDocumentUtils.IsADirectContext3DHandleCategory`
- `DirectContext3DDocumentUtils.GetDirectContext3DHandleInstances`
- `DirectContext3DDocumentUtils.GetDirectContext3DHandleTypes`
- `ParameterFilterUtilities.GetAllFilterableCategories`
- `ParameterFilterUtilities.GetFilterableParametersInCommon`

### CurveElement extensions

- `CurveByPointsUtils.GetHostFace`
- `CurveByPointsUtils.GetProjectionType`
- `CurveByPointsUtils.GetSketchOnSurface`

### ReferencePoint extensions

- `AdaptiveComponentFamilyUtils.IsAdaptivePoint`
- `AdaptiveComponentFamilyUtils.IsAdaptivePlacementPoint`
- `AdaptiveComponentFamilyUtils.IsAdaptiveShapeHandlePoint`
- `AdaptiveComponentFamilyUtils.GetPlacementNumber`
- `AdaptiveComponentFamilyUtils.GetPointConstraintType`
- `AdaptiveComponentFamilyUtils.GetPointOrientationType`

### GeometryObject extensions

- `ExternallyTaggedGeometryValidation.IsNonSolid`
- `ExternallyTaggedGeometryValidation.IsSolid`
- `ExternallyTaggedGeometryValidation.LacksSubnodes`

### ForgeTypeId extensions

- `SpecUtils.IsValidDataType`
- `UnitUtils.IsValidUnit`
- `UnitUtils.GetDiscipline`
- `UnitUtils.GetValidUnits`
- `UnitUtils.GetAllUnits`
- `UnitUtils.GetAllMeasurableSpecs`
- `UnitUtils.GetAllDisciplines`
- `SpecUtils.GetAllSpecs`
- `UnitUtils.GetTypeCatalogStringForSpec`
- `UnitUtils.GetTypeCatalogStringForUnit`
- `ParameterUtils.GetBuiltInParameter`
- `ParameterUtils.GetAllBuiltInParameters`
- `ParameterUtils.GetAllBuiltInGroups`
- `ParameterUtils.GetBuiltInParameterGroupTypeId`
- `ParameterUtils.DownloadParameterOptions`
- `ParameterUtils.DownloadParameter`
- `ParameterUtils.DownloadCompanyName`

### Solid extensions

- `SolidUtils.ComputeIsGeometricallyClosed`
- `SolidUtils.ComputeIsTopologicallyClosed`
- `SolidUtils.TessellateSolidOrShell`

### Other extensions

- `AssemblyViewUtils.AcquireAssemblyViews`
- `FabricationUtils.ValidateConnectivity`
- `PlumbingUtils.HasOpenConnector`
- `RebarSpliceUtils.GetSpliceChain`
- `RebarShapeParameters.GetAllRebarShapeParameters`
- `RebarShapeParameters.IsValidExternalDefinition`
- `RebarShapeParameters.GetElementIdForExternalDefinition`
- `RebarShapeParameters.GetOrCreateElementIdForExternalDefinition`
- `ExternalResourceServiceUtils.GetServersByType`
- `ExportUtils.GetNurbsSurfaceDataForSurface`
- `FacetingUtils.ConvertTrianglesToQuads`
- `SolidUtils.FindAllEdgeEndPointsAtVertex`
- `ColorRepresentationUtils.GetColorName`
- `CoordinationModelLinkUtils.IsCoordinationModelInstance`
- `CoordinationModelLinkUtils.IsCoordinationModelType`
- `AnnotationMultipleAlignmentUtils.ElementSupportsMultiAlign`
- `AnnotationMultipleAlignmentUtils.GetAnnotationOutlineWithoutLeaders`

### FamilyInstance extensions

- `StructuralSectionUtils.GetStructuralSection`
- `StructuralSectionUtils.GetStructuralElementDefinitionData`
- `AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds`
- `AdaptiveComponentInstanceUtils.GetInstanceShapeHandlePointElementRefIds`
- `AdaptiveComponentInstanceUtils.HasAdaptiveFamilySymbol`
- `AdaptiveComponentInstanceUtils.IsInstanceFlipped`
- `MassLevelData.IsMassFamilyInstance`
- `MassInstanceUtils.GetMassGrossFloorArea`
- `MassInstanceUtils.GetMassGrossSurfaceArea`
- `MassInstanceUtils.GetMassGrossVolume`
- `MassInstanceUtils.GetMassJoinedElementIds`
- `MassInstanceUtils.GetMassLevelDataIds`
- `MassInstanceUtils.GetMassLevelIds`
- `StructuralFramingUtils.CanFlipFramingEnds`
- `StructuralFramingUtils.IsFramingJoinAllowedAtEnd`
- `StructuralFramingUtils.CanSetFramingEndReference`
- `StructuralFramingUtils.GetFramingEndReference`

### Family extensions

- `LoadedFamilyIntegrityCheck.CheckFamily`
- `AdaptiveComponentFamilyUtils.IsAdaptiveComponentFamily`
- `AdaptiveComponentFamilyUtils.GetNumberOfAdaptivePoints`
- `AdaptiveComponentFamilyUtils.GetNumberOfPlacementPoints`
- `AdaptiveComponentFamilyUtils.GetNumberOfShapeHandlePoints`

### View extensions

- `ReferenceableViewUtils.GetReferencedViewId`
- `View.GetInstances`

## Development

- Rewrote external events using the `[ExternalEvent]` source generator.
- Refactored the decomposition architecture to use the new extensions' syntax.
- Refactored `UiOrchestratorService`, removed extra memory allocations.
- Added utils to unify object decomposition and member evaluation.
- Added `ServiceDefaults` shared project with host configurations.
- Added unit tests for Revit API inheritance and static method coverage.
- Migrated application startup and shutdown to async implementation.
- Migrated MVVM properties to the new CommunityToolkit partial property syntax.
- Optimized descriptor mapping, significantly reducing overhead when analyzing complex object trees.
- Optimized the event subscription model to save memory and keep the UI responsive.
- Optimized UI converters to Avalonia-like style.

# 2025-07-26 **2026.0.1**

A new minor release with a few bug fixes and improvements.

- Added description support for searching items.
- Added units sorting by built-in name.
- Added new Revit API unit tests.
- Changed the addin vendor. It is now `Lookup Foundation`.
- Disabled addin isolation for Revit 2026. Requires fixes from Autodesk.
- Disabled `DefaultHttpClientFactory` logs.
- Improved thread runner orchestrator.
- Fixed display names of built-in parameters and categories https://github.com/lookup-foundation/RevitLookup/discussions/183

# 2025-04-02 **2026.0.0**

A new major RevitLookup update with official Revit 2026 support and a lot of enhancements 🎉

This release focuses on performance, a renewed UI, expanded functionality, redesigned application architecture, clearer separation of core components, and user flow improvements. Let's move to the details.

## General

- **Revit 2026** support.
- **Added shortcuts**. `SS` shortcut opens the Snoop Selection window.
- **Unlocked Ribbon buttons** for document-independent actions. Some dialogs can be opened without opening a document.
- **Improved context actions**. Now `Delete` command shows the number of deleted Elements from the document.
- **Improved exception handling**. Now shows more informational messages, added more supported error types. Fixed some application crashes.
- **Fixed dependency conflicts** for .NET Framework versions of RevitLookup, according to tests, you should no longer encounter startup issues.
- **Added AppBundle**, that can be installed with all available versions in the release.
- **Added user manuals**, visit the [Wiki](https://github.com/lookup-foundation/RevitLookup/wiki) for RevitLookup features.
- **Added digital signature**. Thanks [Kinship](https://kinship.io/) for providing cloud services!

## User Interface

### Dashboard page

The Dashboard has been significantly redesigned to make it easier to understand for users unfamiliar with the Revit API.

- **Added description** of all actions, navigation became more intuitive and clear.
- **Added UiControlledApplication** action to analyze properties and methods available when running the add-in.
- **Actions now grouped** according to the window size.

![image](https://github.com/user-attachments/assets/141192fb-37e4-4278-8816-3db65377936b)

### Summary page

Updated and improved Summary page components.

- **Improved responsiveness** of the whole page. RevitLookup now supports displaying large amounts of data, 100k rows will not cause freezes and glitches.
- **Improved shortcuts**, now the **F5** key works in all cases, previously it was required to focus on an element.
- **Improved filter menu**. Now applying **Show/hide** filters does not close the menu.
- **Improved tooltips**. Tooltips now display more useful information, number of elements contained in the type, full type name.
- **Improved search**. It is now more intuitive and faster. Priority is given to the members table, the table is filtered first, then if nothing is found, the list of items is filtered. When searching the table of members, the elements with the same type will be filtered as well.
- **Show Extension** filter is now enabled by default.
- **Patched WPF** issues causing non-obvious member's table behavior.

![image](https://github.com/user-attachments/assets/4bf71a83-3adb-4254-95b7-3d62b303d1bb)

### Settings page

- **Added description** of all settings, navigation became more intuitive and clear.

![image](https://github.com/user-attachments/assets/95002ea8-6200-4706-933b-a4bef4ee4606)

### Unit dialogs

- **Improved context menu**, Copy commands are grouped into sub-menu.

### Modules dialog

- **Added new context menu** action. Added navigation to the module directory or directly to the module file.
- **Added column sorting**. Click on the column header to sort.
- **Improved context menu**, Copy commands are grouped into sub-menu.

![image](https://github.com/user-attachments/assets/4b5d5db0-f9a3-4be1-83fe-9549cbe7337f)

### Application

- **Adjusted default window sizes** for better display due to changes in Win.UI components.
- **Synchronised UI library** and styles with Wpf.UI.
- **Improved theme synchronization** with Windows.
- **Added a new high contrast** theme.

## LookupEngine

RevitLookup now runs on its own [engine](https://github.com/lookup-foundation/LookupEngine).
The engine can run outside Revit, and can be reused across the entire family of Autodesk products.
This is a pledge for the future for products like **AutocadLookup**, **InventorLookup** and others. 

Isolating the engine also brings many new improvements:

- **The engine is much faster** than the integrated version, it allocates less memory, and supports more features.
- **Added context support**, now allows additional metadata to be used to resolve members.
- **Fixed CLR exceptions** for critical .NET Core components when evaluating members.
- **New Lookup Engine API**. Isolation encouraged independent use of the API in any application.
- **Improved context detection**, objects now understand what context they are in and provide values specific to their context, not just the active context.
- **Added navigation stack**, now it will be easier to add support for new unsupported parent-dependent methods.

## Improvements

- **Revit 2025.4 patch** brings some fixes to the API, now some methods and properties are available again for use in RevitLookup:
    - `DatumPlane.CanBeVisibleInView`
    - `DatumPlane.GetPropagationViews`
    - `IndependentTag.TagText`
- Added new **View** extensions:
    - `GetAllPlacedInstances`
- Added new **Wall** method overloads by @SergeyNefyodov:
    - `IsWallCrossSectionValid`
- Added new **CompoundStructure** method overloads by @SergeyNefyodov:
    - `CanLayerBeStructuralMaterial`
    - `CanLayerBeVariable`
    - `CanLayerWidthBeNonZero`
    - `GetAdjacentRegions`
    - `GetCoreBoundaryLayerIndex`
    - `GetDeckEmbeddingType`
    - `GetDeckProfileId`
    - `GetLayerAssociatedToRegion`
    - `GetLayerFunction`
    - `GetLayerWidth`
    - `GetMaterialId`
    - `GetNumberOfShellLayers`
    - `GetOffsetForLocationLine`
    - `GetPreviousNonZeroLayerIndex`
    - `GetRegionEnvelope`
    - `GetRegionsAssociatedToLayer`
    - `GetSegmentCoordinate`
    - `GetSegmentOrientation`
    - `GetWallSweepsInfo`
    - `GetWidth`
    - `IsCoreLayer`
    - `IsRectangularRegion`
    - `IsSimpleRegion`
    - `IsStructuralDeck`
    - `ParticipatesInWrapping`

- Visualisation now supports the new [CurveLoop](https://github.com/lookup-foundation/RevitLookup/wiki/visualization#curveloop-visualization) type, thanks @JieGou for the implementation!

    ![image](https://github.com/user-attachments/assets/f4d935f5-2cfd-44d2-b1c7-d5fdc07e95a1)

## Development:

- **Redesigned project structure**:
    - Standalone [LookupEngine](https://github.com/lookup-foundation/LookupEngine.UI). The LookupEngine is now a separate project, independent of RevitLookup.
    - Standalone [LookupEngine.UI](https://github.com/lookup-foundation/LookupEngine.UI). The LookupEngine UI is now a separate project, independent of RevitLookup.
    - UI Playground for development and testing the UI outside the Revit, independently.
- **Moving to Central Package Management**, all dependencies are now managed in a single file.
- **Moving to Central Build Management**, common build configurations are now managed in a single file.
- **Automatic registration of MVVM components** in the service provider, no longer need to register each component manually.
- **.NET 9 SDK support** for development.
- **Migrated to SLNX solution format**, to avoid Git merge conflicts.
- **Changed API for ContextMenu**. Now it provides dependecies and execution context.
- **Disabled PresentationFramework traces** for the entire domain to address UI performance issues and freezing under an attached debugger, leaving only Critical events enabled.
- **Updated** [Contribution](https://github.com/lookup-foundation/RevitLookup/blob/dev/CONTRIBUTING.md) guide.

Made with love by @Nice3point 🕊️

# 2025-09-29 **2025.0.10**

- Fixed placeholder for the Dark theme https://github.com/jeremytammik/RevitLookup/issues/291
- Fixed the Revit.ini editor filter button name
- Fixed the Revit.ini editor filter placeholder
- Disabled the Visual.Enter() method https://github.com/jeremytammik/RevitLookup/issues/292
- Suppressed GenericHost startup messages by @Nefarion in https://github.com/jeremytammik/RevitLookup/pull/294

# 2025-09-22 **2025.0.9**

## Revit.ini File Editor

The **Revit.ini** file is a key configuration file in Revit that stores settings related to user preferences, system behavior, and project defaults.

The **Revit.ini File Editor** provides a simple and efficient way to manage these settings without the need for manual editing.
With this tool, users can quickly adjust Revit's configurations to match project needs or personal preferences, making it an essential utility for both professionals and teams
working with Revit.

![image](https://github.com/user-attachments/assets/701a0a97-1906-419d-950b-b70f9b852966)

This is our first public version, and we are excited for you to try it out for yourself!
Make sure to file issues you encounter on our GitHub so we can continue to improve it.

Documentation: https://github.com/jeremytammik/RevitLookup/wiki/revit-ini-file-editor

## Static dependency conflict analyzer

Some users experience issues launching RevitLookup, often caused by conflicts with third-party plugins (see issue https://github.com/jeremytammik/RevitLookup/issues/269).

To help resolve these issues, we've introduced new tools that allow you to analyze, identify and upgrade problematic plugins causing crashes.

![image](https://github.com/user-attachments/assets/a5f94bd8-7eca-4998-91c7-99d7b079fa47)

Download: https://github.com/jeremytammik/RevitLookup/issues/269#issuecomment-2323309590

Many thanks to @RichardPinka for testing tools in the discussion: https://github.com/jeremytammik/RevitLookup/issues/281

## Public RevitLookup roadmap

Curious about what's next? Stay updated on the latest developments for RevitLookup and share your feedback.

Check out our Public Roadmap to see what's coming up in future releases.: https://github.com/users/jeremytammik/projects/1

![image](https://github.com/user-attachments/assets/14c3479d-871a-4f32-a933-a4b365e566bc)

## Improvements

**New extensions**:

| Type        | Extension                       | Description                                                       | Author                                                                  |
|:------------|---------------------------------|-------------------------------------------------------------------|:------------------------------------------------------------------------|
| Pipe        | HasOpenConnector                | Checks if there is open piping connector for the pipe.            | https://github.com/jeremytammik/RevitLookup/pull/261 by @SergeyNefyodov |
| Family      | FamilyCanConvertToFaceHostBased | Indicates whether the family can be converted to face host based. | https://github.com/jeremytammik/RevitLookup/pull/263 by @SergeyNefyodov |
| Family      | GetProfileSymbols               | Gets the profile Family Symbols.                                  | https://github.com/jeremytammik/RevitLookup/pull/263 by @SergeyFyodorov |
| Document    | GetLightFamily                  | Creates a light family object from the family document.           | https://github.com/jeremytammik/RevitLookup/pull/266 by @SergeyNefyodov |
| LightFamily | GetLightTypeName                | Return the name for the light type.                               | https://github.com/jeremytammik/RevitLookup/pull/266 by @SergeyNefyodov |
| LightFamily | GetLightType                    | Return a LightType object for the light type.                     | https://github.com/jeremytammik/RevitLookup/pull/266 by @SergeyNefyodov |
| Application | GetMacroManager                 | Gets the Macro manager from the application.                      | https://github.com/jeremytammik/RevitLookup/pull/268 by @SergeyNefyodov |
| Document    | GetMacroManager                 | Gets the Macro manager from the document.                         | https://github.com/jeremytammik/RevitLookup/pull/268 by @SergeyNefyodov |

**New API support**:

- **CylindricalFace** class support https://github.com/jeremytammik/RevitLookup/issues/264:
    - Radius property support
- **StructuralSettings** class support https://github.com/jeremytammik/RevitLookup/pull/282 by @SergeyNefyodov:
    - GetStructuralSettings method support
- **StructuralSettings** class support https://github.com/jeremytammik/RevitLookup/pull/283 by @SergeyNefyodov:
    - GetActiveSunAndShadowSettings method support
    - GetSunrise method support
    - GetSunset method support
    - GetSunset method support
    - IsTimeIntervalValid method support
    - IsAfterStartDateAndTime method support
    - IsBeforeEndDateAndTime method support
- **RevisionNumberingSequence** class support https://github.com/jeremytammik/RevitLookup/pull/289 by @SergeyNefyodov:
    - GetAllRevisionNumberingSequences method support
- **AnalyticalLinkType** class support https://github.com/jeremytammik/RevitLookup/pull/288 by @SergeyNefyodov:
    - IsValidAnalyticalFixityState method support
- **AreaVolumeSettings** class support https://github.com/jeremytammik/RevitLookup/pull/287 by @SergeyNefyodov:
    - GetAreaVolumeSettings method support
    - GetSpatialElementBoundaryLocation method support

**New default settings:**

- `Show Static` members enabled by default
- `Show Events` enabled by default
- `Show Extensions` enabled by default

## Bugs

- Fixed missing quick access icon https://github.com/jeremytammik/RevitLookup/issues/267
- Fixed DataGrid accent color https://github.com/jeremytammik/RevitLookup/issues/273

## Mics

- Updated **Contributing** guide.
- Added a new GitHub **issue templates**.

# 2025-06-23 **2025.0.8**

## Addins dependencies isolation. End of DLL hell

In this release, RevitLookup now runs in an isolated container for addin dependencies.
This new capability prevents conflicts and compatibility issues arising from different library versions between plugins, ensuring a more stable and reliable environment for plugin execution.

This enhancement uses the `Nice3point.Revit.Toolkit` to manage the isolation process, effectively eliminating DLL conflicts.
By integrating this package, RevitLookup ensures a consistent and predictable user experience.

Detailed description how it works: https://github.com/Nice3point/RevitToolkit/releases/tag/2025.0.1

Dependency isolation is available starting with Revit 2025.
Note that the isolation mechanism is implemented by an additional library that must be loaded into Revit at first startup for it to work.
Therefore, if your other plugins use `Nice3point.Revit.Toolkit`, it must be updated to version `2025.0.1`, which introduces this feature

## Improvements

- Added new extensions in https://github.com/jeremytammik/RevitLookup/pull/255, https://github.com/jeremytammik/RevitLookup/pull/257:

| Type      | Extension                          | Description                                                                                                                                    |
|:----------|------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------|
| Part      | IsMergedPart                       | Is the Part the result of a merge.                                                                                                             |
| Part      | IsPartDerivedFromLink              | Is the Part derived from link geometry                                                                                                         |
| Part      | GetChainLengthToOriginal           | Calculates the length of the longest chain of divisions/ merges to reach to an original non-Part element that is the source of the tested part |
| Part      | GetMergedParts                     | Retrieves the element ids of the source elements of a merged part                                                                              |
| Part      | ArePartsValidForDivide             | Identifies if provided members are valid for dividing parts                                                                                    |
| Part      | FindMergeableClusters              | Segregates a set of elements into subsets which are valid for merge                                                                            |
| Part      | ArePartsValidForMerge              | Identifies whether Part elements may be merged                                                                                                 |
| Part      | GetAssociatedPartMaker             | Gets associated PartMaker for an element                                                                                                       |
| Part      | GetSplittingCurves                 | Identifies the curves that were used to create the part                                                                                        |
| Part      | GetSplittingElements               | Identifies the elements ( reference planes, levels, grids ) that were used to create the part                                                  |
| Part      | HasAssociatedParts                 | Checks if an element has associated parts                                                                                                      |
| PartMaker | GetPartMakerMethodToDivideVolumeFW | Obtains the object allowing access to the divided volume properties of the PartMaker                                                           |
| Element   | GetCheckoutStatus                  | Gets the ownership status of an element                                                                                                        |
| Element   | GetWorksharingTooltipInfo          | Gets worksharing information about an element to display in an in-canvas tooltip                                                               |
| Element   | GetModelUpdatesStatus              | Gets the status of a single element in the central model                                                                                       |
| Element   | AreElementsValidForCreateParts     | Identifies if the given elements can be used to create parts                                                                                   |

## Solved issues

- Dependencies conflict https://github.com/jeremytammik/RevitLookup/issues/210, https://github.com/jeremytammik/RevitLookup/issues/252
- Request for adding WorksharingTooltipInfo Properties https://github.com/jeremytammik/RevitLookup/issues/254
- AssemblyLoadContext discussion https://github.com/jeremytammik/RevitLookup/issues/246

# 2025-06-11 **2025.0.7**

## General

- **Solid scaling**

  Visualisation now supports scaling a solid, relative to its centre. Exploring small objects is now even easier https://github.com/jeremytammik/RevitLookup/issues/251

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/dbe36b89-3065-469a-8768-fbd151bbe442)

- **Theme synchronisation with Revit**

  Starting with Revit version 2024, you can choose to automatically change the RevitLookup theme. Fans of darker colours will no longer have to dig through the settings every time

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/40844c42-ff8b-4bc6-949b-5a1b3660adf2)

## Improvements

- Improved arrow position for vertical edges on visualization
- Multithreading visualization support. Changing settings now does not affect rendering. Previously there were artifacts due to fast settings changes
- Added new extensions:

  | Type           | Extension                                 | Description                                                                          |
  |:---------------|-------------------------------------------|--------------------------------------------------------------------------------------|
  | Element        | GetCuttingSolids                          | Gets all the solids which cut the input element                                      |
  | Element        | GetSolidsBeingCut                         | Get all the solids which are cut by the input element                                |
  | Element        | IsAllowedForSolidCut                      | Validates that the element is eligible for a solid-solid cut                         |
  | Element        | IsElementFromAppropriateContext           | Validates that the element is from an appropriate document                           |

# 2025-06-06 **2025.0.6**

- Visualization dark theme support https://github.com/jeremytammik/RevitLookup/issues/250

# 2025-06-06 **2025.0.5**

## RevitLookup ✨1000 Stars on GitHub

We're proud to share that RevitLookup has achieved 1000 stars on GitHub!
This milestone is a testament to its value and the dedication of our community.
Thank you for helping us reach this landmark!

<a href="https://star-history.com/#jeremytammik/RevitLookup&Date">
    <picture>
        <source media="(prefers-color-scheme: dark)" srcset="https://api.star-history.com/svg?repos=jeremytammik/RevitLookup&type=Date&theme=dark" />
        <source media="(prefers-color-scheme: light)" srcset="https://api.star-history.com/svg?repos=jeremytammik/RevitLookup&type=Date" />
        <img alt="Star History Chart" src="https://api.star-history.com/svg?repos=jeremytammik/RevitLookup&type=Date" />
    </picture>
</a>

To celebrate it, we are excited to introduce a major new feature in this release that will transform your interaction with models, offering a deeper understanding of the geometric
objects that constitute your models.

## Introducing Geometry Visualization

This release includes comprehensive Geometry Visualization capabilities, enabling users to visualize various geometry objects directly within the RevitLookup interface.

In Revit, geometry is at the core of every model.
Whether you are dealing with simple shapes or intricate structures, having the ability to visualize geometric elements can significantly improve your workflow, analysis, and
understanding of the BIM.

To illustrate the power of these visualization capabilities, here's a glimpse of the geometric objects you can now explore directly within RevitLookup:

| Geometry    | Illustration                                                                                               |
|-------------|------------------------------------------------------------------------------------------------------------|
| Mesh        | ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/84cd42fe-5248-4c13-8f30-0869396ad3b8) |
| Face        | ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/15ba15da-325e-499f-935e-fa5cc9b71390) |
| Solid       | ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/feacdb95-e5ea-43d0-9aef-b6a1f5116b37) |
| Curve       | ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/d08b0bf3-0622-4f49-b999-4365a0955129) |
| Edge        | ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/30291e03-8eb8-4de2-a54f-0c288ee4dcb2) |
| BoundingBox | ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/f800a552-86df-4554-8d5b-c53561720f0d) |
| XYZ         | ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/72b3f7cb-279c-4465-9cff-7918e0aaf37c) |

For detailed documentation, visit: https://github.com/jeremytammik/RevitLookup/wiki/visualization

Feel free to leave comments and suggestions regarding visualization here: https://github.com/jeremytammik/RevitLookup/pull/245.
Your input helps us improve this tool for everyone in the Revit community.

## Improvements

- **BoundingBoxXYZ** class support
    - Added `Bounds` method support
    - Added `MinEnabled` method support
    - Added `MaxEnabled` method support
    - Added `BoundEnabled` method support
- Added **Edit parameter** icon
- Added **Select** context menu action for Reference type
- Added **Export family size table** for FamilySizeTableManager type by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/244
- Added new extensions:

| Type           | Extension                                 | Description                                                        |
|:---------------|-------------------------------------------|--------------------------------------------------------------------|
| Application    | GetFormulaFunctions                       | Gets list of function names supported by formula engine            |
| Application    | GetFormulaOperators                       | Gets list of operator names supported by formula engine            |
| BoundingBoxXYZ | Centroid                                  | Gets the bounding box center point                                 |
| BoundingBoxXYZ | Vertices                                  | Gets list of bounding box vertices                                 |
| BoundingBoxXYZ | Volume                                    | Evaluate bounding box volume                                       |
| BoundingBoxXYZ | SurfaceArea                               | Evaluate bounding box surface area                                 |
| Document       | GetAllGlobalParameters                    | Returns all global parameters available in the given document      |
| Document       | GetLightGroupManager                      | Gets a light group manager object from the given document          |
| Document       | GetTemporaryGraphicsManager               | Gets a TemporaryGraphicsManager reference of the document          |
| Document       | GetAnalyticalToPhysicalAssociationManager | Gets a AnalyticalToPhysicalAssociationManager for this document    |
| Document       | GetFamilySizeTableManager                 | Gets a FamilySizeTableManager from a Family                        |
| UIApplication  | CurrentTheme                              | Gets a current theme                                               |
| UIApplication  | CurrentCanvasTheme                        | Gets a current canvas theme                                        |
| UIApplication  | FollowSystemColorTheme                    | Indicate if the overall theme follows operating system color theme |
| View           | GetSpatialFieldManager                    | Retrieves manager object for the given view                        |

Hope everyone enjoys the new release. Thanks!

Made with love by @Nice3point 🕊️

# 2025-05-17 **2025.0.4**

A new Release focused on improving core functionalities and robustness of the product.

## Improvements

- Introducing a preview feature for **Family Size Table**, making it easier to manage and visualize family sizes by @SergeyNefyodov
  in https://github.com/jeremytammik/RevitLookup/pull/236
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/57001fab-cc5b-4973-a42a-70ffe3420b69)
  To access it:
    - Enable **Show Extensions** in the view menu
    - Select any **FamilyInstance**
    - Navigate to the **Symbol**
    - Navigate to the **Family** (or just search for Family class objects in the **Snoop database** command)
    - Navigate to the **GetFamilySizeTableManager** method
    - Navigate to the **GetSizeTable** method
    - Right-click on one of the tables and select the **Show table** command

  _Note: Family size table is currently in read-only mode_

- Added new context menu item for selecting Element classes without showing
- Added new fresh, intuitive icons to the context menu for a more user-friendly interface.
- Refined labels, class names, and exception messages

## Bugs

- Resolved an issue where the delete action was not displayed in the context menu for ElementType classes
- Fixed the context menu display issue for Element classes, broken in previous release
- Fixed the order of descriptors to prevent missing extensions and context menu items in some classes, broken in previous release by @SergeyNefyodov
  in https://github.com/jeremytammik/RevitLookup/pull/235

# 2025-05-13 **2025.0.3**

## General

- **Memory diagnoser**
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/dfa0fc23-5a63-452d-8a73-25009424c99c)

  Memory column contains the size of allocated **managed memory**.
  Native ETW and allocations in C++ code are not included to avoid severe performance degradation.

## Improvements

- The different method overloading variations, are now displayed in the `Variants` collection
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/22d8c84b-097c-4da3-9dfa-f091a6de9b7f)
  Previous: **GeometryElement**

  Now: **Variants\<GeometryElement\>**

- ConnectorManager class support
    - Added `ConnectorManager.Lookup` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/227
- Wire class support
    - Added `Wire.GetVertex` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/228
- IndependentTag class support
    - Added `IndependentTag.CanLeaderEndConditionBeAssigned` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/229
    - Added `IndependentTag.GetLeaderElbow` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/229
    - Added `IndependentTag.GetLeaderEnd` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/229
    - Added `IndependentTag.HasLeaderElbow` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/229
    - Added `IndependentTag.IsLeaderVisible` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/229
- CurveElement class support
    - Added `CurveElement.GetAdjoinedCurveElements` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/230
    - Added `CurveElement.HasTangentLocks` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/230
    - Added `CurveElement.GetTangentLock` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/230
    - Added `CurveElement.HasTangentJoin` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/230
    - Added `CurveElement.IsAdjoinedCurveElement` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/230
- TableView class support
    - Added `TableView.GetAvailableParameters` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/231
    - Added `TableView.GetCalculatedValueName` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/231
    - Added `TableView.GetCalculatedValueText` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/231
    - Added `TableView.IsValidSectionType` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/231
    - Added `TableView.GetCellText` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/231
- DatumPlane class support
    - Added `DatumPlane.CanBeVisibleInView` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/232
    - Added `DatumPlane.GetPropagationViews` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/232
    - Added `DatumPlane.CanBeVisibleInView` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/232
    - Added `DatumPlane.GetPropagationViews` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/232
    - Added `DatumPlane.GetDatumExtentTypeInView` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/232
    - Added `DatumPlane.HasBubbleInView` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/232
    - Added `DatumPlane.IsBubbleVisibleInView` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/232
    - Added `DatumPlane.GetCurvesInView` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/232
    - Added `DatumPlane.GetLeader` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/232
- Extensions:
    - Added Family class extension `FamilySizeTableManager.GetFamilySizeTableManager` by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/233
    - Added FamilyInstance class extension `AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds`
    - Added FamilyInstance class extension `AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance`
    - Added Solid class extension `SolidUtils.SplitVolumes`
    - Added Solid class extension `SolidUtils.IsValidForTessellation`

# 2025-05-01 **2025.0.2**

## General

- Output error and failure messages to the Revit journal.

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/732db483-b4fd-4a32-bc3e-3e88548c0f3d)

  Messages include detailed crash information, including StackTrace.
  It works for Revit in general and outputs all Domain fatal errors, including failures caused by third-party plugins.

  Journals path: `%LocalAppData%\Autodesk\Revit`

- Displaying the original exception type in tooltips, instead of TargetInvocationException.

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/af613323-0aeb-4e4a-aac2-b1523380d2f9)

## Improvements

- View class support
    - Added `View.GetCategoryHidden` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/217
    - Added `View.GetCategoryOverrides` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/217
    - Added `View.GetIsFilterEnabled` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/217
    - Added `View.GetFilterOverrides` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/217
    - Added `View.GetFilterVisibility` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/217
    - Added `View.GetWorksetVisibility` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/217
    - Added `View.GetColorFillSchemeId` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/217
    - Added `View.IsCategoryOverridable` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/218
    - Added `View.IsFilterApplied` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/218
    - Added `View.IsInTemporaryViewMode` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/218
    - Added `View.IsValidViewTemplate` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/218
    - Added `View.IsWorksetVisible` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/218
    - Added `View.CanCategoryBeHidden` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/219
    - Added `View.CanCategoryBeHiddenTemporary` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/219
    - Added `View.CanViewBeDuplicated` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/219
    - Added `View.SupportsWorksharingDisplayMode` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/219
- ScheduleDefinition class support
    - Added `ScheduleDefinition.CanFilterByGlobalParameters` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
    - Added `ScheduleDefinition.CanFilterByParameterExistence` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
    - Added `ScheduleDefinition.CanFilterBySubstring` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
    - Added `ScheduleDefinition.CanFilterByValue` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
    - Added `ScheduleDefinition.CanFilterByValuePresence` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
    - Added `ScheduleDefinition.CanSortByField` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
    - Added `ScheduleDefinition.GetField` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
    - Added `ScheduleDefinition.GetFieldId` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
    - Added `ScheduleDefinition.GetFieldIndex` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
    - Added `ScheduleDefinition.GetFilter` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
    - Added `ScheduleDefinition.GetSortGroupField` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
    - Added `ScheduleDefinition.IsValidCategoryForEmbeddedSchedule` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/220
- ViewSchedule class support
    - Added `ViewSchedule.GetStripedRowsColor` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.IsValidTextTypeId` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetDefaultNameForKeySchedule` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetDefaultNameForMaterialTakeoff` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetDefaultNameForSchedule` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetDefaultParameterNameForKeySchedule` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.IsValidCategoryForKeySchedule` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.IsValidCategoryForMaterialTakeoff` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.IsValidCategoryForSchedule` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetDefaultNameForKeynoteLegend` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetDefaultNameForNoteBlock` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetDefaultNameForRevisionSchedule` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetDefaultNameForSheetList` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetDefaultNameForViewList` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetValidFamiliesForNoteBlock` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetScheduleInstances` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Added `ViewSchedule.GetSegmentHeight` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/221
    - Disabled `ViewSchedule.RefreshData`
- TableData class support
    - Added `TableData.GetSectionData` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/223
    - Added `TableData.IsValidZoomLevel` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/223
- TableSectionData class support
    - Added `TableSectionData.AllowOverrideCellStyle` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.CanInsertColumn` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.CanInsertRow` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.CanRemoveColumn` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.CanRemoveRow` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellCalculatedValue` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellCalculatedValue` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellCategoryId` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellCategoryId` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellCombinedParameters` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellCombinedParameters` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellFormatOptions` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellFormatOptions` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellParamId` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellParamId` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellSpec` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellText` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellType` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetCellType` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetColumnWidth` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetColumnWidthInPixels` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetMergedCell` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetRowHeight` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetRowHeightInPixels` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.GetTableCellStyle` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.IsCellFormattable` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.IsCellOverridden` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.IsCellOverridden` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.IsValidColumnNumber` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.IsValidRowNumber` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224
    - Added `TableSectionData.RefreshData` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/224

## Bugs

- Temporary disabled `IndependentTag.TagText` for RebarBendingDetail in Revit 2025. Revit API issue https://github.com/jeremytammik/RevitLookup/issues/225

# 2025-04-02 **2025.0.1**

## HotFix

- Fixed Search Bar causing Revit crashing https://github.com/jeremytammik/RevitLookup/issues/214

## Improvements

- Ref parameter type support
- Add `BasePoint.GetSurveyPoint` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/212
- Add `BasePoint.GetProjectBasePoint` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/212
- Add `InternalOrigin.Get` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/212
- Add `ElevationMarker.GetViewId` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/213
- Add `CurtainGrid.GetCell` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/215
- Add `CurtainGrid.GetPanel` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/215
- Add `Panel.GetRefGridLines` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/215

Many thanks to @SergeyNefyodov for contributing to RevitLookup

# 2025-04-02 **2025.0.0**

## General

- Revit 2025 support

- **Action for deleting element**

  Now you can delete an item from the project, the action is available both from the left panel and from the table.
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/e8b4d939-db4f-4a14-847f-5852cb4ebec9)

- **Action for editing element parameter value**

  Now you can edit the parameter value. String, Double, Int, ElementId supported.
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/e6b9dbe2-f5db-4a93-ae58-ccb82cdb3d88)

- **ForgeTypeId class name**

  For developer convenience, the Forge Schema dialog now displays the full class and property name, for direct use in code.
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/780f27dd-2aa2-4cfb-b7c2-ef1970aaaf07)

## Improvements

- Added Symbols, groups to the Forge Schema dialog.
- Add `RevitLinkType.IsLoaded` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/208
- Add .LocationCurve.ElementsAtJoin` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/205
- Add .LocationCurve.JoinType` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/205

# 2024-02-10 **2024.0.13**

## General

- **Modules view**

  The new Modules view lets you inspect the dynamic link libraries (DLLs) and executables that your app uses. In this view, you'll find information such as module names, versions,
  application domains, paths to the module.
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/b9f23a6c-24c8-4ff5-a4d1-59a3a685ac4d)

## Bugs

- Fix clipboard exception causing Revit crash https://github.com/jeremytammik/RevitLookup/issues/202
- Fix opening Search Elements dialog from Revit ribbon

# 2023-12-25 **2024.0.12**

Last corrective update for this year, bringing minor tweaks and improvements

- Add theme update for all open RevitLookup instances by @ricaun in https://github.com/jeremytammik/RevitLookup/pull/200
- Fix incorrect Hue calculation for some colour formats
- Disable all background effects for Windows 10. Thanks @ricaun for help and testing https://github.com/jeremytammik/RevitLookup/issues/194

That's all for now. I wish you all a Happy New Year with best regards, do what you love, evolve, travel, don't forget to have a rest and keep coding 🎉

# 2023-12-25 **2024.0.11**

In this release RevitLookup welcomes you with improved visuals, support for templates to fine-tune data display, improved navigation, in-depth color support, let's take a look

## General

- **Navigation**. Updated navigation allows `Ctrl + Click` in the tree or grid to open any selected item or group of items in a new tab.
  This also allows you to analyze items that RevitLookup doesn't support, how about looking at StackTrace for exceptions
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/0c13c6da-772f-453b-9d34-bff609c04d95)

- **Color Preview**. Changes to the user interface give us the ability to customize the display of any type of data.
  And now you will be able to visually see how materials or ribbon looks like.
  `Autodesk.Revit.DB.Color` and `System.Windows.Media.Color` are supported
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/3b736961-26fa-4a24-b916-bb7c4fddfda9)

## Improvements

- **Update available notification**. Updates are now checked automatically and an icon is now displayed in the navigation area if a new version is available

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/b7ab5fd0-b927-4b9a-805c-91e45fbd9f14)

- **Background effects** Available on windows 11 only.

  Acrylic:

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/259012f7-f19d-4779-8b17-4be96a404023)

  Blur:

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/e8046bf0-ae48-446e-94e3-e3fdc59898e4)

  The visual representation of the background depends on your desktop image and current theme

- **Color extensions** Convert color to other formats HEX, CMYK, etc. Color name identification, `en` and `ru` localizations available.
  `Autodesk.Revit.DB.Color` and `System.Windows.Media.Color` are supported
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/668a9c5c-3239-4100-8829-63fc71c880fb)

## Bugs

- Fixed incorrect display when switching themes on windows 10 https://github.com/jeremytammik/RevitLookup/issues/194
- Returned deleted notification when checking for updates

## Misc

- Updated developer's [guide](https://github.com/jeremytammik/RevitLookup/blob/dev/CONTRIBUTING.md#styles).

Here, I'm wrapping things up. Wishing everyone a splendid New Year and a joyous Christmas ahead. As always, yours truly @Nice3point 🎅

# 2023-12-01 **2024.0.10**

## General

* Introducing a brand new feature: Restore window size! Now, effortlessly revert back to your preferred window dimensions with a simple click

## Improvements

* Add `MEPSystem.GetSectionByIndex` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/189
* Add `MEPSystem.GetSectionByNumber` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/189
* Add `MEPSection.GetElementIds` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/192
* Add `MEPSection.GetCoefficient` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/192
* Add `MEPSection.GetPressureDrop` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/192
* Add `MEPSection.GetSegmentLength` support by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/192
* Add `MEPSection.IsMain` support
* Add show `System.Object` option (named Root hierarchy) by @SergeyNefyodov in https://github.com/jeremytammik/RevitLookup/pull/193
* Add generic types support for the help button
* Minor tooltip changes

## Bugs

* Fixed search that worked in the main thread

# 2023-11-19 **2024.0.9**

## User interface

* **Settings Accessibility:** Most settings have been relocated to the grid context menu for a more intuitive and accessible user experience. Now, users can conveniently access and
  modify settings directly from the grid context menu

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/77fc172a-48d3-4439-a5d6-9d8d80ee0efc)

## Improvements

* Added ExtensibleStorage `Array` and `Map` support https://github.com/jeremytammik/RevitLookup/issues/184
* Added `Room.GetBoundarySegments()` support by [@SergeyNefyodov](https://github.com/SergeyNefyodov) in https://github.com/jeremytammik/RevitLookup/pull/187
* Added `BoundarySegment` support by [@SergeyNefyodov](https://github.com/SergeyNefyodov) in https://github.com/jeremytammik/RevitLookup/pull/187
* Added changing theme in runtime
* Optimized rendering performance for both tree and grid views, resulting in smoother and faster visual representation
* Added shortcuts for close current and all RevitLookup instances https://github.com/jeremytammik/RevitLookup/issues/172

    * `Esc` now closes the current window.

    * `Shift + Esc` closes all RevitLookup instances

    * Explore all available shortcuts [here](https://github.com/jeremytammik/RevitLookup/wiki/Context-actions)

## Bugs

* Fixed some crashes https://github.com/jeremytammik/RevitLookup/issues/180

## Breaking changes

* Disabled the last selection restoration during search to improve performance, especially on larger lists.
  This change optimizes search functionality by preventing potential slowdowns previously experienced with extensive lists

## Miscellaneous Updates

* Upgraded the UI library to the latest version, ensuring compatibility and incorporating potential improvements in functionality and design
* Project updated to .Net 8 and C# 12

## New Contributors

* [@SergeyNefyodov](https://github.com/SergeyNefyodov)  made their first contribution in https://github.com/jeremytammik/RevitLookup/pull/187

Full changelog: https://github.com/jeremytammik/RevitLookup/compare/2024.0.8...2024.0.9

# 2023-06-22 **2024.0.8**

## Features

### Core

* Computing Time Tracking

  This feature includes the ability to monitor the computing time taken to invoke a member, such as methods or properties.
  By tracking the execution time, you can identify and analyze slow-performing methods or properties, gaining insights into their overall performance.
  The computing time is displayed in a separate column and a tooltip, providing you with detailed information. This feature is optional and disabled by default

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/9f9c816f-2f49-49a0-9757-6f0bb0000113)

### User interface

* Context Menu

  A convenient context menu has been added to the table, providing you with additional options to manage columns and update contents.
  This menu enables you to customize your table view and effortlessly perform actions to enhance your experience.

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/25491ace-1d16-46cc-9dc3-3e5016b266a0)

* Enhanced Visualization

  Icons have been added to the context menu, making it more visually appealing and intuitive for users to navigate and interact with the available options.

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/afe44cbf-6e5b-4508-beda-b5a4e7babcf2)

## Improvements

* Added async support for unit dialogs
* Added API for external programs https://github.com/jeremytammik/RevitLookup/issues/171
* Added FamilyParameter support by @CADBIMDeveloper in https://github.com/jeremytammik/RevitLookup/pull/174
* Added FamilyManager.GetAssociatedFamilyParameter extension by @CADBIMDeveloper in https://github.com/jeremytammik/RevitLookup/pull/175

## Bugs

* Fixed shortcuts reloading leading to incorrect ribbon update https://github.com/jeremytammik/RevitLookup/issues/177

Full changelog: https://github.com/jeremytammik/RevitLookup/compare/2024.0.7...2024.0.8

# 2023-06-03 **2024.0.7**

Corrective update to the last major release [RevitLookup 2024.0.6](https://github.com/jeremytammik/RevitLookup/releases/tag/2024.0.6)

## Hotfix

* Fixed cases where the return value type was displayed instead of the value itself for methods that support overloads

## Improvements

* Added the `BuiltInCategory` extension of the `Category` class for Revit versions 2021-2022 where this property is not present in the official
  API https://github.com/jeremytammik/RevitLookup/issues/162

# 2023-06-01 **2024.0.6**

## Features

### User interface

* Icons

  Introducing a collection of new icons for properties, methods, fields, and events, ensuring a visually appealing representation
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/ffbba475-e240-4928-bf02-68d8f75cbc4c)

* Enhanced Performance with Separate UI Thread

  The RevitLookup user interface now operates in a dedicated thread, independent of Revit's workload. This architectural improvement significantly enhances smoothness and
  responsiveness

* New Additional Setting Options

  Introducing a range of new setting options that expand customization capabilities and provide users with greater control over the tool's behavior

### Core

* Class fields

  Introducing support for displaying class fields, enabling a comprehensive understanding of the class structure
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/a4304fd4-4537-4bd2-8d90-88f46137a55a)

* Class events

  Introducing support for displaying class events, facilitating better comprehension of event-driven programming within the class
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/3b7ae347-e7bc-4642-89a0-99cd089f0abe)

* Class private members

  Empowering developers with the ability to visualize and access class private fields, properties, methods, and events, providing a complete overview of the class implementation
  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/4c6e4459-cf2f-4d35-9b03-fe0b259b3c9a)

## Improvements

* ElementId Redirection to Category

  Implemented a helpful feature that automatically redirects ElementId to Category, whenever applicable. This simplifies navigation and enhances the user experience
* Content Virtualization

  Applied content virtualization to the dashboard and settings page, optimizing performance by efficiently managing large amounts of data and dynamically loading content as needed.
  This
  results in a smoother and more efficient user interaction

# 2023-05-17 **2024.0.5**

## Features

* Static members support

  For example, RevitLookup now supports the display of these and other properties and methods:
    ```c#
    Category.GetCategory();
    Document.GetDocumentVersion()
    UIDocument.GetRevitUIFamilyLoadOptions()
    Application.MinimumThickness
    ```

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/1712cf55-2451-4ed8-8487-20c5ab973278)

* Ribbon update

  SplitButton replaced by PullDownButton. Thank for voting https://github.com/jeremytammik/RevitLookup/discussions/159

  ![image](https://github.com/jeremytammik/RevitLookup/assets/20504884/b6819f3b-5670-45ad-9353-1dabf9d3b512)

## Improvements

* Added DefinitionGroup support
* Added Element.GetMaterialArea support
* Added Element.GetMaterialVolume support
* Added FamilyInstance.get_Room support
* Added FamilyInstance.get_ToRoom support
* Added FamilyInstance.get_FromRoom support
* "Show element" is no longer available for ElementType

## Bugs

* Fixed issue when GetMaterialIds method didn't return nonPaint materials https://github.com/jeremytammik/RevitLookup/issues/163

# 2023-04-27 **2024.0.4**

Critical security patch

# 2023-04-24 **2024.0.3**

## Improvements

* Added Workset support
* Added WorksetTable support
* Added Document.GetUnusedElements support

## Bugs

* Fixed Dashboard window startup location

# 2023-04-18 **2024.0.2**

## Bugs

* Fixed Fatal Error on Windows 10 https://github.com/jeremytammik/RevitLookup/issues/153

  Accent colour sync with OS now only available in Windows 11 and above. Many thanks to [Aleksey Negus](https://t.me/a_negus) for testing builds

# 2023-04-14 **2024.0.1**

* Added option to enable hardware acceleration (experimental)

  The user interface is now more responsive. Revit uses software acceleration by default. Contact us if you encounter problems with your graphics cards

  Known issue: rendering performance drops on selection. This is especially evident on
  roofs. https://forums.autodesk.com/t5/revit-api-forum/revit-2024-rendering-performance-drops-on-selection/td-p/11878396
* Added button to enable RevitLookup panel on Modify tab by @ricaun in https://github.com/jeremytammik/RevitLookup/pull/152

  Disabled by default. Thanks vor voting https://github.com/jeremytammik/RevitLookup/discussions/151

* Opening RevitLookup window only when the Revit runtime context is available https://github.com/jeremytammik/RevitLookup/issues/155

## Improvements

* Added shortcuts support for the Modify tab https://github.com/jeremytammik/RevitLookup/issues/150
* Added EvaluatedParameter support
* Added Category.get_Visible support
* Added Category.get_AllowsVisibilityControl support
* Added Category.GetLineWeight support
* Added Category.GetLinePatternId support
* Added Category.GetElements extension
* Added Reference.ConvertToStableRepresentation support

## Bugs

* Fixed rare crashes in EventMonitor on large models
* Fixed Curve.Evaluate resolver using EndParameter as values

## Other

* Added installers for previous RevitLookup versions https://github.com/jeremytammik/RevitLookup/wiki/versions

# 2023-04-04 **2024.0.0**

<div align="center">
<img alt="RevitLookup" width="600" src="https://user-images.githubusercontent.com/20504884/218192495-19b13547-ce67-40e3-8fe8-e847f89bddb7.png"/>
</div>

In this release, the entire code base has been completely rewritten from scratch with a redesigned user interface. New tools, OTA update, Windows 11 support

# UI

* A brand-new user interface

  ![image](https://user-images.githubusercontent.com/20504884/225871636-21c17658-d02e-411c-93cd-34e4d2121933.png)

* Themes

  ![image](https://user-images.githubusercontent.com/20504884/225851738-9db4dde1-fa05-4115-bd45-5e3af870ad36.png)

* Extended context menu

  ![image](https://user-images.githubusercontent.com/20504884/225880995-e6a20465-26c0-494d-8f35-3abaacdb9525.png)

  Wiki page: https://github.com/jeremytammik/RevitLookup/wiki/Context-actions

* Tooltips

  ![image](https://user-images.githubusercontent.com/20504884/225851987-7cc5ad2f-1a3b-4c4c-8744-6f3372e0f2ff.png)

* The Snoop Selection button has been moved to the Modify tab

  ![image](https://user-images.githubusercontent.com/93661926/225279009-e440f9cd-b59f-4198-b058-e081cc4204c4.png)

* Smooth navigation. Enable acceleration in Revit settings if you are having trouble with this option

  ![image](https://user-images.githubusercontent.com/20504884/225870803-785b5082-edd2-44cf-a384-8633052740d7.png)

* Windows 11 Mica effect support
* Windows 11 Snap Layouts support

  ![image](https://user-images.githubusercontent.com/93661926/225279198-c6985018-b1fc-435e-9fb4-f0c97f99ff8c.png)

* Accent colour synced with OS

  ![image](https://user-images.githubusercontent.com/20504884/225880312-4e9ee066-97ba-4e72-b89f-966269b385ec.png)

* New logo
* Searchbar. Focus is triggered by pressing any key on the keyboard

# Engine

* A brand-new core
* Extensions. Support new methods from the API and other libraries

  ![image](https://user-images.githubusercontent.com/20504884/225852056-9bb523c6-85dd-44d8-b900-ef3ca1eefaa6.png)

  Available extensions: https://github.com/jeremytammik/RevitLookup/wiki/extensions

* Displaying all methods that objects have, even if RevitLookup does not support them

  ![image](https://user-images.githubusercontent.com/20504884/225852714-3255ece8-1c6c-464e-8949-5693b7ed7223.png)

  ![image](https://user-images.githubusercontent.com/20504884/225853186-ef6ce65e-6ee3-4a4d-a213-49fb8dfc7e75.png)

* Generic names support

  | Before                                                                                                          | Now                                                                                                             |
                              |-----------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------|
  | ![image](https://user-images.githubusercontent.com/20504884/225105646-37f2b052-f3fc-4771-967b-0578a94f9b07.png) | ![image](https://user-images.githubusercontent.com/20504884/225852403-4023c704-1932-471e-9f9f-84f8433013d7.png) |

* Multiple results for methods with overloads

  ![image](https://user-images.githubusercontent.com/20504884/225853785-8b5beacd-c8f1-401e-a51e-3e45f6aa8cba.png)

* Extensible storage moved to the `GetEntity()` method
* Adding new features and extending the functionality of RevitLookup just got easier. Developer's
  guide: https://github.com/jeremytammik/RevitLookup/blob/dev/CONTRIBUTING.md#architecture

# New features

* Snoop Point
* Snoop Sub-Element
* Snoop UI Application
* Component manager. Explore AdWindows.dll and learn how the ribbon and user interface in Revit are arranged

  ![image](https://user-images.githubusercontent.com/20504884/225854089-04c8448f-34f9-419c-b859-c51b2a2375b6.png)

* PerformanceAdviser. Explore document performance issues
* Registry research: schemas, services, updaters
* Explore BuiltIn and Forge units

  ![image](https://user-images.githubusercontent.com/20504884/225869710-3c651c4a-0b35-4dd4-8180-27370f657cd8.png)

* Event monitor. Track all incoming events. Events from the RevitAPI.dll and RevitAPIUI.dll libraries are available. The search bar is used to filter results

  ![image](https://user-images.githubusercontent.com/20504884/225856874-46c14b80-5c7d-444c-999a-10e8e4809ad2.png)

* Reworked search. Now you can search for multiple values by `Name`, `Id`, `UniqueId`, `IfcGUID` and `Type IfcGUID` parameters

  ![image](https://user-images.githubusercontent.com/20504884/225869706-7d5e2e4a-1f03-416e-9ad6-a96184b07836.png)

  Wiki page: https://github.com/jeremytammik/RevitLookup/wiki/Search-elements

* Visual search in a project.

  Showing elements:

  ![image](https://user-images.githubusercontent.com/20504884/225874545-aa0f7829-5215-412d-8c50-31ede8705ca8.png)

  Showing faces (Revit 2023 or higher):

  ![image](https://user-images.githubusercontent.com/20504884/225866963-d9a3c2e4-1569-40c4-9072-1736906dce6b.png)

  Showing solids (Revit 2023 or higher):

  ![image](https://user-images.githubusercontent.com/20504884/225867976-ccb417a0-85c6-4996-bcd8-fdff8a504152.png)

  Showing edges (Revit 2023 or higher):

  ![image](https://user-images.githubusercontent.com/20504884/225867460-282751ad-0782-4cb7-bb96-7465be556c6f.png)

* OTA update. The RevitLookup update is now available directly from the plugin

  ![image](https://user-images.githubusercontent.com/20504884/225875561-bda637d0-d170-411d-83ac-97b17342904a.png)

Designed & Developed by [Nice3point](https://github.com/Nice3point) 🕊

# 2022-06-17 **2023.1.0**

In this update:

- New: Hello World window changed to About
- New: resorted commands on the Revit ribbon, frequently used moved to the top
- New: added Snoop Active Document command
- Fix: revert support search index from keyboard
- Fix: removed label null if ElementID was -1

# 2022-04-05 **2023.0.0**

Revit 2023 support

# 2022-03-02 **2022.0.4.1**

Minor UI changes https://github.com/jeremytammik/RevitLookup/pull/135

# 2022-03-02 **2022.0.4.0**

Minimize, maximize support https://github.com/jeremytammik/RevitLookup/pull/134.
Fixed problem with sending a print job https://github.com/jeremytammik/RevitLookup/pull/133

# 2022-01-15 **2022.0.3.3**

BindingMap support. https://github.com/jeremytammik/RevitLookup/issues/128 issue.

# 2022-01-03 **2022.0.3.2**

This update fixes the display of labels:

- Support for string empty label.
- Support for string null label.
- Support for double? null label.
- Renamed "View = null" to "Undefined View". This was a misnomer because null throws an exception
- Renamed "View = null - Including geometry objects not set as Visible" to "Undefined View, including non-visible objects"
- Renamed "View = Document.ActiveView" to "Active View"
- Renamed "View = Document.ActiveView - Including geometry objects not set as Visible" to "Active View, including non-visible objects"

# 2021-12-21 **2022.0.3.1**

Fixed https://github.com/jeremytammik/RevitLookup/issues/117 issue. Reflection TargetException message replaced by InnerException message.

# 2021-12-04 **2022.0.3.0**

This is patch release to fix a few items. This release will fix the issues below:

- Removed unused code and resources, reduced application size
- Optimized collections and arrays, updating the window just got a little faster
- Fixed broken print and preview button
- Reduced memory allocation

# 2021-11-30 **2022.0.2.6**

mention RFA and RVT project in the project description in readme.md and repository summary

# 2021-11-13 **2022.0.2.5**

Added automatic generation of a release for the master branch

# 2021-10-29 **2022.0.2.0**

integrated pull request [#108](https://github.com/jeremytammik/RevitLookup/pull/108) by Roman @Nice3point
to include previous versions in the installer

# 2021-10-28 **2022.0.1.6**

integrated pull request [#107](https://github.com/jeremytammik/RevitLookup/pull/107) by Roman @Nice3point
to rename Build, fix hello world version, and move version number into csproj

# 2021-10-28 **2022.0.1.4**

integrated pull request [#105](https://github.com/jeremytammik/RevitLookup/pull/105) by Roman @Nice3point
to update badges, consolidate version number management, clean up builder, and remove gitlab CI

# 2021-10-28 **2022.0.1.3**

integrated pull request [#104](https://github.com/jeremytammik/RevitLookup/pull/104) by Roman @Nice3point
to fix snoop db exception due to tag and enum mismatch

# 2021-10-28 **2022.0.1.2**

integrated pull request [#102](https://github.com/jeremytammik/RevitLookup/pull/102) by Roman @Nice3point
to add changelog and remove unused files

# 2021-10-28 **2022.0.1.1**

integrated pull request [#101](https://github.com/jeremytammik/RevitLookup/pull/101) by Roman @Nice3point
implementing code rerstructuring, cleanup, build system and installer

# 2021-10-24 **2022.0.1.0**

integrated pull request [#99](https://github.com/jeremytammik/RevitLookup/pull/99) by @NeVeS
to Eliminate warnings from [#98](https://github.com/jeremytammik/RevitLookup/pull/98)

# 2021-10-18 **2022.0.1.0**

integrated pull request [#97](https://github.com/jeremytammik/RevitLookup/pull/97) by @NeVeS
to restore ability to snoop plan topologies

# 2021-10-17 **2022.0.1.0**

integrated pull request [#96](https://github.com/jeremytammik/RevitLookup/pull/96) by @NeVeS
to fix crash on user cancel picking object in cmds: SnoopPickFace, SnoopPickEdge, SnoopLinkedElement

# 2021-10-17 **2022.0.1.0**

integrated pull request [#95](https://github.com/jeremytammik/RevitLookup/pull/95) by @NeVeS
to handle multiple open documents at the same time

# 2021-10-16 **2022.0.1.0**

integrated pull request [#94](https://github.com/jeremytammik/RevitLookup/pull/94) by @NeVeS
fixing problem with tranferring focus to Revit when using selectors from modeless window

# 2021-10-16 **2022.0.1.0**

integrated pull request [#93](https://github.com/jeremytammik/RevitLookup/pull/93) by @NeVeS
imlementing Modeless windows

# 2021-09-22 **2022.0.0.16**

integrated pull request [#91](https://github.com/jeremytammik/RevitLookup/pull/91) by @mphelt
to add PartUtilsStream

# 2021-07-01 **2022.0.0.13**

integrated pull request [#86](https://github.com/jeremytammik/RevitLookup/pull/86) by Luiz Henrique Cassettari
adding OnLoad to update width of snoop window value ListView last column

# 2021-06-30 **2022.0.0.12**

integrated pull request [#85](https://github.com/jeremytammik/RevitLookup/pull/85) by Luiz Henrique Cassettari
increasing width of snoop window value ListView column from 300 to 800

# 2021-06-07 **2022.0.0.11**

integrated pull request [#84](https://github.com/jeremytammik/RevitLookup/pull/84) by @RevitArkitek
adding PlanViewRange functionality to display view range level id and offset

# 2021-06-07 **2022.0.0.10**

integrated pull request [#83](https://github.com/jeremytammik/RevitLookup/pull/83) by @RevitArkitek
fixing error where element cannot be retrieved for an element id because SupportedColorFillCategoryIds returns category ids instead

# 2021-05-18 **2022.0.0.9**

integrated pull request [#81](https://github.com/jeremytammik/RevitLookup/pull/81) by @CADBIMDeveloper
enhancing `ElementId` and Revit 2022 extensible storage support

# 2021-05-14 **2022.0.0.8**

integrated pull request [#80](https://github.com/jeremytammik/RevitLookup/pull/80) by @WspDev
to remove deprecated `ParameterType` usage

# 2021-05-07 **2022.0.0.7**

integrated pull request [#78](https://github.com/jeremytammik/RevitLookup/pull/78) by @RevitArkitek
to handle `TableData.GetSectionData`

# 2021-05-07 **2022.0.0.6**

integrated pull request [#77](https://github.com/jeremytammik/RevitLookup/pull/77) by @RevitArkitek
to get end points for curves

# 2021-04-16 **2022.0.0.5**

integrated pull request [#76](https://github.com/jeremytammik/RevitLookup/pull/76) by @peterhirn
to fix CI for new VS 2019 Revit 2022 dotnet-core csproj

# 2021-04-15 **2022.0.0.4**

upgraded to Visual Studio 2019 (from 2017) and adopted @peterhirn project and solution files

# 2021-04-15 **2022.0.0.3**

reset Revit API assembly DLL references to Copy Local false

# 2021-04-15 **2022.0.0.3**

integrated pull request [#73](https://github.com/jeremytammik/RevitLookup/pull/73) by @mphelt
to wrap snoop in temporary transaction allowing to snoop PlanTopologies

# 2021-04-15 **2022.0.0.2**

integrated pull request [#75](https://github.com/jeremytammik/RevitLookup/pull/75) by @peterhirn
to fix CI for Revit 2022 and non-dotnet-core project file

# 2021-04-15 **2022.0.0.1**

integrated pull request [#74](https://github.com/jeremytammik/RevitLookup/pull/74) by @peterhirn
setting up CI to Revit 2022

# 2021-04-15 **2022.0.0.0**

flat migration to Revit 2022

# 2021-02-09 **2021.0.0.13**

integrated pull request https://github.com/jeremytammik/RevitLookup/pull/71 by @RevitArkitek
adding handler for ScheduleDefinition.GetField to address issue https://github.com/jeremytammik/RevitLookup/issues/70

# 2021-02-01 **2021.0.0.12**

integrated pull request [#69](https://github.com/jeremytammik/RevitLookup/pull/69) by @RevitArkitek
adding handler for the GetSplitRegionOffsets method to address issue https://github.com/jeremytammik/RevitLookup/issues/68 Split Region Offsets (2021)

# 2021-01-12 **2021.0.0.11**

integrated pull request [#67](https://github.com/jeremytammik/RevitLookup/pull/67) by @peterhirn
to update timestamp server from Verisign to digicert

# 2021-01-11 **2021.0.0.10**

increment copyright year

# 2020-12-06 **2021.0.0.9**

locally disable warning CS0618 `DisplayUnitType` is obsolete for one specific use case

# 2020-12-04 **2021.0.0.8**

integrated pull request [#66](https://github.com/jeremytammik/RevitLookup/pull/66) by @RevitArkitek
adding handlers for View GetTemplateParameterIds and GetNonControlledTemplateParameterIds

# 2020-11-09 **2021.0.0.7**

integrated pull request [#64](https://github.com/jeremytammik/RevitLookup/pull/64) by @peterhirn
to update CI for Revit 2021

# 2020-10-20 **2021.0.0.6**

eliminated deprecated unit api usage

# 2020-10-20 **2021.0.0.5**

integrated pull request [#63](https://github.com/jeremytammik/RevitLookup/pull/63) by @swfaust
to update command registration and remove obsolete test framework command

# 2020-04-14 **2021.0.0.1**

integrated pull request [#58](https://github.com/jeremytammik/RevitLookup/pull/58) by @harrymattison with
solution changes for multi-release building

# 2020-04-12 **2021.0.0.0**

flat migration to Revit 2021

# 2020-02-11 **2020.0.0.4**

incremented copyright year

# 2019-08-20 **2020.0.0.3**

integrated pull request [#56](https://github.com/jeremytammik/RevitLookup/pull/56) by @nonoesp

- fix two small typos in readme

# 2019-06-03 **2020.0.0.2**

integrated pull request [#53](https://github.com/jeremytammik/RevitLookup/pull/53) by @CADBIMDeveloper

- list available values for ParameterType.FamilyType and FamilyParameters titles

# 2019-04-26 **2020.0.0.1**

integrated pull request [#52](https://github.com/jeremytammik/RevitLookup/pull/52) by @CADBIMDeveloper

# 2019-04-18 **2020.0.0.0**

flat migration to Revit 2020

# 2019-04-18 **2019.0.0.13**

added MSI installer for Revit 2017-2020 by Harry Mattison

# 2019-03-27 **2019.0.0.12**

added MSI installer for 2018.0.0.0 submitted by @VBScab in issue [#51](https://github.com/jeremytammik/RevitLookup/issues/51)

# 2019-03-26 **2019.0.0.11**

integrated pull request [#50](https://github.com/jeremytammik/RevitLookup/pull/50) by Victor Chekalin
to handle DoubleArray4d values

# 2019-03-25 **2019.0.0.10**

integrated pull requests [#48](https://github.com/jeremytammik/RevitLookup/pull/48) and [#49](https://github.com/jeremytammik/RevitLookup/pull/49) by Victor Chekalin
to snoop rendering AssetProperty via Material-AppearanceAssetId-GetRenderingAssset

# 2019-03-18 **2019.0.0.9**

added CmdSnoopModScopeDependents

# 2019-03-18 **2019.0.0.8**

cleanup before adding CmdSnoopModScopeDependents

# 2019-01-21 **2019.0.0.7**

fixed typo in variable name reported by @yk35 in pull request [#47](https://github.com/jeremytammik/RevitLookup/pull/47)

# 2019-01-17 **2019.0.0.6**

added new commands by H�vard Leding: pick surface, edge, linked element

# 2019-01-09 **2019.0.0.5**

incremented copyright year to 2019

# 2018-12-13 **2019.0.0.4**

merged issue [#45](https://github.com/jeremytammik/RevitLookup/issues/45) and pull request [#46](https://github.com/jeremytammik/RevitLookup/pull/46)from @TheKidMSX
to center parent for forms

# 2018-05-29 **2019.0.0.2**

merged pull request [#43](https://github.com/jeremytammik/RevitLookup/pull/43) from Levente Koncz @palver123
to use ProgramW6432 variable in csproj to locate Revit API assembly DLLs

# 2018-04-15 **2019.0.0.0**

flat migration to Revit 2019

# 2018-03-12 **2018.0.0.8**

merged pull request [#42](https://github.com/jeremytammik/RevitLookup/pull/42) from @Modis Pekshev:
Add "Search by and snoop" command

# 2018-03-02 **2018.0.0.7**

merged pull request [#41](https://github.com/jeremytammik/RevitLookup/pull/41) from @Modis Pekshev:
Add ConvertToStableRepresentation method for References

# 2018-01-05 **2018.0.0.6**

incremented copyright year to 2018

# 2018-01-05 **2018.0.0.5**

readme enhancements: badges, installer, and updated link to MSI installer

# 2017-08-28 **2018.0.0.3**

merged pull request [#36](https://github.com/jeremytammik/RevitLookup/pull/36) from @Andrey-Bushman:
switch target platform to.Net 4.6 and replace Revit 2017 NuGet package by Revit 2018.1 Nuget package

# 2017-06-05 **2018.0.0.1**

merged pull request [#34](https://github.com/jeremytammik/RevitLookup/pull/34) from @CADBIMDeveloper:
annotative family instance geometry, element enumerations instead of ids, parameter names, and byte property values

# 2017-04-21 **2018.0.0.0**

flat migration to Revit 2018

# 2017-04-07 **2017.0.0.24**

merged pull request [#33](https://github.com/jeremytammik/RevitLookup/pull/33) by @peterhirn
added build status badge

# 2017-03-27 **2017.0.0.23**

dummy modification to trigger build for https://lookupbuilds.com
cf.https://forums.autodesk.com/t5/revit-api-forum/ci-for-revit-lookup/m-p/6947111

# 2017-03-17 **2017.0.0.22**

added 'new' keyword to avoid warning and override inherited methods

# 2017-03-17 **2017.0.0.21**

merged pull request [#31](https://github.com/jeremytammik/RevitLookup/pull/31) from @CADBIMDeveloper
removing try-catch handler

# 2017-03-16 **2017.0.0.20**

merged pull request [#30](https://github.com/jeremytammik/RevitLookup/pull/30) from @eirannejad
adding icon and exception handling

# 2017-03-15 **2017.0.0.19**

merged pull request [#29](https://github.com/jeremytammik/RevitLookup/pull/29) from @CADBIMDeveloper
fixing bugs initialising type and opening background documents

# 2017-03-02 **2017.0.0.18**

merged pull request [#27](https://github.com/jeremytammik/RevitLookup/pull/27) from @CADBIMDeveloper
to display category BuiltInCategory, nullable double properties, and empty lists

# 2017-02-21 **2017.0.0.17**

merged pull request [#26](https://github.com/jeremytammik/RevitLookup/pull/26) from Alexander Ignatovich
to restore ability to see extensible storage content

# 2017-02-17 **2017.0.0.16**

merged pull request [#25](https://github.com/jeremytammik/RevitLookup/pull/25) from chekalin-v:
fix old bugs, significant improvements to the new reflection approach

# 2017-02-06 **2017.0.0.15**

merged pull request [#23](https://github.com/jeremytammik/RevitLookup/pull/23) from awmcc90
to catch specific reflection invocation exceptions, not all

# 2017-02-06 **2017.0.0.14**

merged pull request [#22](https://github.com/jeremytammik/RevitLookup/pull/22) from awmcc90
drastic changes implementing object inspection via reflection and cross-version compatibility

# 2017-02-02 **2017.0.0.13**

whitespace

# 2017-02-02 **2017.0.0.12**

merged pull [#21](https://github.com/jeremytammik/RevitLookup/pull/21) from @eibre adding UnitType property on the parameter Definition class

# 2017-01-06 **2017.0.0.11**

fixed issue [#19](https://github.com/jeremytammik/RevitLookup/issues/19) raised by LeeJaeYoung spot dimension position and text position error

# 2017-01-06 **2017.0.0.10**

merged pull [#20](https://github.com/jeremytammik/RevitLookup/pull/20) from @luftbanana supporting close-with-ESC
to all forms by assigning the cancel button

# 2017-01-03 **2017.0.0.9**

incremented copyright year

# 2016-12-20 **2017.0.0.8**

added version number to Hello World message box

# 2016-12-20 **2017.0.0.7**

merged pull request [#18](https://github.com/jeremytammik/RevitLookup/pull/18) by @Andrey-Bushman
to use NuGet Revit API package

# 2016-08-05 **2017.0.0.6**

merged pull request [#16](https://github.com/jeremytammik/RevitLookup/pull/16) by @arif-hanif
to add post build event to project file to copy addin manifest and dll to addins folder

# 2016-06-23 **2017.0.0.5**

merged pull request [#14](https://github.com/jeremytammik/RevitLookup/pull/14) by Shayneham
to handle exceptions snooping flex pipe and duct lacking levels etc.

# 2016-06-04 **2017.0.0.4**

merged pull request [#13](https://github.com/jeremytammik/RevitLookup/pull/13) by awmcc90
to skip mepSys.Elements for OST_ElectricalInternalCircuits category

# 2016-06-04 **2017.0.0.3**

before merging pull request [#13](https://github.com/jeremytammik/RevitLookup/pull/13) by awmcc90 to skip mepSys.Elements

# 2016-04-19 **2017.0.0.2**

ready for publication

# 2016-04-15 **2017.0.0.1**

microscopic cleanup

# 2016-04-15 **2017.0.0.0**

migration to Revit 2017 Manuel of Sofistik

# 2016-04-04 **2016.0.0.13**

incremented copyright year from 2015 to 2016

# 2015-10-22 **2016.0.0.12**

readme cleanup

# 2015-09-15 **2016.0.0.11**

implemented support for Element bounding box

# 2015-09-01 **2016.0.0.10**

handle null floor.GetAnalyticalModel returned in RAC and RME

# 2015-05-21 **2016.0.0.9**

display all the display names of the BuiltInParameter enumeration value

# 2015-05-15 **2016.0.0.8**

updated Revit API assembly paths for final release of Revit 2016

# 2015-04-23 **2016.0.0.7**

updated post-build event target path to Revit 2016 add-ins folder

# 2015-04-21 **2016.0.0.6**

set Copy Local false on Revit API assemblies

# 2015-04-21 **2016.0.0.5**

initial migration to Revit 2016 - first public release

# 2015-04-20 **2016.0.0.4**

integrated changes from previous Revit 2016 version into elaine's one

# 2015-04-20 **2016.0.0.3**

initial migration to Revit 2016 by @ElaineJieyanZheng

# 2015-04-19 **2015.0.0.8**

integrated pull request #6 by yzraeu, additinal try catch for Level Offset and MEP System

# 2015-01-30 **2015.0.0.7**

removed all statements 'using' the empty root namespace Autodesk.Revit

# 2015-01-29 **2015.0.0.6**

incremented copyright message year from 2014 to 2015

# 2015-01-13 **2015.0.0.5**

added CategoryType to the CategoryCollector

# 2014-11-24 **2015.0.0.4**

encapsulate transaction in using statement

# 2014-11-24 **2015.0.0.3**

merged fix by Tom Pesman @tompesman to catch exception thrown by doc.PrintManager

# 2014-10-06 **2015.0.0.2**

removed obsolete Revit API usage to compile with zero errors and zero warnings

# 2014-04-17 **2015.0.0.1**

recompiled for Revit 2015 UR1

# 2014-04-02 **2015.0.0.0**

initial migration to Revit 2015 Meridian prerelease PR10

# 2014-01-28 **2014.0.1.0**

double checked that this version corresponds with florian's

# 2014-01-27 **2014.0.0.7**

merged pull request from FlorianSchmid of SOFiSTiK:
extended (added) snooping of geometry, FormatOptions, and RevitLinkInstances plus some fixes of compiler errors/warnings; bumped copyright year from 2013 to 2014

# 2014-01-11 **2014.0.0.6**

joespiff adjusted the Anchor property of the "Built-in Enums Map..." button on the Parameters form so that it behaves well when stretching the form

# 2013-10-24 **2014.0.0.5**

merged pull request from Prasadgalle