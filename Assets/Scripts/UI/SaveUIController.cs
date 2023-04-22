using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class SaveUIController : MonoBehaviour
{
    public static SaveUIController _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError($"Multiple instances of type {this}");
            return;
        }

        _instance = this;

        transform = GetComponent<Transform>() as RectTransform;
    }

    private const float ACTIVE_POSITION = 0;
    private const float UNACTIVE_POSITION = -264;
    private const float ACTIVE_SCALE = 1;
    private const float UNACTIVE_SCALE = 0.5f;

    private bool isActive = false;

    [SerializeField] private GameObject GridImageObject;
    private const int GRID_IMAGE_COLUMNS = 4;
    private const float GRID_IMAGE_OFFSETS = 58;
    private const float GRID_IMAGE_FIRST_OFFSET = 31;

    [SerializeField] private GameObject PressedHighlight;

    [SerializeField] private RectTransform SaveUI;

    private new RectTransform transform;

    [SerializeField] private RectTransform LoadField;
    private bool LoadField_Active;
    private const float LOADFIELD_ACTIVE_Y = -39;
    private const float LOADFIELD_UNACTIVE_Y = 23;

    [SerializeField] private RectTransform ConfirmDeleteField;
    private bool ConfirmDelete_Active;
    private int indexToDelete;

    [SerializeField] private UiFade SelectGridTextFieldFade;

    [SerializeField] private TMP_InputField SaveNameInput;

    [SerializeField] private RectTransform ContentTransform;

    public static bool toggleInputEnabled = true;

    private List<Texture2D> cachedGrids = new();

    private List<GridImageController> gridImages = new();
    private int selectedGridIndex = -1;

    private Vector2 screenSize;

    private void ScaleUp()
    {
        SaveUI.DOScale(ACTIVE_SCALE, 0.25f);
    }

    private Vector2 GetCurrentScreenSize()
    {
        return new Vector2(Screen.width, Screen.height);
    }

    private void Update()
    {
        if (false == isActive && screenSize != GetCurrentScreenSize())
        {
            screenSize = GetCurrentScreenSize();
            SetToDeactivePosition();
        }

        if (true == toggleInputEnabled && Input.GetKeyDown(KeyCode.Tab))
        {            
            if (isActive) //deactivate
            {
                Deactivate();
            }
            else //activate
            {
                ActivateUi();
            }            
        }
    }

    private void Deactivate()
    {
        CameraController.canMove = true;

        float leftBorderX = -transform.sizeDelta.x / 2;
        float halfSize = SaveUI.sizeDelta.x / 2 * UNACTIVE_SCALE;

        SaveUI.DOMoveRectX(leftBorderX - halfSize, 0.5f);
        SaveUI.DOScale(UNACTIVE_SCALE, 0.25f);
        CancelInvoke(nameof(ScaleUp));

        LoadField.DOMoveRectY(LOADFIELD_UNACTIVE_Y);

        isActive = false;

        UIController._instance.ActivateAll(true);
    }

    private void ActivateUi()
    {
        CameraController.canMove = false;

        SaveUI.DOMoveRectX(ACTIVE_POSITION, 0.5f);
        Invoke(nameof(ScaleUp), 0.25f);

        isActive = true;

        UIController._instance.ActivateAll(false);
    }

    private void SetToDeactivePosition()
    {
        float leftBorderX = -transform.sizeDelta.x / 2;
        float halfSize = SaveUI.sizeDelta.x / 2 * UNACTIVE_SCALE;
        SaveUI.anchoredPosition = new Vector3(leftBorderX - halfSize, 0);
    }

    private void Start()
    {
        screenSize = GetCurrentScreenSize();

        SetToDeactivePosition();

        LoadSavedGridImages();
    }

    /*public void Save()
    {        
        SaveField.DOMoveRectY(SaveField_Active ? SAVEFIELD_UNACTIVE_Y : SAVEFIELD_ACTIVE_Y);

        SaveField_Active = !SaveField_Active;
    }

    public void ConfirmSave()
    {
        if (string.IsNullOrEmpty(SaveNameInput.text)) //not valid //todo bluurrrräärä why not work?
        {
            SaveNameInput.transform.DOShakeRotation(0.2f);
            //TODO possibly blink red?

            return;
        }

        var gridTexture = SaveSystem.SaveGrid(GridController._instance.grid, out string fileName);

        AddGrid(gridTexture, fileName);

        SaveNameInput.text = "";
        SaveField.DOMoveRectY(SAVEFIELD_UNACTIVE_Y);
        SaveField_Active = false;
    }*/

    public void Save()
    {
        Texture2D gridTexture = SaveSystem.SaveGrid(GridController._instance.grid, out string fileName);
        gridTexture.filterMode = FilterMode.Point;

        LoadField.DOMoveRectY(LOADFIELD_UNACTIVE_Y);

        AddGrid(gridTexture, fileName);
    }

    private void LoadSavedGridImages()
    {
        (Texture2D texture, string fileName)[] textures = SaveSystem.LoadSavedGridImages();

        if (null == textures)
        {
            return;
        }

        foreach (var texture in textures)
        {
            AddGrid(texture.texture, texture.fileName);
        }
    }

    public void Load()
    {
        if(-1 == selectedGridIndex)
        {
            SelectGridTextFieldFade.FadeOut();

            return;
        }

        LoadField.DOMoveRectY(LoadField_Active ? LOADFIELD_UNACTIVE_Y : LOADFIELD_ACTIVE_Y);

        LoadField_Active = !LoadField_Active;
    }

    public void ConfirmLoad()
    {
        GridController.LoadGridFromTexture(cachedGrids[selectedGridIndex]);

        LoadField.DOMoveRectY(LOADFIELD_UNACTIVE_Y);

        LoadField_Active = false;

        Deactivate();
    }

    public void SelectGrid(int index)
    {
        if (-1 != selectedGridIndex)
        {
            gridImages[selectedGridIndex].isSelected = false;
        }        

        selectedGridIndex = index;

        PressedHighlight.SetActive(true);
        //((RectTransform)PressedHighlight.transform).anchoredPosition = GetPositionForIndex(index) + new Vector2(1, -1);
        PressedHighlight.transform.SetParent(gridImages[selectedGridIndex].transform, false);
    }

    public void DeselectGrid()
    {
        selectedGridIndex = -1;

        PressedHighlight.SetActive(false);
        PressedHighlight.transform.SetParent(ContentTransform, false);

        LoadField.DOMoveRectY(LOADFIELD_UNACTIVE_Y);
        LoadField_Active = false;
    }

    private Vector2 GetPositionForIndex(int index)
    {
        float x = GRID_IMAGE_FIRST_OFFSET + (index % GRID_IMAGE_COLUMNS * GRID_IMAGE_OFFSETS);
        float y = -(GRID_IMAGE_FIRST_OFFSET + (index / GRID_IMAGE_COLUMNS * GRID_IMAGE_OFFSETS));

        return new Vector2(x, y);
    }

    public void AddGrid(Texture2D image, string fileName)
    {        
        GridImageController controller = Instantiate(GridImageObject, ContentTransform).GetComponent<GridImageController>();
        ((RectTransform)controller.transform).anchoredPosition = GetPositionForIndex(gridImages.Count);        

        controller.index = gridImages.Count;
        controller.image.texture = image;
        controller.fileName = fileName;

        gridImages.Add(controller);
        cachedGrids.Add(image);

        if (gridImages.Count % GRID_IMAGE_COLUMNS == 1)
        {
            ResizeContentField();
        }
    }

    public void DeleteGrid(int index)
    {
        if (false == ConfirmDelete_Active)
        {
            ConfirmDeleteField.gameObject.SetActive(true);
        }

        indexToDelete = index;
    }

    public void ConfirmDeleteGrid()
    {
        if (indexToDelete == selectedGridIndex)
        {
            DeselectGrid();
        }

        Destroy(gridImages[indexToDelete].gameObject);
        SaveSystem.Delete(gridImages[indexToDelete].fileName);

        gridImages.RemoveAt(indexToDelete);
        cachedGrids.RemoveAt(indexToDelete);

        for (int i = indexToDelete; i < gridImages.Count; i++)
        {
            gridImages[i].index = i;
            ((RectTransform)gridImages[i].transform).anchoredPosition = GetPositionForIndex(i);

            if (i == selectedGridIndex)
            {
                SelectGrid(i);
            }
        }

        if (gridImages.Count % GRID_IMAGE_COLUMNS == 0)
        {
            ResizeContentField();
        }

        ConfirmDeleteField.gameObject.SetActive(false);
        ConfirmDelete_Active = false;
    }

    public void CancelDeleteGrid()
    {
        ConfirmDeleteField.gameObject.SetActive(false);
        ConfirmDelete_Active = false;
    }

    private void ResizeContentField()
    {
        ContentTransform.sizeDelta = new Vector2(236, GRID_IMAGE_OFFSETS * (1 + (gridImages.Count - 1) / GRID_IMAGE_COLUMNS) + 4);
    }
}
