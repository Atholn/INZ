using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapToPlayStorage : MonoBehaviour
{
    enum Wins
    {
        free = 0, // -
        dominate = 1, // 0 - lose, 1 - win
        wood = 2, //cout
        gold = 3, //cout
        units = 4, //cout
        upgrades = 5, // all upgrades
        soldiers = 6, // cout of soldiers
        soldiersType = 7, // cout of soldiers type, 
        time = 8,
        attacks = 9,
    }

    public static Map Map;
    public static List<GameStartPoint> GameStartPoints;

    public static Dictionary<string, string> WinRequaried = new Dictionary<string, string>();
    public static string SceneToBack;

    internal static List<T> ImportResources<T>(string path, string end) where T : UnityEngine.Object
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
        return new DirectoryInfo(Path.GameAppPath + $"/Resources/{path}")
            .GetFiles()
            .Where(n => !n.Name.Contains(".meta") && n.Name.Contains(end))
            .Select(n => n.Name)
            .ToList();
    }

    internal static void AddDominateRequaried(bool loseWin)
    {
        WinRequaried.Add(Wins.dominate.ToString(), loseWin.ToString());
    }
}
