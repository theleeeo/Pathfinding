using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController _instance;

    private void Awake()
    {
        camera = Camera.main;

        if (_instance != null)
        {
            Debug.LogError($"Multiple instances of type {this}");
            return;
        }

        _instance = this;
    }

    public static new Camera camera;

    public static bool canMove = true;

    public static void CenterCameraPosition()
    {
        camera.transform.position = new Vector3(GridController.Size_X / 2f - 0.5f, GridController.Size_Y / 2f - 0.5f, -1);

        if (GridController.Size_Y * camera.aspect < GridController.Size_X) //grid is too wide
        {
            camera.orthographicSize = GridController.Size_X / camera.aspect / 2 + 1;
        }
        else
        {
            camera.orthographicSize = GridController.Size_Y / 2 + 1;
        }
    }

    private void Update()
    {
        if (canMove)
        {
            camera.orthographicSize += Input.mouseScrollDelta.y * Time.deltaTime * 10;

            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, GridController.MIN_SIZE / 2, GridController.MAX_SIZE / 2);

            transform.position += 5 * new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * camera.orthographicSize / 10;
        }        
    }
}
