// compile_check
// Remove the line above if you are subitting to GradeScope for a grade. But leave it if you only want to check
// that your code compiles and the autograder can access your public methods.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAICourse
{
    public class CreateGrid
    {
        // Please change this string to your name
        public const string StudentAuthorName = "Weixuan Xu";


        // Helper method provided to help you implement this file. Leave as is.
        // Returns true if point p is inside (or on edge) the polygon defined by pts (CCW winding). False, otherwise
        static bool IsPointInsidePolygon(Vector2Int[] pts, Vector2Int p)
        {
            return CG.InPoly1(pts, p) != CG.PointPolygonIntersectionType.Outside;
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Returns float converted to int according to default scaling factor (1000)
        static int Convert(float v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns Vector2 converted to Vector2Int according to default scaling factor (1000)
        static Vector2Int Convert(Vector2 v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns true is segment AB intersects CD properly or improperly
        static bool Intersects(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
        {
            return CG.Intersect(a, b, c, d);
        }


        // IsPointInsideBoundingBox(): Determines whether a point (Vector2Int:p) is On/Inside a bounding box (such as a grid cell) defined by
        // minCellBounds and maxCellBounds (both Vector2Int's).
        // Returns true if the point is ON/INSIDE the cell and false otherwise
        // This method should return true if the point p is on one of the edges of the cell.
        // This is more efficient than PointInsidePolygon() for an equivalent dimension poly
        // Preconditions: minCellBounds <= maxCellBounds, per dimension
        static bool IsPointInsideAxisAlignedBoundingBox(Vector2Int minCellBounds, Vector2Int maxCellBounds,
            Vector2Int p)
        {
            if (p.x > maxCellBounds.x || p.x < minCellBounds.x || p.y > maxCellBounds.y || p.y < minCellBounds.y)
            {
                return false;
            }

            // placeholder logic to be replaced by the student
            return true;
        }


        // IsRangeOverlapping(): Determines if the range (inclusive) from min1 to max1 overlaps the range (inclusive) from min2 to max2.
        // The ranges are considered to overlap if one or more values is within the range of both.
        // Returns true if overlap, false otherwise.
        // Preconditions: min1 <= max1 AND min2 <= max2
        static bool IsRangeOverlapping(int min1, int max1, int min2, int max2)
        {
            if (min1 <= max2 && min2 <= max1)
            {
                return true;
            }

            return false;
        }

        // IsAxisAlignedBouningBoxOverlapping(): Determines if the AABBs defined by min1,max1 and min2,max2 overlap or touch
        // Returns true if overlap, false otherwise.
        // Preconditions: min1 <= max1, per dimension. min2 <= max2 per dimension
        static bool IsAxisAlignedBoundingBoxOverlapping(Vector2Int min1, Vector2Int max1, Vector2Int min2,
            Vector2Int max2)
        {
            // HINT: Call IsRangeOverlapping()
            if (IsRangeOverlapping(min1.x, max1.x, min2.x, max2.x) &&
                IsRangeOverlapping(min1.y, max1.y, min2.y, max2.y))
            {
                return true;
            }

            return false;
        }


        // IsTraversable(): returns true if the grid is traversable from grid[x,y] in the direction dir, false otherwise.
        // The grid boundaries are not traversable. If the grid position x,y is itself not traversable but the grid cell in direction
        // dir is traversable, the function will return false.
        // returns false if the grid is null, grid rank is not 2 dimensional, or any dimension of grid is zero length
        // returns false if x,y is out of range
        // Note: public methods are autograded
        public static bool IsTraversable(bool[,] grid, int x, int y, TraverseDirection dir)
        {
            // TODO IMPLEMENT

            //placeholder logic to be replaced by the student
            return true;
        }


        // Create(): Creates a grid lattice discretized space for navigation.
        // canvasOrigin: bottom left corner of navigable region in world coordinates
        // canvasWidth: width of navigable region in world dimensions
        // canvasHeight: height of navigable region in world dimensions
        // cellWidth: target cell width (of a grid cell) in world dimensions
        // obstacles: a list of collider obstacles
        // grid: an array of bools. A cell is true if navigable, false otherwise
        //    Example: grid[x_pos, y_pos]

        public static void Create(Vector2 canvasOrigin, float canvasWidth, float canvasHeight, float cellWidth,
            List<Polygon> obstacles,
            out bool[,] grid
        )
        {
            // ignoring the obstacles for this limited demo; 
            // Marks cells of the grid untraversable if geometry intersects interior!
            // Carefully consider all possible geometry interactions

            // also ignoring the world boundary defined by canvasOrigin and canvasWidth and canvasHeight

            Debug.Log(canvasOrigin);

            int rows = Mathf.FloorToInt(canvasWidth / cellWidth);
            int cols = Mathf.FloorToInt(canvasHeight / cellWidth);
            Vector2Int canvasOriginConverted = Convert(canvasOrigin);
            Vector2Int cellGridScale = new Vector2Int(Convert(cellWidth), Convert(cellWidth));

            Debug.Log(canvasOriginConverted);
            Debug.Log(cellGridScale);
            foreach (Polygon p in obstacles)
            {
                foreach (Vector2Int v in p.getIntegerPoints())
                {
                    Debug.Log(v);
                }
            }

            grid = new bool[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    grid[row, col] = true;
                }
            }

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Vector2Int lowerLeft = cellGridScale * new Vector2Int(row, col) + canvasOriginConverted;
                    Vector2Int upperLeft = cellGridScale * new Vector2Int(row + 1, col) + canvasOriginConverted;
                    Vector2Int lowerRight = cellGridScale * new Vector2Int(row, col + 1) + canvasOriginConverted;
                    Vector2Int upperRight = cellGridScale * new Vector2Int(row + 1, col + 1) + canvasOriginConverted;

                    Vector2Int[] cellGrid = { upperRight, lowerRight, lowerLeft, upperLeft };
                    
                    foreach (Polygon obstacle in obstacles)
                    {
                        Vector2Int[] polygon = obstacle.getIntegerPoints(); // +- 600, += 600

                        foreach (Vector2Int point in polygon)
                        {
                            if (IsPointInsidePolygon(cellGrid, point))
                            {
                                grid[row, col] = false;
                            }
                        }
                    }
                }
            }
        }
    }
}