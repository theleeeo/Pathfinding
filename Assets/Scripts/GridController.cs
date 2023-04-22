using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridController : MonoBehaviour
{
    public static GridController _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError($"Multiple instances of type {this}");
            return;
        }

        _instance = this;
    }

    public const int MIN_SIZE = 3;
    public const int MAX_SIZE = 50;

    public GridNode[,] grid { get; private set; }

    public static int Size_X { get; private set; }
    public static int Size_Y { get; private set; }

    public static List<GridNode> alteredNodes = new();

    public static Vector2Int WorldToNodePos(Vector2 pos)
    {
        if (pos.x < -0.5f || pos.x > Size_X - 0.5f || pos.y < -0.5f || pos.y > Size_Y - 0.5f)
        {
            return new Vector2Int(-1, -1);
        }

        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    /// <summary>
    /// Unsafe
    /// </summary>
    public static GridNode GetNodeAtNodePos(Vector2Int nodePos)
    {
        return _instance.grid[nodePos.x, nodePos.y];
    }

    public static GridNode GetNodeAtWorldPos(Vector2 pos)
    {
        Vector2Int nodePos = WorldToNodePos(pos);

        if (-1 == nodePos.x)
        {
            return null;
        }

        return _instance.grid[nodePos.x, nodePos.y];
    }

    public static void LoadGridFromTexture(Texture2D gridTexture)
    {
        InstantiateGrid(gridTexture.width, gridTexture.height);

        for (int x = 0; x < Size_X - 2; x++)
        {
            for (int y = 0; y < Size_Y - 2; y++)
            {
                if(Color.black == gridTexture.GetPixel(x, y))
                {
                    _instance.grid[x + 1, y + 1].SetToWall();
                }
            }
        }
    }

    public static void InstantiateGrid(int sizeX, int sizeY)
    {
        for (int x = 0; x < Size_X; x++)
        {
            for (int y = 0; y < Size_Y; y++)
            {
                Destroy(_instance.grid[x, y].gameObject);
            }
        }

        Size_X = Mathf.Clamp(sizeX, MIN_SIZE, MAX_SIZE) + 2;
        Size_Y = Mathf.Clamp(sizeY, MIN_SIZE, MAX_SIZE) + 2;

        _instance.grid = new GridNode[Size_X , Size_Y];

        for (int x = 0; x < Size_X; x++)
        {
            for (int y = 0; y < Size_Y; y++)
            {
                _instance.grid[x, y] = Instantiate(ResourceHandler._instance.NodeObject, new Vector3(x, y), Quaternion.identity, _instance.transform).GetComponent<GridNode>();
            }
        }

        alteredNodes.Clear();

        CameraController.CenterCameraPosition();

        WaypointController.ClearWaypoints();
        AlgorithmController.ResetLineRenderer();

        SetSolidBoundary();
    }

    private static void SetSolidBoundary()
    {
        for(int x = 0; x < Size_X; x++)
        {
            _instance.grid[x, 0].SetToSolid();
            _instance.grid[x, Size_Y - 1].SetToSolid();
        }

        for (int y = 1; y < Size_Y - 1; y++)
        {
            _instance.grid[0, y].SetToSolid();
            _instance.grid[Size_X - 1, y].SetToSolid();
        }
    }

    public static void ClearGrid()
    {
        for (int x = 1; x < Size_X - 1; x++)
        {
            for (int y = 1; y < Size_Y - 1; y++)
            {
                _instance.grid[x, y].SetToEmpty();
                _instance.grid[x, y].ResetState();
            }
        }

        ResetGrid();
        WaypointController.ClearWaypoints();
    }

    public static void ResetGrid()
    {
        foreach (GridNode node in alteredNodes)
        {
            node.ResetState();
        }

        alteredNodes.Clear();

        AlgorithmController.ResetLineRenderer();
    }

    private void Start()
    {
        DOTween.Init();

        InstantiateGrid(5, 5);
        WaypointController.AddWaypoint(new Vector2Int(2, 2));
        WaypointController.AddWaypoint(new Vector2Int(4, 4));
    }
}
