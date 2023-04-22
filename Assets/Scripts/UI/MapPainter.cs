using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPainter : MonoBehaviour
{
    public static MapPainter _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError($"Multiple instances of type {this}");
            return;
        }

        _instance = this;
    }

    private bool canPaint = false;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.H))
        {
            Ray ray = CameraController.camera.ScreenPointToRay(Input.mousePosition);

            GridNode node = GridController.GetNodeAtWorldPos(ray.origin);

            Debug.Log($"NodeType: {node.nodeType}\nVisited: {node.isVisited}");
        }
#endif

        if (canPaint && false == AlgorithmController.AlgorithmIsRunning)
        {
            if (Input.GetButton("Fire1")) //Left mouse
            {
                Ray ray = CameraController.camera.ScreenPointToRay(Input.mousePosition);

                GridNode node = GridController.GetNodeAtWorldPos(ray.origin);

                if (node != null && node.nodeType != NodeType.solid)
                {
                    node.SetToWall();
                    WaypointController.RemoveWaypoint(GridController.WorldToNodePos(ray.origin));
                }
            }
            else if (Input.GetButton("Fire2")) //Right mouse
            {
                Ray ray = CameraController.camera.ScreenPointToRay(Input.mousePosition);

                GridNode node = GridController.GetNodeAtWorldPos(ray.origin);

                if (node != null && node.nodeType != NodeType.solid)
                {
                    node.SetToEmpty();
                    WaypointController.RemoveWaypoint(GridController.WorldToNodePos(ray.origin));
                }
            }
        }        
    }

    public void SetPaintMode(bool value)
    {
        canPaint = value;

        if (value)
        {
            GridController.ResetGrid();
        }

        UIController._instance.ActivateClearButton(value);
    }
}
