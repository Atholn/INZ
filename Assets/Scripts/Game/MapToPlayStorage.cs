using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapToPlayStorage : MonoBehaviour
{
    public static Map Map;
    private static List<Material> MaterialList = new List<Material>();
    private static List<Sprite> ColorList = new List<Sprite>();

    public static List<Material> ImportMaterials()
    {
        List<string> list = GetNamesMaps();

        foreach (string materialName in list)
        {
            MaterialList.Add(Resources.Load<Material>($"Materials/Units/{materialName.Remove(materialName.IndexOf("."), 4)}"));
        }

        return MaterialList;
    }

    private static List<string> GetNamesMaps()
    {
        return new DirectoryInfo(Application.dataPath + "/Resources/Materials/Units/")
            .GetFiles()
            .Where(n => !n.Name.Contains(".meta")
            && n.Name.Contains(".mat"))
            .Select(n => n.Name)
            .ToList();
    }

    public static List<Sprite> ImportColorsSpirtes()
    {
        List<string> list = GetNamesColorsSpirtes();

        foreach (string colorName in list)
        {
            ColorList.Add(Resources.Load<Sprite>($"UI/UnitColors/{colorName.Remove(colorName.IndexOf("."), 4)}"));
        }

        return ColorList;
    }

    private static List<string> GetNamesColorsSpirtes()
    {
        return new DirectoryInfo(Application.dataPath + "/Resources/UI/UnitColors/")
            .GetFiles()
            .Where(n => !n.Name.Contains(".meta")
            && n.Name.Contains(".bmp"))
            .Select(n => n.Name)
            .ToList();
    }
}
