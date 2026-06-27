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

namespace RevitLookup.Core.Visualization.Helpers;

public static class RenderGeometryHelper
{
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
                    facetNormals[(int) triangle.get_Index(0)] ??= normal;
                    facetNormals[(int) triangle.get_Index(1)] ??= normal;
                    facetNormals[(int) triangle.get_Index(2)] ??= normal;
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

    public static Solid ScaleSolid(Solid solid, double scale)
    {
        if (scale is 1d) scale = EvaluateScale(solid, RevitApiContext.Application.VertexTolerance * 3);

        var centroid = solid.GetBoundingBox().Transform.Origin;
        var moveToCentroid = Transform.CreateTranslation(-centroid);
        var scaleTransform = Transform.Identity.ScaleBasis(scale);
        var moveBack = Transform.CreateTranslation(centroid);
        var combinedTransform = moveBack.Multiply(scaleTransform).Multiply(moveToCentroid);
        return SolidUtils.CreateTransformed(solid, combinedTransform);
    }

    public static int InterpolateSegmentsCount(double diameter)
    {
        const int minSegments = 6;
        const int maxSegments = 33;
        const double minDiameter = 0.1 / 12d;
        const double maxDiameter = 3 / 12d;

        if (diameter <= minDiameter) return minSegments;
        if (diameter >= maxDiameter) return maxSegments;

        var normalDiameter = (diameter - minDiameter) / (maxDiameter - minDiameter);
        return (int) (minSegments + normalDiameter * (maxSegments - minSegments));
    }

    public static double InterpolateOffsetByDiameter(double diameter)
    {
        const double minOffset = 0.01d;
        const double maxOffset = 0.1d;
        const double minDiameter = 0.1 / 12d;
        const double maxDiameter = 3 / 12d;

        if (diameter <= minDiameter) return minOffset;
        if (diameter >= maxDiameter) return maxOffset;

        var normalOffset = (diameter - minDiameter) / (maxDiameter - minDiameter);
        return minOffset + normalOffset * (maxOffset - minOffset);
    }

    public static double InterpolateOffsetByArea(double area)
    {
        const double minOffset = 0.01d;
        const double maxOffset = 0.1d;
        const double minArea = 0.01d;
        const double maxArea = 1d;

        if (area <= minArea) return minOffset;
        if (area >= maxArea) return maxOffset;

        var normalOffset = (area - minArea) / (maxArea - minArea);
        return minOffset + normalOffset * (maxOffset - minOffset);
    }

    public static double InterpolateAxisLengthByArea(double area)
    {
        const double minLength = 0.1d;
        const double maxLength = 1d;
        const double minArea = 0.01d;
        const double maxArea = 1d;

        if (area <= minArea) return minLength;
        if (area >= maxArea) return maxLength;

        var normalOffset = (area - minArea) / (maxArea - minArea);
        return minLength + normalOffset * (maxLength - minLength);
    }

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

        if (Math.Abs(maxDimension - currentLength) < 1e-9) return (currentLength + offset) / currentLength;
        if (Math.Abs(maxDimension - currentWidth) < 1e-9) return (currentWidth + offset) / currentWidth;
        return (currentHeight + offset) / currentHeight;
    }
}