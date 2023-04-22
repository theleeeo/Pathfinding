using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    empty,
    wall,
    solid
}

public class GridNode : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [HideInInspector] public NodeType nodeType = NodeType.empty;

    public bool isVisited;
    public Vector2Int parentPosition = new Vector2Int(-1, -1);

    private bool isChanged;
    private bool isWaypoint;    

    public void SetToEmpty()
    {
        nodeType = NodeType.empty;
        spriteRenderer.sprite = ResourceHandler._instance.empty_sprite;

        isWaypoint = false;
    }

    public void SetToWall()
    {
        nodeType = NodeType.wall;
        spriteRenderer.sprite = ResourceHandler._instance.wall_sprite;
    }

    public void SetToSolid()
    {
        nodeType = NodeType.solid;
        spriteRenderer.sprite = ResourceHandler._instance.wall_sprite;
    }

    public void ShowAsPath()
    {
        spriteRenderer.sprite = ResourceHandler._instance.blue_sprite;
    }

    public void ShowAsStart()
    {
        spriteRenderer.sprite = ResourceHandler._instance.start_Sprite;

        isWaypoint = true;
    }

    public void ShowAsEnd()
    {
        spriteRenderer.sprite = ResourceHandler._instance.end_Sprite;

        isWaypoint = true;
    }

    public void ShowAsTemp()
    {
        if (spriteRenderer.sprite != ResourceHandler._instance.empty_sprite)
        {
            return;
        }
        
        spriteRenderer.sprite = ResourceHandler._instance.grey_sprite;
    }

    public void ResetState()
    {
        isVisited = false;
        parentPosition = new Vector2Int(-1, -1);
        isChanged = false;

        if (nodeType == NodeType.empty && spriteRenderer.sprite != ResourceHandler._instance.empty_sprite && false == isWaypoint)
        {
            spriteRenderer.sprite = ResourceHandler._instance.empty_sprite;
        }
    }

    public void MarkAsChanged()
    {
        if (isChanged)
        {
            return;
        }

        isChanged = true;

        GridController.alteredNodes.Add(this);
    }
}
