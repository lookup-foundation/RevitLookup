RevitLookup provides functionality to display methods in the user interface that do not exist in the Revit API or implemented as a Util classes.
These extensions expose useful functionality in the context of specific objects.

> [!NOTE]  
> The available extensions may vary depending on the version of Revit you are using.

The table below lists the extensions that are available in RevitLookup:

| Type           | Extension                                 | API method                                                                       |
|:---------------|-------------------------------------------|----------------------------------------------------------------------------------|
| Application    | GetFormulaFunctions                       | FormulaManager.GetFormulaFunctions                                               |
| Application    | GetFormulaOperators                       | FormulaManager.GetFormulaOperators                                               |
| Application    | GetMacroManager                           | MacroManager.GetMacroManager                                                     |
| UIApplication  | CurrentTheme                              | UIThemeManager.CurrentTheme                                                      |
| UIApplication  | CurrentCanvasTheme                        | UIThemeManager.CurrentCanvasTheme                                                |
| UIApplication  | FollowSystemColorTheme                    | UIThemeManager.FollowSystemColorTheme                                            |
| Document       | GetAllGlobalParameters                    | GlobalParametersManager.GetAllGlobalParameters                                   |
| Document       | GetLightGroupManager                      | LightGroupManager.GetLightGroupManager                                           |
| Document       | GetTemporaryGraphicsManager               | TemporaryGraphicsManager.GetTemporaryGraphicsManager                             |
| Document       | GetAnalyticalToPhysicalAssociationManager | AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager |
| Document       | CreateFamilySizeTableManager              | FamilySizeTableManager.CreateFamilySizeTableManager                              |
| Document       | GetLightFamily                            | LightFamily.GetLightFamily                                                       |
| Document       | GetMacroManager                           | MacroManager.GetMacroManager                                                     |
| Element        | CanBeMirrored                             | ElementTransformUtils.CanBeMirrored                                              |
| Element        | GetJoinedElements                         | JoinGeometryUtils.GetJoinedElements                                              |
| Element        | GetCuttingSolids                          | SolidSolidCutUtils.GetCuttingSolids                                              |
| Element        | GetSolidsBeingCut                         | SolidSolidCutUtils.GetSolidsBeingCut                                             |
| Element        | IsAllowedForSolidCut                      | SolidSolidCutUtils.IsAllowedForSolidCut                                          |
| Element        | IsElementFromAppropriateContext           | SolidSolidCutUtils.IsElementFromAppropriateContext                               |
| Element        | GetCheckoutStatus                         | WorksharingUtils.GetCheckoutStatus                                               |
| Element        | GetWorksharingTooltipInfo                 | WorksharingUtils.GetWorksharingTooltipInfo                                       |
| Element        | GetModelUpdatesStatus                     | WorksharingUtils.GetModelUpdatesStatus                                           |
| Element        | AreElementsValidForCreateParts            | PartUtils.AreElementsValidForCreateParts                                         |
| Element        | CanDeleteElement                          | DocumentValidation.CanDeleteElement                                              |
| FamilyInstance | GetInstancePlacementPointElementRefIds    | AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds            |
| FamilyInstance | IsAdaptiveComponentInstance               | AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance                       |
| Family         | GetFamilySizeTableManager                 | FamilySizeTableManager.GetFamilySizeTableManager                                 |
| Family         | FamilyCanConvertToFaceHostBased           | FamilyUtils.FamilyCanConvertToFaceHostBased                                      |
| Family         | GetProfileSymbols                         | FamilyUtils.GetProfileSymbols                                                    |
| HostObject     | GetBottomFaces                            | HostObjectUtils.GetBottomFaces                                                   |
| HostObject     | GetTopFaces                               | HostObjectUtils.GetTopFaces                                                      |
| HostObject     | GetSideFaces                              | HostObjectUtils.GetSideFaces                                                     |
| View           | GetSpatialFieldManager                    | SpatialFieldManager.GetSpatialFieldManager                                       |
| View           | GetAllPlacedInstances                     | -                                                                                |
| Pipe           | HasOpenConnector                          | PlumbingUtils.HasOpenConnector                                                   |
| Wall           | IsWallJoinAllowedAtEnd                    | WallUtils.IsWallJoinAllowedAtEnd                                                 |
| Parameter      | AsBool                                    | -                                                                                |
| Parameter      | AsColor                                   | -                                                                                |
| Parameter      | GetAssociatedFamilyParameter              | FamilyManager.GetAssociatedFamilyParameter                                       |
| ForgeTypeId    | ToUnitLabel                               | Returns user visible label for unit                                              |
| ForgeTypeId    | ToSpecLabel                               | Returns user visible label for spec                                              |
| ForgeTypeId    | ToSymbolLabel                             | Returns user visible label for symbol                                            |
| ForgeTypeId    | ToGroupLabel                              | Returns user visible label for group                                             |
| ForgeTypeId    | ToDisciplineLabel                         | Returns user visible label for discipline                                        |
| ForgeTypeId    | ToParameterLabel                          | Returns user visible label for parameter                                         |
| ForgeTypeId    | IsSymbol                                  | UnitUtils.IsSymbol                                                               |
| ForgeTypeId    | IsUnit                                    | UnitUtils.IsUnit                                                                 |
| ForgeTypeId    | IsSpec                                    | UnitUtils.IsSpec                                                                 |
| ForgeTypeId    | IsMeasurableSpec                          | UnitUtils.IsMeasurableSpec                                                       |
| ForgeTypeId    | IsBuiltInParameter                        | ParameterUtils.IsBuiltInParameter                                                |
| ForgeTypeId    | IsBuiltInGroup                            | ParameterUtils.IsBuiltInGroup                                                    |
| Category       | BuiltInCategory                           | -                                                                                |
| Category       | GetElements                               | -                                                                                |
| Schema         | GetElements                               | -                                                                                |
| Color          | Name                                      | -                                                                                |
| Color          | HEX                                       | -                                                                                |
| Color          | HEX int                                   | -                                                                                |
| Color          | RGB                                       | -                                                                                |
| Color          | HSL                                       | -                                                                                |
| Color          | HSV                                       | -                                                                                |
| Color          | CMYK                                      | -                                                                                |
| Color          | HSB                                       | -                                                                                |
| Color          | HSI                                       | -                                                                                |
| Color          | HWB                                       | -                                                                                |
| Color          | NCol                                      | -                                                                                |
| Color          | CIELAB                                    | -                                                                                |
| Color          | CIEXYZ                                    | -                                                                                |
| Color          | VEC4                                      | -                                                                                |
| Color          | Decimal                                   | -                                                                                |
| Solid          | SplitVolumes                              | SolidUtils.SplitVolumes                                                          |
| Solid          | IsValidForTessellation                    | SolidUtils.IsValidForTessellation                                                |
| BoundingBoxXYZ | Centroid                                  | -                                                                                |
| BoundingBoxXYZ | Vertices                                  | -                                                                                |
| BoundingBoxXYZ | Volume                                    | -                                                                                |
| BoundingBoxXYZ | SurfaceArea                               | -                                                                                |
| ModelPath      | ConvertModelPathToUserVisiblePath         | ModelPathUtils.ConvertModelPathToUserVisiblePath                                 |
| ModelPath      | IsDocumentTransmitted                     | TransmissionData.IsDocumentTransmitted                                           |
| ModelPath      | DocumentIsNotTransmitted                  | TransmissionData.DocumentIsNotTransmitted                                        |
| ModelPath      | ReadTransmissionData                      | TransmissionData.ReadTransmissionData                                            |
| ModelPath      | GetUserWorksetInfo                        | WorksharingUtils.GetUserWorksetInfo                                              |
| Part           | IsMergedPart                              | PartUtils.IsMergedPart                                                           |
| Part           | IsPartDerivedFromLink                     | PartUtils.IsPartDerivedFromLink                                                  |
| Part           | GetChainLengthToOriginal                  | PartUtils.GetChainLengthToOriginal                                               |
| Part           | GetMergedParts                            | PartUtils.GetMergedParts                                                         |
| Part           | ArePartsValidForDivide                    | PartUtils.ArePartsValidForDivide                                                 |
| Part           | FindMergeableClusters                     | PartUtils.FindMergeableClusters                                                  |
| Part           | ArePartsValidForMerge                     | PartUtils.ArePartsValidForMerge                                                  |
| Part           | GetAssociatedPartMaker                    | PartUtils.GetAssociatedPartMaker                                                 |
| Part           | GetSplittingCurves                        | PartUtils.GetSplittingCurves                                                     |
| Part           | GetSplittingElements                      | PartUtils.GetSplittingElements                                                   |
| Part           | HasAssociatedParts                        | PartUtils.HasAssociatedParts                                                     |
| PartMaker      | GetPartMakerMethodToDivideVolumeFW        | PartUtils.GetPartMakerMethodToDivideVolumeFW                                     |
