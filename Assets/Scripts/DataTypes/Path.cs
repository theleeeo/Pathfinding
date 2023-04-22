using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public List<Vector2Int> path = new();
    //private int currentIndex = 0;

    public void AddToPath(Vector2Int nodePos)
    {
        path.Add(nodePos);
    }

    public int Count()
    {
        return path.Count;
    }

    /*public void ResetPosition()
    {
        currentIndex = 0;
    }

    public Vector2Int GetNextPosition()
    {
        return path[currentIndex++];
    }*/
}
