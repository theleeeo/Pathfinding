using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

public abstract class AlgorithmBase : MonoBehaviour
{
    protected static readonly Vector2Int[] childOffsets = { new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(0, 1) };

    protected Stopwatch stopwatch = new();
    protected int NodesSearched;

    public abstract void GeneratePath(Vector2Int startNodePos, Vector2Int endNodePos);

    protected static Path GeneratePathFromGoal(Vector2Int endPos)
    {
        Path path = new Path();

        GridNode currentNode = GridController.GetNodeAtNodePos(endPos);
        Vector2Int currentPos = currentNode.parentPosition;
        currentNode = GridController.GetNodeAtNodePos(currentPos);

        Vector2Int noParent = new Vector2Int(-1, -1);
        while (currentNode.parentPosition != noParent)
        {
            path.AddToPath(currentPos);

            currentPos = currentNode.parentPosition;
            currentNode = GridController.GetNodeAtNodePos(currentPos);
        }

        AlgorithmController.ShowPath(path);

        return path;
    }
}
