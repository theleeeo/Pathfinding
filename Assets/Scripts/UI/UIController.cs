using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError($"Multiple instances of type {this}");
            return;
        }

        _instance = this;
    }

    public RectTransform CB; //clear button
    private const float CB_OFFSET = -12;

    [SerializeField] private RectTransform PM; //paint mode
    private const float PM_OFFSET = -12;

    [SerializeField] private RectTransform AP; //algorithm picker
    private const float AP_UNACTIVE_POS = -86;
    private const float AP_OFFSET = 70;
    private bool AP_active;

    [SerializeField] private RectTransform AM; //algorithm manager
    private const float AM_OFFSET = 16;

    [SerializeField] private RectTransform SP; //size picker
    private const float SP_UNACTIVE_POS = 48;
    private const float SP_OFFSET = -32;
    private bool SP_active;

    [SerializeField] private RectTransform ST; //settings
    private const float ST_OFFSET = -16;

    [SerializeField] private RectTransform BM; //benchmark
    private const float BM_OFFSET = -48;
    private bool BM_active;


    [SerializeField] private Toggle SizePickerToggle;
    [SerializeField] private Toggle AlgorithmPickerToggle;

    public TextMeshProUGUI Benchmark_Time, Benchmark_Nodes, Benchmark_PathLength;    

    private void Start()
    {
        SizePickerToggle.isOn = true;
        AlgorithmPickerToggle.isOn = true;
    }

    public void OpenAlgoritmPicker(bool value)
    {
        AP_active = value;
        AP.DOMoveRectY(AP_OFFSET * (value ? 1 : -1));
    }

    public void OpenSizePicker(bool value)
    {
        SP_active = value;
        SP.DOMoveRectX(SP_OFFSET * (value ? 1 : -1));
    }

    public void OpenBenchmarkUI(bool value)
    {
        BM_active = value;
        BM.DOMoveRectX(BM_OFFSET * (value ? 1 : -1));
    }


    public void ActivateClearButton(bool value)
    {
        CB.DOMoveRectY(CB_OFFSET * (value ? 1 : -1));
    }  

    public void ActivateAlgorithmPicker(bool value)
    {
        if (value)
        {
            AP.DOMoveRectY(AP_OFFSET * (AP_active ? 1 : -1));
        }
        else
        {
            AP.DOMoveRectY(AP_UNACTIVE_POS);
        }        
    }

    public void ActivateSizePicker(bool value)
    {
        if (value)
        {
            SP.DOMoveRectX(SP_OFFSET * (SP_active ? 1 : -1));
        }
        else
        {
            SP.DOMoveRectX(SP_UNACTIVE_POS);
        }
    }

    public void ActivateAlgorithmManager(bool value)
    {
        AM.DOMoveRectY(AM_OFFSET * (value ? 1 : -1));
    }

    public void ActivateSettings(bool value)
    {
        ST.DOMoveRectY(ST_OFFSET * (value ? 1 : -1));
    }

    public void ActivatePaintMode(bool value)
    {
        PM.DOMoveRectY(PM_OFFSET * (value ? 1 : -1));
    }

    public void ActivateAll(bool value)
    {
        ActivateClearButton(value);

        if (BM_active)
        {
            OpenBenchmarkUI(value);
        }
        
        ActivateAlgorithmPicker(value);
        ActivateAlgorithmManager(value);
        ActivateSettings(value);
        ActivatePaintMode(value);
        ActivateSizePicker(value);
    }
}
