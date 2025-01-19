using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalHelper 
{


    public static List<Vector2Int> PolysIntersections(List<Polygon> polys)
    {
        var intersections = new List<Vector2Int>();

        if (polys == null)
            return intersections;

        var polysCount = polys.Count;

        for (int i = 0; i < polysCount; ++i)
        {
            var polyA = polys[i];

            for (int j = i + 1; j < polysCount; ++j)
            {
                var polyB = polys[j];

                var ret = PolyPolyEdgeIntersections(polyA.getIntegerPoints(), polyB.getIntegerPoints());
                intersections.AddRange(ret);
            }

        }

        return intersections;

    }

    public static List<Vector2Int> PolyPolyEdgeIntersections(Vector2Int[] polyA, Vector2Int[] polyB)
    {

        var intersections = new List<Vector2Int>();

        var polyALen = polyA.Length;
        var ccwA = CG.Ccw(polyA);

        if (polyA == null || polyALen < 3)
        {
            Debug.LogError($"Bad poly passed: vert count: {polyALen} ccw?: {ccwA}");
            return intersections;
        }



        var polyBLen = polyB.Length;
        var ccwB = CG.Ccw(polyB);

        if (polyB == null || polyBLen < 3)
        {
            Debug.LogError($"Bad poly passed: vert count: {polyBLen} ccw?: {ccwB}");
            return intersections;
        }



        for (int i = 0, j = polyALen - 1; i < polyALen; j = i++)
        {

            var A = polyA[j];
            var B = polyA[i];

            for (int k = 0, l = polyBLen - 1; k < polyBLen; l = k++)
            {
                var C = polyB[l];
                var D = polyB[k];

                var ret = CG.SegSegInt(A, B, C, D, out var p, out var q, out var pi, out var qi);

                if (ret == CG.LineSegmentIntersectionType.IntersectionProper)
                {
                    intersections.Add(pi);
                }

            }

        }

        return intersections;

    }



    // Tests whether two polygons intersect (touch or overlap), returning true if so, false otherwise
    public static bool PolysIntersect(Vector2Int[] polyA, Vector2Int[] polyB)
    {

        var polyALen = polyA.Length;
        var ccwA = CG.Ccw(polyA);

        if (polyA == null || polyALen < 3)
        {
            Debug.LogError($"Bad poly passed: vert count: {polyALen} ccw?: {ccwA}");
            return false;
        }



        var polyBLen = polyB.Length;
        var ccwB = CG.Ccw(polyB);

        if (polyB == null || polyBLen < 3)
        {
            Debug.LogError($"Bad poly passed: vert count: {polyBLen} ccw?: {ccwB}");
            return false;
        }


        // edge intersections (including touching) handle all overlaps including
        // through vertices, and perfectly aligned identical polygons

        for (int i = 0, j = polyALen - 1; i < polyALen; j = i++)
        {
            var A = polyA[j];
            var B = polyA[i];

            if (IntersectionLineSegmentWithPolygon(A, B, polyB))
            {
                return true;
            }
        }

        // check for points of one poly inside other. Will indicate a fully contained
        // polygon

        for (int i = 0; i < polyALen; i++)
        {
            if (CG.InPoly1(polyB, polyA[i]) != CG.PointPolygonIntersectionType.Outside)
                return true;
        }


        for (int i = 0; i < polyBLen; i++)
        {
            if (CG.InPoly1(polyA, polyB[i]) != CG.PointPolygonIntersectionType.Outside)
                return true;
        }


        return false;
    }


    public static bool IntersectionLineSegmentWithPolygons(Vector2Int A, Vector2Int B, List<Polygon> polys)
    {
        foreach (var poly in polys)
        {
            if (IntersectionLineSegmentWithPolygon(A, B, poly.getIntegerPoints()))
                return true;
        }

        return false;
    }


    // (OutsideOrOnToOnOrInside) IntersectionLineSegmentWithPolygon():
    // This tests whether a line segment that starts outside (or on) a polygon crosses inside
    // (or stays on or touches the outside). It doesn't test for segment AB entirely inside the poly.
    // In short, this method returns true if a line segment touches one or more of the poly segments
    public static bool IntersectionLineSegmentWithPolygon(Vector2Int A, Vector2Int B, Vector2Int[] poly)
    {


        var len = poly.Length;
        var ccw = CG.Ccw(poly);

        if (poly == null || len < 3 || !ccw)
        {
            Debug.LogError($"Bad poly passed: vert count: {len} ccw?: {ccw}");
            return false;
        }

        for (int i = 0, j = len - 1; i < len; j = i++)
        {
            var C = poly[j];
            var D = poly[i];

            if (CG.Intersect(A, B, C, D))
            {
                return true;
            }
        }

        return false;
    }


}
