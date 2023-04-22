using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstras : AlgorithmBase
{
    public override void GeneratePath(Vector2Int startNodePos, Vector2Int endNodePos)
    {
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

    private void SyncGeneration(Vector2Int startNodePos, Vector2Int endNodePos)
    {
        MinNodeHeap nodeHeap = new();

        nodeHeap.Insert(new HeuristicPosPair(startNodePos, 0));

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

            foreach (Vector2Int offset in childOffsets)
            {
                HeuristicPosPair child = new(current.pos + offset, current.heuristic + 1);
                GridNode childNode = GridController.GetNodeAtNodePos(child.pos);

                if (childNode.isVisited || childNode.nodeType != NodeType.empty)
                {
                    continue;
                }

                childNode.isVisited = true;
                childNode.parentPosition = current.pos;

                childNode.MarkAsChanged();

                nodeHeap.Insert(child);
            }
        }


        Debug.LogError("No path found");

        AlgorithmController.AlgorithmIsRunning = false;
    }

    private IEnumerator AsyncGeneration(Vector2Int startNodePos, Vector2Int endNodePos)
    {
        MinNodeHeap nodeHeap = new();

        nodeHeap.Insert(new HeuristicPosPair(startNodePos, 0));

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

            GridController.GetNodeAtNodePos(current.pos).ShowAsTemp();

            foreach (Vector2Int offset in childOffsets)
            {
                HeuristicPosPair child = new(current.pos + offset, current.heuristic + 1);                
                GridNode childNode = GridController.GetNodeAtNodePos(child.pos);

                if (childNode.isVisited || childNode.nodeType != NodeType.empty)
                {
                    continue;
                }

                childNode.isVisited = true;
                childNode.parentPosition = current.pos;

                childNode.MarkAsChanged();

                nodeHeap.Insert(child);
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
            MinNodeHeap nodeHeap = new();

            nodeHeap.Insert(new HeuristicPosPair(startNodePos, 0));

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

                foreach (Vector2Int offset in childOffsets)
                {
                    HeuristicPosPair child = new(current.pos + offset, current.heuristic + 1);
                    GridNode childNode = GridController.GetNodeAtNodePos(child.pos);

                    if (childNode.isVisited || childNode.nodeType != NodeType.empty)
                    {
                        continue;
                    }

                    childNode.isVisited = true;
                    childNode.parentPosition = current.pos;

                    childNode.MarkAsChanged();

                    nodeHeap.Insert(child);
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
