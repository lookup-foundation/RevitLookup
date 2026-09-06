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

namespace RevitLookup.Visualization.Rendering;

/// <summary>
///     Provides geometry computations for the visualization render buffers.
/// </summary>
public static class RenderGeometryHelper
{
    /// <summary>
    ///     Computes the circular cross-section vertices of a tube swept along the specified polyline.
    /// </summary>
    /// <param name="vertices">The polyline vertices the tube is swept along.</param>
    /// <param name="diameter">The diameter of the tube.</param>
    /// <returns>The tessellated circle vertices at each polyline vertex.</returns>
    public static List<List<XYZ>> GetSegmentationTube(IList<XYZ> vertices, double diameter)
    {
        var points = new List<List<XYZ>>();

        for (var i = 0; i < vertices.Count; i++)
        {
            var center = vertices[i];
            XYZ normal;
            if (i == 0)
            {
                normal = (vertices[i + 1] - center).Normalize();
            }
            else if (i == vertices.Count - 1)
            {
                normal = (center - vertices[i - 1]).Normalize();
            }
            else
            {
                normal = ((vertices[i + 1] - vertices[i - 1]) / 2.0).Normalize();
            }

            points.Add(TessellateCircle(center, normal, diameter / 2));
        }

        return points;
    }

    /// <summary>
    ///     Returns the per-vertex normal of the specified mesh, resolved according to its normal distribution.
    /// </summary>
    /// <param name="mesh">The mesh to compute vertex normals for.</param>
    /// <returns>The list of normals, one per mesh vertex.</returns>
    public static List<XYZ> GetMeshVertexNormals(Mesh mesh)
    {
        var vertexCount = mesh.Vertices.Count;
        var normals = new List<XYZ>(vertexCount);

        switch (mesh.DistributionOfNormals)
        {
            case DistributionOfNormals.AtEachPoint:
                for (var i = 0; i < vertexCount; i++)
                {
                    normals.Add(mesh.GetNormal(i));
                }

                break;
            case DistributionOfNormals.OnEachFacet:
                var facetNormals = new XYZ[vertexCount];
                for (var i = 0; i < mesh.NumTriangles; i++)
                {
                    var triangle = mesh.get_Triangle(i);
                    var normal = mesh.GetNormal(i);
                    facetNormals[(int)triangle.get_Index(0)] ??= normal;
                    facetNormals[(int)triangle.get_Index(1)] ??= normal;
                    facetNormals[(int)triangle.get_Index(2)] ??= normal;
                }

                for (var i = 0; i < vertexCount; i++)
                {
                    normals.Add(facetNormals[i] ?? XYZ.Zero);
                }

                break;
            case DistributionOfNormals.OnePerFace:
                var faceNormal = mesh.GetNormal(0);
                for (var i = 0; i < vertexCount; i++)
                {
                    normals.Add(faceNormal);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mesh), mesh.DistributionOfNormals, null);
        }

