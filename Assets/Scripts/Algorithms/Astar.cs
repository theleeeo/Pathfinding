using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Astar : AlgorithmBase
{
    public override void GeneratePath(Vector2Int startNodePos, Vector2Int endNodePos)
    {
        this.endNodePos = endNodePos;

        if (AlgorithmController.ShowGeneration)
        {
            StartCoroutine(AsyncGeneration(startNodePos, endNodePos));
        }
        else if (AlgorithmController.Benchmark)
        {
            Benchmark(startNodePos, endNodePos);
        }
        else
        {
            SyncGeneration(startNodePos, endNodePos);
        }
    }

    public bool UseManhattanDist;

    private Vector2Int endNodePos;
    private int[,] distancesFromStart;

    private int GetDistanceToGoal(Vector2Int nodePos) //manhattan distance
    {
        return Mathf.Abs(endNodePos.x - nodePos.x) + Mathf.Abs(endNodePos.y - nodePos.y);
    }

    private void SyncGeneration(Vector2Int startNodePos, Vector2Int endNodePos)
    {
        distancesFromStart = new int[GridController.Size_X, GridController.Size_Y];
        MinNodeHeap nodeHeap = new();

        nodeHeap.Insert(new HeuristicPosPair(startNodePos, 0 + GetDistanceToGoal(startNodePos)));

        GridNode startNode = GridController.GetNodeAtNodePos(startNodePos);
        startNode.isVisited = true;
        startNode.MarkAsChanged();

        while (nodeHeap.PeekMin().heuristic != int.MaxValue)
        {
            HeuristicPosPair current = nodeHeap.ExtractMin();

            if (current.pos == endNodePos) //goal found
            {
                GeneratePathFromGoal(endNodePos);
                return;
            }

            int currentDistFromStart = distancesFromStart[current.pos.x, current.pos.y];

            foreach (Vector2Int offset in childOffsets)
            {
                Vector2Int childPos = current.pos + offset;
                HeuristicPosPair child = new(childPos, currentDistFromStart + 1 + GetDistanceToGoal(childPos));
                GridNode childNode = GridController.GetNodeAtNodePos(child.pos);

                if (childNode.nodeType != NodeType.empty)
                {
                    continue;
                }

                if (childNode.isVisited)
                {
                    if (currentDistFromStart + 1 < distancesFromStart[childPos.x, childPos.y]) //new way is closer, dont need to add distToGoal since it cancels out with the heuristic
                    {
                        childNode.parentPosition = current.pos;
                        nodeHeap.Insert(child);
                        distancesFromStart[childPos.x, childPos.y] = currentDistFromStart + 1;
                    }
                }
                else
                {
                    childNode.isVisited = true;
                    childNode.parentPosition = current.pos;

                    nodeHeap.Insert(child);
                    distancesFromStart[childPos.x, childPos.y] = currentDistFromStart + 1;

                    childNode.MarkAsChanged();
                }
            }
        }


        Debug.LogError("No path found");

        AlgorithmController.AlgorithmIsRunning = false;
    }

    private IEnumerator AsyncGeneration(Vector2Int startNodePos, Vector2Int endNodePos)
    {
        distancesFromStart = new int[GridController.Size_X, GridController.Size_Y];
        MinNodeHeap nodeHeap = new();

        nodeHeap.Insert(new HeuristicPosPair(startNodePos, 0 + GetDistanceToGoal(startNodePos)));

        GridNode startNode = GridController.GetNodeAtNodePos(startNodePos);
        startNode.isVisited = true;
        startNode.MarkAsChanged();

        while (nodeHeap.PeekMin().heuristic != int.MaxValue)
        {            
            HeuristicPosPair current = nodeHeap.ExtractMin();            

            if (current.pos == endNodePos) //goal found
            {
                GeneratePathFromGoal(endNodePos);
                yield break;
            }

            int currentDistFromStart = distancesFromStart[current.pos.x, current.pos.y];

            GridController.GetNodeAtNodePos(current.pos).ShowAsTemp();

            foreach (Vector2Int offset in childOffsets)
            {
                Vector2Int childPos = current.pos + offset;
                HeuristicPosPair child = new(childPos, currentDistFromStart + 1 + GetDistanceToGoal(childPos));
                GridNode childNode = GridController.GetNodeAtNodePos(child.pos);

                if(childNode.nodeType != NodeType.empty)
                {
                    continue;
                }

                if (childNode.isVisited)
                {
                    if(currentDistFromStart + 1 < distancesFromStart[childPos.x, childPos.y]) //new way is closer, dont need to add distToGoal since it cancels out with the heuristic
                    {
                        Debug.Log("hmeme");
                        childNode.parentPosition = current.pos;
                        nodeHeap.Insert(child);
                        distancesFromStart[childPos.x, childPos.y] = currentDistFromStart + 1;
                    }
                }
                else
                {
                    childNode.isVisited = true;
                    childNode.parentPosition = current.pos;                    

                    nodeHeap.Insert(child);
                    distancesFromStart[childPos.x, childPos.y] = currentDistFromStart + 1;

                    childNode.MarkAsChanged();
                }                
            }

            yield return new WaitForSeconds(0.1f);
        }


        Debug.LogError("No path found");

        AlgorithmController.AlgorithmIsRunning = false;
    }

    private void Benchmark(Vector2Int startNodePos, Vector2Int endNodePos)
    {       
        stopwatch.Reset();

        for (int i = 0; i < AlgorithmController.benchmarkIterations; i++)
        {
            NodesSearched = 0;
            GridController.ResetGrid();

            stopwatch.Start();
            //------------- 
            distancesFromStart = new int[GridController.Size_X, GridController.Size_Y];
            MinNodeHeap nodeHeap = new();            

            nodeHeap.Insert(new HeuristicPosPair(startNodePos, 0 + GetDistanceToGoal(startNodePos)));

            GridNode startNode = GridController.GetNodeAtNodePos(startNodePos);
            startNode.isVisited = true;
            startNode.MarkAsChanged();

            while (nodeHeap.PeekMin().heuristic != int.MaxValue)
            {
                HeuristicPosPair current = nodeHeap.ExtractMin();

                if (current.pos == endNodePos) //goal found
                {
                    break;
                }

                NodesSearched++;

                int currentDistFromStart = distancesFromStart[current.pos.x, current.pos.y];

                foreach (Vector2Int offset in childOffsets)
                {
                    Vector2Int childPos = current.pos + offset;
                    HeuristicPosPair child = new(childPos, currentDistFromStart + 1 + GetDistanceToGoal(childPos));
                    GridNode childNode = GridController.GetNodeAtNodePos(child.pos);

                    if (childNode.nodeType != NodeType.empty)
                    {
                        continue;
                    }

                    if (childNode.isVisited)
                    {
                        if (currentDistFromStart + 1 < distancesFromStart[childPos.x, childPos.y]) //new way is closer, dont need to add distToGoal since it cancels out with the heuristic
                        {
                            childNode.parentPosition = current.pos;
                            nodeHeap.Insert(child);
                            distancesFromStart[childPos.x, childPos.y] = currentDistFromStart + 1;
                        }
                    }
                    else
                    {
                        childNode.isVisited = true;
                        childNode.parentPosition = current.pos;

                        nodeHeap.Insert(child);
                        distancesFromStart[childPos.x, childPos.y] = currentDistFromStart + 1;

                        childNode.MarkAsChanged();
                    }
                }                          
            }

            //-------------

            stopwatch.Stop();            
        }

        Path path = GeneratePathFromGoal(endNodePos);

        AlgorithmController.DisplayBenchmark(stopwatch.ElapsedMilliseconds, NodesSearched, path.Count());

        AlgorithmController.AlgorithmIsRunning = false;
    }
}
