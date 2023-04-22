using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    /*public static SaveSystem _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError($"Multiple instances of type {this}");
            return;
        }

        _instance = this;
    }*/

    private static readonly string DIR_PATH = Application.persistentDataPath + "/";

    private static int NextSaveID = 1;

    public static Texture2D SaveGrid(GridNode[,] grid, out string fileName)
    {
        int x_size = grid.GetLength(0) - 2;
        int y_size = grid.GetLength(1) - 2;

        Texture2D texture = new Texture2D(x_size, y_size, TextureFormat.RGB24, false);

        for (int y = 0; y < y_size; y++)
        {
            for (int x = 0; x < x_size; x++)
            {
                Color color = grid[x + 1, y + 1].nodeType == NodeType.empty ? Color.white : Color.black;

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        byte[] imageBytes = texture.EncodeToPNG();        

        try
        {
            File.WriteAllBytes(DIR_PATH + NextSaveID + ".png", imageBytes);
            NextSaveID++;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        fileName = NextSaveID + ".png";
        return texture;
    }

    public static (Texture2D texture, string fileName)[] LoadSavedGridImages()
    {
        FileInfo[] files = new DirectoryInfo(DIR_PATH).GetFiles("*.png");

        if (0 == files.Length)
        {
            return null;
        }

        if (false == int.TryParse(files[^1].Name.Split(".")[0], out NextSaveID))
        {
            Debug.LogError("Incorrect naming of file");
            return null;
        }

        (Texture2D texture, string fileName)[] textures = new (Texture2D texture, string ID)[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            textures[i].texture = LoadGrid(files[i].Name);
            textures[i].fileName = files[i].Name;
        }

        NextSaveID++;
        return textures;
    }

    private static Texture2D ConvertToTexture(byte[] imageBytes)
    {
        Texture2D image = new Texture2D(1, 1);

        if (image.LoadImage(imageBytes))
        {
            return image;
        }

        return null;
    }

    public static Texture2D LoadGrid(string fileName)
    {
        byte[] imageBytes = File.ReadAllBytes(DIR_PATH + fileName);

        Texture2D texture = ConvertToTexture(imageBytes);

        if(null == texture)
        {
            Debug.LogError("Image unable to be loaded");
        }

        texture.filterMode = FilterMode.Point;

        return texture;
    }

    public static void Delete(string name)
    {
        new FileInfo(DIR_PATH + name).Delete();
    }
}
