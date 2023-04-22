using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    public static ResourceHandler _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError($"Multiple instances of type {this}");
            return;
        }

        _instance = this;
    }

    public Sprite empty_sprite;
    public Sprite wall_sprite;
    public Sprite blue_sprite;
    public Sprite grey_sprite;

    public Sprite start_Sprite;
    public Sprite end_Sprite;

    public GameObject NodeObject;
}
