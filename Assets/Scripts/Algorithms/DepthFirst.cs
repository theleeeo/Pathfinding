using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFirst : AlgorithmBase
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
        Stack<Vector2Int> nodeStack = new();
        nodeStack.Push(startNodePos);

        GridNode startNode = GridController.GetNodeAtNodePos(startNodePos);
        startNode.MarkAsChanged();

        while (nodeStack.Count != 0)
        {
            Vector2Int currentPos = nodeStack.Pop();

            if (currentPos == endNodePos) //goal found
            {
                GeneratePathFromGoal(endNodePos);
                return;
            }

            GridNode currentNode = GridController.GetNodeAtNodePos(currentPos);

            if (currentNode.isVisited)
            {
                continue;
            }

            currentNode.isVisited = true;

            foreach (Vector2Int offset in childOffsets)
            {
                Vector2Int childPos = currentPos + offset;
                GridNode childNode = GridController.GetNodeAtNodePos(childPos);

                if (childNode.isVisited || childNode.nodeType != NodeType.empty)
                {
                    continue;
                }

                childNode.parentPosition = currentPos;

                childNode.MarkAsChanged();

                nodeStack.Push(childPos);
            }
        }

        Debug.LogError("No path found");

        AlgorithmController.AlgorithmIsRunning = false;
    }

    private IEnumerator AsyncGeneration(Vector2Int startNodePos, Vector2Int endNodePos)
    {
        Stack<Vector2Int> nodeStack = new();
        nodeStack.Push(startNodePos);

        GridNode startNode = GridController.GetNodeAtNodePos(startNodePos);
        startNode.MarkAsChanged();

        while (nodeStack.Count != 0)
        {
            Vector2Int currentPos = nodeStack.Pop();

            if (currentPos == endNodePos) //goal found
            {
                GeneratePathFromGoal(endNodePos);
                yield break;
            }

            GridNode currentNode = GridController.GetNodeAtNodePos(currentPos);

            if (currentNode.isVisited)
            {
                continue;
            }
            
            currentNode.isVisited = true;

            currentNode.ShowAsTemp();                       

            foreach (Vector2Int offset in childOffsets)
            {
                Vector2Int childPos = currentPos + offset;
                GridNode childNode = GridController.GetNodeAtNodePos(childPos);

                if (childNode.isVisited || childNode.nodeType != NodeType.empty)
                {
                    continue;
                }
                
                childNode.parentPosition = currentPos;

                childNode.MarkAsChanged();

                nodeStack.Push(childPos);
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
            Stack<Vector2Int> nodeStack = new();
            nodeStack.Push(startNodePos);

            GridNode startNode = GridController.GetNodeAtNodePos(startNodePos);
            startNode.MarkAsChanged();

            while (nodeStack.Count != 0)
            {
                Vector2Int currentPos = nodeStack.Pop();

                if (currentPos == endNodePos) //goal found
                {
                    GeneratePathFromGoal(endNodePos);
                    break;
                }

                GridNode currentNode = GridController.GetNodeAtNodePos(currentPos);

                if (currentNode.isVisited)
                {
                    continue;
                }

                currentNode.isVisited = true;

                NodesSearched++;

                foreach (Vector2Int offset in childOffsets)
                {
                    Vector2Int childPos = currentPos + offset;
                    GridNode childNode = GridController.GetNodeAtNodePos(childPos);

                    if (childNode.isVisited || childNode.nodeType != NodeType.empty)
                    {
                        continue;
                    }

                    childNode.parentPosition = currentPos;

                    childNode.MarkAsChanged();

                    nodeStack.Push(childPos);
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
