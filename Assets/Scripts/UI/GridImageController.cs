using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridImageController : MonoBehaviour, IPointerClickHandler
{
    public RawImage image;

    public int index;

    public bool isSelected = false;

    public string fileName;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected)
        {
            SaveUIController._instance.DeselectGrid();
        }
        else
        {
            SaveUIController._instance.SelectGrid(index);
        }

        isSelected = !isSelected;
    }

    public void Delete()
    {
        SaveUIController._instance.DeleteGrid(index);
    }    

    public void Select()
    {
        if (isSelected)
        {
            Deselect();
        }
        else
        {
            SaveUIController._instance.SelectGrid(index);
        }
    }

    private void Deselect()
    {
        SaveUIController._instance.DeselectGrid();
    }
}
