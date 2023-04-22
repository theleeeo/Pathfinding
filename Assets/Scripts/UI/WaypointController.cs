using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour
{
    public static WaypointController _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError($"Multiple instances of type {this}");
            return;
        }

        _instance = this;
    }

    private const float HALF_BAR_Y = 16;

    private bool waypointPaintMode = false;

    private Vector2Int startPos = new Vector2Int(-1, -1);
    private Vector2Int endPos = new Vector2Int(-1, -1);
    private int pointsSet = 0;

    public static Vector2Int GetStartPos()
    {
        return _instance.startPos;
    }

    public static Vector2Int GetEndPos()
    {
        return _instance.endPos;
    }

    public void Open(bool value)
    {
        transform.DOMoveRectY(HALF_BAR_Y * (value ? 1 : -1));
    }

    public void SetWaypointPaintMode(bool value)
    {
        waypointPaintMode = value;

        if (value)
        {
            GridController.ResetGrid();
        }
    }

    private void Update()
    {
        if(false == waypointPaintMode || AlgorithmController.AlgorithmIsRunning)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1")) //Left mouse
        {
            Ray ray = CameraController.camera.ScreenPointToRay(Input.mousePosition);            

            Vector2Int nodePosition = GridController.WorldToNodePos(ray.origin);

            if(-1 != nodePosition.x)
            {
                AddWaypoint(nodePosition);
            }        
        }
        else if (Input.GetButtonDown("Fire2")) //Right mouse
        {
            Ray ray = CameraController.camera.ScreenPointToRay(Input.mousePosition);           

            Vector2Int nodePosition = GridController.WorldToNodePos(ray.origin);

            if (-1 != nodePosition.x)
            {
                RemoveWaypoint(nodePosition);
            }
        }
    }

    public static void AddWaypoint(Vector2Int nodePos)
    {
        GridNode node = GridController.GetNodeAtNodePos(nodePos);

        if (node == null || node.nodeType == NodeType.solid || _instance.pointsSet == 2 || nodePos == _instance.startPos) //no check for end pos needed, logically false
        {
            return;
        }

        if (0 == _instance.pointsSet)
        {
            _instance.startPos = nodePos;
            node.ShowAsStart();
        } else
        {
            _instance.endPos = nodePos;
            node.ShowAsEnd();
        }      

        _instance.pointsSet++;
    }

    public static void RemoveWaypoint(Vector2Int nodePos)
    {
        GridNode node = GridController.GetNodeAtNodePos(nodePos);

        if (node == null || node.nodeType == NodeType.solid || (nodePos != _instance.startPos && nodePos != _instance.endPos))
        {
            return;
        }

        if(nodePos == _instance.endPos) //remove end
        {
            _instance.endPos = new Vector2Int(-1, -1);           
        }
        else //remove start
        {
            if (2 == _instance.pointsSet) //end exists
            {
                GridController.GetNodeAtNodePos(_instance.endPos).ShowAsStart();

                _instance.startPos = _instance.endPos;

                _instance.endPos = new Vector2Int(-1, -1);
            }
            else
            {
                _instance.startPos = new Vector2Int(-1, -1);
            }
        }

        node.SetToEmpty();
        _instance.pointsSet--;
    }

    public static void ClearWaypoints() //removes waypoint locally, setting to empty must be done from caller
    {
        _instance.endPos = new Vector2Int(-1, -1);
        _instance.startPos = new Vector2Int(-1, -1);

        _instance.pointsSet = 0;
    }
}
