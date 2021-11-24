using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapToPlayStorage : MonoBehaviour
{
    public static Map Map;
    public static List<GameStartPoint> GameStartPoints;

    public static List<T> ImportResources<T>(string path, string end) where T: UnityEngine.Object
    {
        List<string> list = GetNamesResources(path, end);
        List<T> genericList = new List<T>();

        foreach (string name in list)
        {         
            genericList.Add(Resources.Load<T>($"{path}{name.Remove(name.IndexOf("."), end.Length)}"));
        }

        return genericList;
    }

    private static List<string> GetNamesResources(string path, string end)
    {
        return new DirectoryInfo(Application.dataPath + $"/Resources/{path}")
            .GetFiles()
            .Where(n => !n.Name.Contains(".meta") && n.Name.Contains(end))
            .Select(n => n.Name)
            .ToList();
    }
}
