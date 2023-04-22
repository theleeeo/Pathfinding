using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HeuristicPosPair
{
    public HeuristicPosPair(Vector2Int _pos, int _heuristic)
    {
        pos = _pos;
        heuristic = _heuristic;
    }

    public Vector2Int pos;
    public int heuristic;

    public override string ToString()
    {
        return $"{pos} {heuristic}";
    }
}
