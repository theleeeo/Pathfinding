using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SizePicker : MonoBehaviour
{
    private int chosenX, chosenY;
    private bool valueChanged;

    [SerializeField] private TMP_InputField TextInput_X, TextInput_Y;

    private void Start()
    {
        TextInput_X.onSelect.AddListener((string _) =>
        {
            StartCoroutine(SwitchSelectedOnTab());
        });
    }

    public void FieldSelected(string _)
    {
        SaveUIController.toggleInputEnabled = false;
    }

    private IEnumerator SwitchSelectedOnTab()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TextInput_Y.Select();
                break;
            }

            yield return null;
        }
    }

    public void SetX(string value)
    {
        SaveUIController.toggleInputEnabled = true;        

        if (int.TryParse(value, out chosenX))
        {
            if(chosenX > GridController.MAX_SIZE)
            {
                TextInput_X.text = GridController.MAX_SIZE.ToString();
            }
            else if(chosenX < GridController.MIN_SIZE)
            {
                TextInput_X.text = GridController.MIN_SIZE.ToString();
            }

            valueChanged = true;
        }
        else
        {
            TextInput_X.text = (GridController.Size_X - 2).ToString();
        }        
    }

    public void SetY(string value)
    {
        SaveUIController.toggleInputEnabled = true;

        if (int.TryParse(value, out chosenY))
        {
            if (chosenY >= GridController.MAX_SIZE)
            {
                TextInput_Y.text = GridController.MAX_SIZE.ToString();
            }
            else if (chosenY < GridController.MIN_SIZE)
            {
                TextInput_Y.text = GridController.MIN_SIZE.ToString();
            }

            valueChanged = true;
        }
        else
        {
            TextInput_Y.text = (GridController.Size_Y - 2).ToString();
        }
    }

    public void ApplyNewSize()
    {
        if(false == valueChanged)
        {
            return;
        }

        GridController.InstantiateGrid(chosenX, chosenY);
        valueChanged = false;
    }
}
