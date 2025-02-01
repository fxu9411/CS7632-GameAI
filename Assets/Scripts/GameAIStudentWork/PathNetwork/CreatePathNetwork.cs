// Remove the line above if you are subitting to GradeScope for a grade. But leave it if you only want to check
// that your code compiles and the autograder can access your public methods.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAICourse
{
    public class CreatePathNetwork
    {
        public const string StudentAuthorName = "Weixuan Xu";


        // Helper method provided to help you implement this file. Leave as is.
        // Returns Vector2 converted to Vector2Int according to default scaling factor (1000)
        public static Vector2Int ConvertToInt(Vector2 v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns float converted to int according to default scaling factor (1000)
        public static int ConvertToInt(float v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns Vector2Int converted to Vector2 according to default scaling factor (1000)
        public static Vector2 ConvertToFloat(Vector2Int v)
        {
            float f = 1f / (float)CG.FloatToIntFactor;
            return new Vector2(v.x * f, v.y * f);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns int converted to float according to default scaling factor (1000)
        public static float ConvertToFloat(int v)
        {
            float f = 1f / (float)CG.FloatToIntFactor;
            return v * f;
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Returns true is segment AB intersects CD properly or improperly
        static public bool Intersects(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
        {
            return CG.Intersect(a, b, c, d);
        }


        //Get the shortest distance from a point to a line
        //Line is defined by the lineStart and lineEnd points
        public static float DistanceToLineSegment(Vector2Int point, Vector2Int lineStart, Vector2Int lineEnd)
        {
            return CG.DistanceToLineSegment(point, lineStart, lineEnd);
        }


        //Get the shortest distance from a point to a line
        //Line is defined by the lineStart and lineEnd points
        public static float DistanceToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            return CG.DistanceToLineSegment(point, lineStart, lineEnd);
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Determines if a point is inside/on a CCW polygon and if so returns true. False otherwise.
        public static bool IsPointInPolygon(Vector2Int[] polyPts, Vector2Int point)
        {
            return CG.PointPolygonIntersectionType.Outside != CG.InPoly1(polyPts, point);
        }

        // Returns true iff p is strictly to the left of the directed
        // line through a to b.
        // You can use this method to determine if 3 adjacent CCW-ordered
        // vertices of a polygon form a convex or concave angle

        public static bool Left(Vector2Int a, Vector2Int b, Vector2Int p)
        {
            return CG.Left(a, b, p);
        }

        // Vector2 version of above
        public static bool Left(Vector2 a, Vector2 b, Vector2 p)
        {
            return CG.Left(CG.Convert(a), CG.Convert(b), CG.Convert(p));
        }


        //Student code to build the path network from the given pathNodes and Obstacles
        //Obstacles - List of obstacles on the plane
        //agentRadius - the radius of the traversing agent
        //minPoVDist AND maxPoVDist - used for Points of Visibility (see assignment doc)
        //pathNodes - ref parameter that contains the pathNodes to connect (or return if pathNetworkMode is set to PointsOfVisibility)
        //pathEdges - out parameter that will contain the edges you build.
        //  Edges cannot intersect with obstacles or boundaries. Edges must be at least agentRadius distance
        //  from all obstacle/boundary line segments. No self edges, duplicate edges. No null lists (but can be empty)
        //pathNetworkMode - enum that specifies PathNetwork type (see assignment doc)

        public static void Create(Vector2 canvasOrigin, float canvasWidth, float canvasHeight,
            List<Polygon> obstacles, float agentRadius, float minPoVDist, float maxPoVDist, ref List<Vector2> pathNodes,
            out List<List<int>> pathEdges,
            PathNetworkMode pathNetworkMode)
        {
            //STUDENT CODE HERE

            // Initialize the pathEdges list
            pathEdges = InitializePathEdges(pathNodes);

            // Create the path network based on the pathNetworkMode
            for (int i = 0; i < pathNodes.Count; ++i)
            {
                for (int j = i + 1; j < pathNodes.Count; ++j)
                {
                    Vector2 pathNodeA = pathNodes[i];
                    Vector2 pathNodeB = pathNodes[j];

                    if (CanConnect(pathNodeA, pathNodeB, obstacles, canvasOrigin, canvasWidth, canvasHeight,
                            agentRadius))
                    {
                        pathEdges[i].Add(j);
                        pathEdges[j].Add(i);
                    }
                }
            }
            
            // Deduplicate the pathEdges
            for (int i = 0; i < pathEdges.Count; ++i)
            {
                pathEdges[i] = new List<int>(new HashSet<int>(pathEdges[i]));
            }


            // END STUDENT CODE
        }

        private static List<List<int>> InitializePathEdges(List<Vector2> pathNodes)
        {
            List<List<int>> pathEdges;
            pathEdges = new List<List<int>>(pathNodes.Count);

            for (int i = 0; i < pathNodes.Count; ++i)
            {
                pathEdges.Add(new List<int>());
            }

            return pathEdges;
        }

        private static bool CanConnect(Vector2 pathNodeA, Vector2 pathNodeB, List<Polygon> obstacles,
            Vector2 canvasOrigin, float canvasWidth, float canvasHeight, float agentRadius)
        {
            // Two nodes can connect if
            // 1. There's no obstacle or boundary wall between the two path nodes
            // 2. There is sufficient space on either side of the edge so that an agent can follow the line without colliding with any obstacles or boundary wall
            // 3. A path node that is inside the boundary but outside any obstacles
            
            // If the distance between line segment AB and any obstacle is less than agentRadius, return false
            foreach (Polygon obstacle in obstacles)
            {
                for (int i = 0; i < obstacle.getPoints().Length; ++i)
                {
                    Vector2 obstaclePoint = obstacle.getPoints()[i];
                    if (DistanceToLineSegment(obstaclePoint, pathNodeA, pathNodeB) < agentRadius)
                    {
                        return false;
                    }
                }
            }
            
            // If line segment AB intersects with any edge of the obstacle, return false
            for (int i = 0; i < obstacles.Count; ++i)
            {
                Polygon obstacle = obstacles[i];
                for (int j = 0; j < obstacle.getPoints().Length; ++j)
                {
                    Vector2 obstaclePointA = obstacle.getPoints()[j];
                    Vector2 obstaclePointB = obstacle.getPoints()[(j + 1) % obstacle.getPoints().Length];
                    if (Intersects(ConvertToInt(pathNodeA), ConvertToInt(pathNodeB), ConvertToInt(obstaclePointA),
                        ConvertToInt(obstaclePointB))
                    )
                    {
                        return false;
                    }
                }
            }
            
            // If either pathNodeA or pathNodeB's distance to boundary wall is less than agentRadius, return false
            float leftBoundary = canvasOrigin.x + agentRadius;
            float rightBoundary = canvasOrigin.x + canvasWidth - agentRadius;
            float lowerBoundary = canvasOrigin.y + agentRadius;
            float upperBoundary = canvasOrigin.y + canvasHeight - agentRadius;
            if (pathNodeA.x < leftBoundary || pathNodeA.x > rightBoundary ||
                pathNodeA.y < lowerBoundary || pathNodeA.y > upperBoundary ||
                pathNodeB.x < leftBoundary || pathNodeB.x > rightBoundary ||
                pathNodeB.y < lowerBoundary || pathNodeB.y > upperBoundary)
            {
                return false;
            }
            
            // If pathNode to any obstacle edges is less than agentRadius, return false
            foreach (Polygon obstacle in obstacles)
            {
                for (int i = 0; i < obstacle.getPoints().Length; ++i)
                {
                    Vector2 obstaclePointA = obstacle.getPoints()[i];
                    Vector2 obstaclePointB = obstacle.getPoints()[(i + 1) % obstacle.getPoints().Length];
                    if (DistanceToLineSegment(pathNodeA, obstaclePointA, obstaclePointB) < agentRadius || 
                        DistanceToLineSegment(pathNodeB, obstaclePointA, obstaclePointB) < agentRadius)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}