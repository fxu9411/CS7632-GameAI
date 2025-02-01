// Remove the line above if you are subitting to GradeScope for a grade. But leave it if you only want to check
// that your code compiles and the autograder can access your public methods.

using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;


namespace GameAICourse
{
    public class AStarPathSearchImpl
    {
        // Please change this string to your name
        public const string StudentAuthorName = "Weixuan Xu";


        // Null Heuristic for Dijkstra
        public static float HeuristicNull(Vector2 nodeA, Vector2 nodeB)
        {
            return 0f;
        }

        // Null Cost for Greedy Best First
        public static float CostNull(Vector2 nodeA, Vector2 nodeB)
        {
            return 0f;
        }


        // Heuristic distance function implemented with manhattan distance
        public static float HeuristicManhattan(Vector2 nodeA, Vector2 nodeB)
        {
            //STUDENT CODE HERE

            // The following code is just a placeholder so that the method has a valid return
            // You will replace it with the correct implementation
            return Mathf.Abs(nodeA.x - nodeB.x) + Mathf.Abs(nodeA.y - nodeB.y);

            //END CODE 
        }

        // Heuristic distance function implemented with Euclidean distance
        public static float HeuristicEuclidean(Vector2 nodeA, Vector2 nodeB)
        {
            //STUDENT CODE HERE

            // The following code is just a placeholder so that the method has a valid return
            // You will replace it with the correct implementation
            return Mathf.Sqrt(Mathf.Pow(nodeA.x - nodeB.x, 2) + Mathf.Pow(nodeA.y - nodeB.y, 2));

            //END CODE 
        }


        // Cost is only ever called on adjacent nodes. So we will always use Euclidean distance.
        // We could use Manhattan dist for 4-way connected grids and avoid sqrroot and mults.
        // But we will avoid that for simplicity.
        public static float Cost(Vector2 nodeA, Vector2 nodeB)
        {
            //STUDENT CODE HERE

            // The following code is just a placeholder so that the method has a valid return
            // You will replace it with the correct implementation
            return HeuristicEuclidean(nodeA, nodeB);

            //END STUDENT CODE
        }


        public static PathSearchResultType FindPathIncremental(
            GetNodeCount getNodeCount,
            GetNode getNode,
            GetNodeAdjacencies getAdjacencies,
            CostCallback G,
            CostCallback H,
            int startNodeIndex, int goalNodeIndex,
            int maxNumNodesToExplore, bool doInitialization,
            ref int currentNodeIndex,
            ref Dictionary<int, PathSearchNodeRecord> searchNodeRecords,
            ref SimplePriorityQueue<int, float> openNodes, ref HashSet<int> closedNodes, ref List<int> returnPath)
        {
            PathSearchResultType pathResult = PathSearchResultType.InProgress;

            var nodeCount = getNodeCount();

            if (startNodeIndex >= nodeCount || goalNodeIndex >= nodeCount ||
                startNodeIndex < 0 || goalNodeIndex < 0 ||
                maxNumNodesToExplore <= 0 ||
                (!doInitialization &&
                 (openNodes == null || closedNodes == null || currentNodeIndex < 0 ||
                  currentNodeIndex >= nodeCount)))

                return PathSearchResultType.InitializationError;


            // STUDENT CODE HERE

            // The following code is just a placeholder so that the method has a valid return
            // You will replace it with the correct implementation
            if (doInitialization)
            {
                currentNodeIndex = startNodeIndex;
                searchNodeRecords = new Dictionary<int, PathSearchNodeRecord>();
                openNodes = new SimplePriorityQueue<int, float>();
                closedNodes = new HashSet<int>();
                var firstNodeRecord = new PathSearchNodeRecord(currentNodeIndex);
                searchNodeRecords.Add(firstNodeRecord.NodeIndex, firstNodeRecord);
                openNodes.Enqueue(firstNodeRecord.NodeIndex, 0f);
                returnPath = new List<int>();
            }

            int nodesProcessed = 0;
            while (nodesProcessed < maxNumNodesToExplore && openNodes.Count > 0)
            {
                PathSearchNodeRecord currentNode = searchNodeRecords[openNodes.First];
                currentNodeIndex = currentNode.NodeIndex;

                ++nodesProcessed;

                if (currentNodeIndex == goalNodeIndex) break;

                PathSearchNodeRecord endNodeRecord;

                List<int> adjacencyNodeIndexes = getAdjacencies(currentNodeIndex);
                foreach (int endNodeIndex in adjacencyNodeIndexes)
                {
                    float endNodeCost = currentNode.CostSoFar +
                                        G(getNode(currentNodeIndex), getNode(endNodeIndex));
                    float endNodeHeuristic;

                    if (closedNodes.Contains(endNodeIndex))
                    {
                        endNodeRecord = searchNodeRecords[endNodeIndex];
                        if (endNodeRecord.CostSoFar <= endNodeCost) continue;
                        closedNodes.Remove(endNodeIndex);
                        endNodeHeuristic = endNodeRecord.EstimatedTotalCost - endNodeRecord.CostSoFar;
                    }
                    else if (openNodes.Contains(endNodeIndex))
                    {
                        endNodeRecord = searchNodeRecords[endNodeIndex];
                        if (endNodeRecord.CostSoFar <= endNodeCost) continue;
                        endNodeHeuristic = endNodeRecord.EstimatedTotalCost - endNodeRecord.CostSoFar;
                    }
                    else
                    {
                        endNodeRecord = new PathSearchNodeRecord(endNodeIndex);
                        endNodeHeuristic = H(getNode(endNodeIndex), getNode(goalNodeIndex));
                    }

                    endNodeRecord.CostSoFar = endNodeCost;
                    endNodeRecord.FromNodeIndex = currentNodeIndex;
                    endNodeRecord.EstimatedTotalCost = endNodeCost + endNodeHeuristic;
                    searchNodeRecords[endNodeIndex] = endNodeRecord;

                    if (!openNodes.Contains(endNodeIndex))
                    {
                        openNodes.Enqueue(endNodeIndex, endNodeRecord.EstimatedTotalCost);
                    }
                    else
                    {
                        openNodes.UpdatePriority(endNodeIndex, endNodeRecord.EstimatedTotalCost);
                    }
                }

                openNodes.Remove(currentNodeIndex);
                closedNodes.Add(currentNodeIndex);
            }

            if (openNodes.Count <= 0 && currentNodeIndex != goalNodeIndex)
            {
                pathResult = PathSearchResultType.Partial;
                //find the closest node we looked at and use for partial path
                int closest = -1;
                float closestDist = float.MaxValue;
                foreach (var n in closedNodes)
                {
                    var nrec = searchNodeRecords[n];
                    var d = Cost(getNode(nrec.NodeIndex), getNode(goalNodeIndex));
                    if (d < closestDist)
                    {
                        closest = n;
                        closestDist = d;
                    }
                }

                if (closest >= 0)
                {
                    currentNodeIndex = closest;
                }
            }
            else if (currentNodeIndex == goalNodeIndex)
            {
                pathResult = PathSearchResultType.Complete;
            }


            if (pathResult != PathSearchResultType.InProgress)
            {
                // processing complete, a path (possibly partial) can be generated returned
                returnPath = new List<int>();
                while (currentNodeIndex != startNodeIndex)
                {
                    returnPath.Add(currentNodeIndex);
                    currentNodeIndex = searchNodeRecords[currentNodeIndex].FromNodeIndex;
                }

                returnPath.Add(startNodeIndex);
                returnPath.Reverse();
            }

            return pathResult;
        }
        //END STUDENT CODE HERE
    }
}