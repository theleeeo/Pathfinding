using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirst : AlgorithmBase
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
        Queue<Vector2Int> nodeQueue = new Queue<Vector2Int>();

        nodeQueue.Enqueue(startNodePos);

        GridNode startNode = GridController.GetNodeAtNodePos(startNodePos);
        startNode.isVisited = true;
        startNode.MarkAsChanged();

        while (nodeQueue.Count != 0)
        {
            Vector2Int pos = nodeQueue.Dequeue();

            if (pos == endNodePos) //goal found
            {
                GeneratePathFromGoal(endNodePos);
                return;
            }

            foreach (Vector2Int offset in childOffsets)
            {
                Vector2Int childPos = pos + offset;
                GridNode childNode = GridController.GetNodeAtNodePos(childPos);

                if (childNode.isVisited || childNode.nodeType != NodeType.empty)
                {
                    continue;
                }

                childNode.parentPosition = pos;

                childNode.isVisited = true;
                childNode.MarkAsChanged();

                nodeQueue.Enqueue(childPos);
            }
        }

        Debug.LogError("No path found");

        AlgorithmController.AlgorithmIsRunning = false;
    }

    private IEnumerator AsyncGeneration(Vector2Int startNodePos, Vector2Int endNodePos)
    {
        Queue<Vector2Int> nodeQueue = new Queue<Vector2Int>();

        nodeQueue.Enqueue(startNodePos);

        GridNode startNode = GridController.GetNodeAtNodePos(startNodePos);
        startNode.isVisited = true;
        startNode.MarkAsChanged();

        while (nodeQueue.Count != 0)
        {
            Vector2Int pos = nodeQueue.Dequeue();

            if (pos == endNodePos) //goal found
            {
                GeneratePathFromGoal(endNodePos);
                yield break;
            }

            GridController.GetNodeAtNodePos(pos).ShowAsTemp();            

            foreach (Vector2Int offset in childOffsets)
            {
                Vector2Int childPos = pos + offset;
                GridNode childNode = GridController.GetNodeAtNodePos(childPos);

                if (childNode.isVisited || childNode.nodeType != NodeType.empty)
                {
                    continue;
                }

                childNode.parentPosition = pos;                                

                childNode.isVisited = true;
                childNode.MarkAsChanged();

                nodeQueue.Enqueue(childPos);
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

            Queue<Vector2Int> nodeQueue = new Queue<Vector2Int>();            

            nodeQueue.Enqueue(startNodePos);

            GridNode startNode = GridController.GetNodeAtNodePos(startNodePos);
            startNode.isVisited = true;
            startNode.MarkAsChanged();

            while (nodeQueue.Count != 0)
            {                
                Vector2Int pos = nodeQueue.Dequeue();

                if (pos == endNodePos) //goal found
                {
                    break;
                }

                NodesSearched++;

                foreach (Vector2Int offset in childOffsets)
                {
                    Vector2Int childPos = pos + offset;
                    GridNode childNode = GridController.GetNodeAtNodePos(childPos);

                    if (childNode.isVisited || childNode.nodeType != NodeType.empty)
                    {
                        continue;
                    }

                    childNode.parentPosition = pos;

                    childNode.isVisited = true;
                    childNode.MarkAsChanged();

                    nodeQueue.Enqueue(childPos);
                }
            }

            stopwatch.Stop();
        }

        Path path = GeneratePathFromGoal(endNodePos);

        AlgorithmController.DisplayBenchmark(stopwatch.ElapsedMilliseconds, NodesSearched, path.Count());

        AlgorithmController.AlgorithmIsRunning = false;
    }
}
