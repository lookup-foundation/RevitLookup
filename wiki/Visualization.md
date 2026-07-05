In Revit, geometry is at the core of every model. 
Whether you are dealing with simple shapes or intricate structures, having the ability to visualize geometric elements can significantly improve your workflow, analysis and understanding of the BIM.

Visualization available in the context menu for each object:

![image](https://github.com/lookup-foundation/RevitLookup/assets/20504884/521a8f6a-ac4a-4c41-92d8-8e5655991c55)

> [!IMPORTANT]  
> The geometry is not updated when the element is updated. If you change the object properties to display the new geometry, retrieve it again

## Mesh visualization

The Mesh Visualization feature emphasizes the detailed structure of a mesh, providing insights into its geometric composition.
Meshes consist of vertices, forming a network that approximates the surface of a 3D object.
This feature enables to:

- Explore the intricate details of a mesh's structure.
- Understand the distribution and vertices organization.
- Analyze the mesh orientation faces through normal vectors.

![image](https://github.com/lookup-foundation/RevitLookup/assets/20504884/84cd42fe-5248-4c13-8f30-0869396ad3b8)

| Setting       | Description                                                                                    |
|---------------|------------------------------------------------------------------------------------------------|
| Surface       | Shows the mesh surface                                                                         |
| Extrusion     | Adjusts the depth of the mesh surface extrusion to enhance the 3D appearance for small objects |
| Transparency  | Sets the mesh surface transparency                                                             |
| Mesh grid     | Shows the mesh grid lines, highlighting the individual faces and their connections             |
| Normal vector | Shows the normal vectors of the mesh vertices                                                  |

## Face visualization

The Face Visualization feature focuses on rendering and inspecting individual faces of solid geometry.
Faces are planar or curved surfaces that define the boundaries of a solid.
This feature enables users to:

- Inspect the detailed structure of a face within the model.
- Visualize the orientation and normal vector of a face.

![image](https://github.com/lookup-foundation/RevitLookup/assets/20504884/15ba15da-325e-499f-935e-fa5cc9b71390)

| Setting       | Description                                                                                    |
|---------------|------------------------------------------------------------------------------------------------|
| Surface       | Shows the face surface                                                                         |
| Extrusion     | Adjusts the depth of the face surface extrusion to enhance the 3D appearance for small objects |
| Transparency  | Sets the face surface transparency                                                             |
| Mesh grid     | Shows the mesh grid lines, highlighting the individual faces and their connections.            |
| Normal vector | Shows the face centroid normal vector                                                          |

## Solid visualization

The Solid Visualization feature provides a comprehensive view of the entire solid geometry within a model.
Solids are volumetric shapes that define the three-dimensional space occupied by objects.
This feature enables users to:

- Comprehend the full volume of the solids.
- Analyze the geometric boundaries and surfaces of solids.
- Inspect the internal and external structure of complex solid forms.

![image](https://github.com/lookup-foundation/RevitLookup/assets/20504884/dc3ce6e3-25a3-496d-a014-84d5df43d2dc)

| Setting      | Description                                                            |
|--------------|------------------------------------------------------------------------|
| Faces        | Shows the solid faces                                                  |
| Transparency | Sets the face surface transparency                                     |
| Scale        | Adjusts the solid scale to enhance the 3D appearance for small objects |
| Edges        | Shows the solid edges                                                  |

The **wireframe** display mode is best suited for this type of geometry. 
However, if you need to completely isolate the Revit geometry from the visualised geometry, temporarily **hide** the element being explored.
To analyze internal structure, use section box, hide the element and crop it.

## Curve visualization

The Curve Visualization feature highlights curves within the geometry, including lines, arcs, splines, and others.
Curves define the paths and shapes in a model.
This feature enables users to:

- Visualize the detailed curve structure.
- Visualize the curve segments directions.
- Analyze the curvature and continuity of lines, arcs, and splines.

![image](https://github.com/lookup-foundation/RevitLookup/assets/20504884/d08b0bf3-0622-4f49-b999-4365a0955129)

| Setting      | Description                                                               |
|--------------|---------------------------------------------------------------------------|
| Surface      | Shows the surface along the curve path                                    |
| Diameter     | Adjusts the diameter of the curve visualization, enhancing its visibility |
| Transparency | Adjusts the transparency level of the curve surface                       |
| Polyline     | Shows the polyline approximation of the curve                             |
| Direction    | Shows the curve segments directions                                       |

Visualisation is only available for bound or cyclic curves

## Edge visualization

The Edge Visualization feature focuses on the edges of geometry, which are the lines where two faces meet.
This feature enables users to:

- Analyze the connectivity and topology of edges.
- Visualize the edge segments directions.

![image](https://github.com/lookup-foundation/RevitLookup/assets/20504884/30291e03-8eb8-4de2-a54f-0c288ee4dcb2)

| Setting      | Description                                                              |
|--------------|--------------------------------------------------------------------------|
| Surface      | Shows the surface along the edge path                                    |
| Diameter     | Adjusts the diameter of the edge visualization, enhancing its visibility |
| Transparency | Adjusts the transparency level of the edge surface                       |
| Polyline     | Shows the polyline approximation of the edge                             |
| Direction    | Shows the edge segments directions                                       |

## CurveLoop visualization

The CurveLoop Visualization feature focuses on visualizing a collection of curves, known as a CurveLoop. A CurveLoop is an array of curves that define a closed loop in the geometry. This feature enables users to:

- Analyze the connectivity and topology of the curves within the loop.
- Visualize the direction and continuity of the curve segments.

![image](https://github.com/user-attachments/assets/f4d935f5-2cfd-44d2-b1c7-d5fdc07e95a1)

| Setting      | Description                                                                |
|--------------|----------------------------------------------------------------------------|
| Surface      | Shows the surface along the curve loop path                                |
| Diameter     | Adjusts the diameter of the curves visualization, enhancing its visibility |
| Transparency | Adjusts the transparency level of the curves surface                       |
| Curve Loop   | Shows the curves of the curve loop                                         |
| Direction    | Shows the curve segments directions within the loop                        | 

## Bounding box visualization

The Bounding Box Visualization feature represents the minimal box aligned with the cardinal axes that entirely encloses a geometric object. 
This feature enables users to:

- Understand the spatial extent of an object.
- Analyze the dimensions and proportions of the bounding box.
- Inspect how objects fit within the defined space constraints.

![image](https://github.com/lookup-foundation/RevitLookup/assets/20504884/f800a552-86df-4554-8d5b-c53561720f0d)

| Setting      | Description                                                                        |
|--------------|------------------------------------------------------------------------------------|
| Surface      | Shows the surface of the bounding box                                              |
| Transparency | Adjusts the transparency level of the bounding box surface                         |
| Edges        | Shows the edges of the bounding box, providing a clear outline                     |
| Axis         | Shows the axis vectors of the bounding box, indicating min and max points location |

Transform is applied to the BoundingBox when visualising it

## XYZ visualization

The XYZ Visualization feature illustrates the coordinates and orientation of points in the model space.
This feature enables users to:

- Visualize the position of points in the 3D space.
- Inspect the relationships between different coordinates in the model.

![image](https://github.com/lookup-foundation/RevitLookup/assets/20504884/72b3f7cb-279c-4465-9cff-7918e0aaf37c)

| Setting | Description                                                                                 |
|---------|---------------------------------------------------------------------------------------------|
| Planes  | Shows the coordinate planes for X, Y, and Z axes, helping to visualize spatial orientation  |
| Length  | Adjusts the length of the planes and axes, providing a better scale reference for the model |
| X axis  | Shows the X axis direction and its alignment in the 3D space                                |
| Y axis  | Shows the Y axis direction and its alignment in the 3D space                                |
| Z axis  | Shows the Z axis direction and its alignment in the 3D space                                |

Visualisation is not available for unit length vectors.

## Color Picker editor

To fine-tune your chosen color, select the central color in the color bar. 
The fine-tuning control lets you change the color's HSV, RGB, and HEX values.

To choose a similar color, select one of the segments on the left and right edges of the color bar.

![image](https://github.com/lookup-foundation/RevitLookup/assets/20504884/5bfc8d4f-3583-4fed-b990-84f97f2e4196)

## Settings

Settings are saved between sessions. To completely reset to default values, open the settings and perform a reset

![image](https://github.com/lookup-foundation/RevitLookup/assets/20504884/09934326-2f51-4dd4-a7f2-77bfd1196665)