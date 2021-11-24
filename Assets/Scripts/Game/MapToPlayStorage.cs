using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapToPlayStorage : MonoBehaviour
{
    public static Map Map;
    public static List<Material> MaterialList = new List<Material>();

    public static void ImportMaterials()
    {
        List<string> list = GetNamesMaps();

        foreach (string materialName in list)
        {
            MaterialList.Add(Resources.Load<Material>($"Materials/Units/{materialName.Remove(materialName.IndexOf("."), 4)}"));
        }
    }

    private static List<string> GetNamesMaps()
    {
        return new DirectoryInfo(Application.dataPath + "/Resources/Materials/Units/")
            .GetFiles()
            .Where(n => !n.Name.Contains(".meta") && n.Name.Contains(".mat"))
            .Select(n => n.Name)
            .ToList();
    }
}