        return normals;
    }

    /// <summary>
    ///     Tessellates a circle centered at the specified point, perpendicular to the specified normal.
    /// </summary>
    /// <param name="center">The center point of the circle.</param>
    /// <param name="normal">The normal the circle plane is perpendicular to.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <returns>The tessellated circle vertices.</returns>
    public static List<XYZ> TessellateCircle(XYZ center, XYZ normal, double radius)
    {
        var vertices = new List<XYZ>();
        var segmentCount = InterpolateSegmentsCount(radius);
        var xDirection = normal.CrossProduct(XYZ.BasisZ).Normalize() * radius;
        if (xDirection.IsZeroLength())
        {
            xDirection = normal.CrossProduct(XYZ.BasisX).Normalize() * radius;
        }

        var yDirection = normal.CrossProduct(xDirection).Normalize() * radius;

        for (var i = 0; i < segmentCount; i++)
        {
            var angle = 2 * Math.PI * i / segmentCount;
            var vertex = center + xDirection * Math.Cos(angle) + yDirection * Math.Sin(angle);
            vertices.Add(vertex);
        }

        return vertices;
    }

    /// <summary>
    ///     Returns a copy of the specified solid scaled about its bounding box centroid.
    /// </summary>
    /// <param name="solid">The solid to scale.</param>
    /// <param name="scale">The scale factor. A value of <c>1</c> computes a scale that inflates the solid by a small constant offset.</param>
    /// <returns>The scaled solid.</returns>
    public static Solid ScaleSolid(Solid solid, double scale)
    {
        if (scale is 1d)
        {
            scale = EvaluateScale(solid, RevitApiContext.Application.VertexTolerance * 3);
        }

        var centroid = solid.GetBoundingBox().Transform.Origin;
        var moveToCentroid = Transform.CreateTranslation(-centroid);
        var scaleTransform = Transform.Identity.ScaleBasis(scale);
        var moveBack = Transform.CreateTranslation(centroid);
        var combinedTransform = moveBack.Multiply(scaleTransform).Multiply(moveToCentroid);
        return SolidUtils.CreateTransformed(solid, combinedTransform);
    }

    /// <summary>
    ///     Determines the number of tessellation segments for the specified tube diameter.
    /// </summary>
    /// <param name="diameter">The tube diameter.</param>
    /// <returns>The number of segments, clamped between a minimum and maximum count.</returns>
    public static int InterpolateSegmentsCount(double diameter)
    {
        const int minSegments = 6;
        const int maxSegments = 33;
        const double minDiameter = 0.1 / 12d;
        const double maxDiameter = 3 / 12d;

        if (diameter <= minDiameter)
        {
            return minSegments;
        }

        if (diameter >= maxDiameter)
        {
            return maxSegments;
        }

        var normalDiameter = (diameter - minDiameter) / (maxDiameter - minDiameter);
        return (int)(minSegments + normalDiameter * (maxSegments - minSegments));
    }

    /// <summary>
    ///     Determines the surface-to-mesh-grid offset for the specified tube diameter.
    /// </summary>
    /// <param name="diameter">The tube diameter.</param>
    /// <returns>The interpolated offset, clamped between a minimum and maximum value.</returns>
    public static double InterpolateOffsetByDiameter(double diameter)
    {
        const double minOffset = 0.01d;
        const double maxOffset = 0.1d;
        const double minDiameter = 0.1 / 12d;
        const double maxDiameter = 3 / 12d;

        if (diameter <= minDiameter)
        {
            return minOffset;
        }

        if (diameter >= maxDiameter)
        {
            return maxOffset;
        }

        var normalOffset = (diameter - minDiameter) / (maxDiameter - minDiameter);
        return minOffset + normalOffset * (maxOffset - minOffset);
    }

    /// <summary>
    ///     Determines the surface-to-mesh-grid offset for the specified face or mesh area.
    /// </summary>
    /// <param name="area">The face or mesh area.</param>
    /// <returns>The interpolated offset, clamped between a minimum and maximum value.</returns>
    public static double InterpolateOffsetByArea(double area)
    {
        const double minOffset = 0.01d;
        const double maxOffset = 0.1d;
        const double minArea = 0.01d;
        const double maxArea = 1d;

        if (area <= minArea)
        {
            return minOffset;
        }

        if (area >= maxArea)
        {
            return maxOffset;
        }

        var normalOffset = (area - minArea) / (maxArea - minArea);
        return minOffset + normalOffset * (maxOffset - minOffset);
    }

    /// <summary>
    ///     Determines the normal vector axis length for the specified face or mesh area.
    /// </summary>
    /// <param name="area">The face or mesh area.</param>
    /// <returns>The interpolated axis length, clamped between a minimum and maximum value.</returns>
    public static double InterpolateAxisLengthByArea(double area)
    {
        const double minLength = 0.1d;
        const double maxLength = 1d;
        const double minArea = 0.01d;
        const double maxArea = 1d;

        if (area <= minArea)
        {
            return minLength;
        }

        if (area >= maxArea)
        {
            return maxLength;
        }

        var normalOffset = (area - minArea) / (maxArea - minArea);
        return minLength + normalOffset * (maxLength - minLength);
    }

    /// <summary>
    ///     Determines the axis length for a bounding box spanning the specified corners.
    /// </summary>
    /// <param name="min">The minimum corner of the bounding box.</param>
    /// <param name="max">The maximum corner of the bounding box.</param>
    /// <returns>The interpolated axis length.</returns>
    public static double InterpolateAxisLengthByPoints(XYZ min, XYZ max)
    {
        const double maxLength = 1d;

        var width = max.X - min.X;
        var height = max.Y - min.Y;
        var depth = max.Z - min.Z;

        var maxSize = Math.Max(width, Math.Max(height, depth));

        if (maxLength * 2 < maxSize)
        {
            return maxLength;
        }

        return maxSize * 0.35;
    }

    /// <summary>
    ///     Computes the total surface area of the specified mesh.
    /// </summary>
    /// <param name="mesh">The mesh to compute the surface area for.</param>
    /// <returns>The sum of the areas of the mesh triangles.</returns>
    public static double ComputeMeshSurfaceArea(Mesh mesh)
    {
#if REVIT2024_OR_GREATER
        return mesh.ComputeSurfaceArea();
#else
        var surfaceArea = 0.0;

        for (var i = 0; i < mesh.NumTriangles; i++)
        {
            var triangle = mesh.get_Triangle(i);

            var vertex0 = triangle.get_Vertex(0);
            var vertex1 = triangle.get_Vertex(1);
            var vertex2 = triangle.get_Vertex(2);

            var side1 = vertex1 - vertex0;
            var side2 = vertex2 - vertex0;

            var crossProduct = side1.CrossProduct(side2);

            surfaceArea += crossProduct.GetLength() / 2.0;
        }

        return surfaceArea;
#endif
    }

    private static double EvaluateScale(Solid solid, double offset)
    {
        var boundingBox = solid.GetBoundingBox();

        var currentLength = boundingBox.Max.X - boundingBox.Min.X;
        var currentWidth = boundingBox.Max.Y - boundingBox.Min.Y;
        var currentHeight = boundingBox.Max.Z - boundingBox.Min.Z;

        var maxDimension = Math.Max(Math.Max(currentLength, currentWidth), currentHeight);

        if (Math.Abs(maxDimension - currentLength) < 1e-9)
        {
            return (currentLength + offset) / currentLength;
        }

        if (Math.Abs(maxDimension - currentWidth) < 1e-9)
        {
            return (currentWidth + offset) / currentWidth;
        }

        return (currentHeight + offset) / currentHeight;
    }
}
