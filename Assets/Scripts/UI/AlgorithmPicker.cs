using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlgorithmPicker : MonoBehaviour
{    
    private int currentAlgorithm = -1;

    [SerializeField] private ToggleGroup paintingGroup;    

    public void Choice(int algorithmID)
    {       
        if(algorithmID == currentAlgorithm)
        {
            return;
        }

        currentAlgorithm = algorithmID;

        if (AlgorithmController.AlgorithmIsRunning == true)
        {
            AlgorithmController.StopAlgorithm();
        }
    }

    public void RunAlgorithm()
    {
        AlgorithmController.RunAlgorithm(currentAlgorithm);
    }
}
