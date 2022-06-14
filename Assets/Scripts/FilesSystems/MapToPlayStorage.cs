using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapToPlayStorage : MonoBehaviour
{
    enum Wins
    {
        free = 0, 
        dominate = 1, // 0 - lose, 1 - win
        sources = 2, //  0 - all*, 1 - gold, 2 - wood, 3 - untis (food), 4 - units max // count of sources  *gold and wood 
        upgrades = 3, // 0 - all, 1 - knight, 2 - axeman, 3 - bowman // true false
        soldiers = 4, // 0 - all, 1 - knight, 2 - axeman, 3 - bowman  // count    
        buldings = 5, // 0 - all, 1 - townhall, 2 - barracks, 3 - farm, 4 - blacksmith, 5 - tower
    }

    public static Map Map;
    public static List<GameStartPoint> GameStartPoints;

    internal static Dictionary<string, Dictionary<string, string>> WinRequarieds = new Dictionary<string, Dictionary<string, string>>();
    public static string SceneToBack;

    #region Resources
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
    #endregion

    #region Requaried
    internal static void ResetRequaried()
    {
        WinRequarieds = new Dictionary<string, Dictionary<string, string>>();
    }

    //0
    internal static void AddFreeRequaried()
    {
        WinRequarieds.Add(Wins.free.ToString(), new Dictionary<string, string>());
    }

    //1
    internal static void AddDominateRequaried(bool loseWin)
    {
        var additionalDictionary = new Dictionary<string, string>();
        additionalDictionary.Add("win", loseWin.ToString());
        WinRequarieds.Add(Wins.dominate.ToString(), additionalDictionary);
    }

    //2
    internal static void AddSourcesRequaried(int whichSoruces, int count)
        => AddRequaried(Wins.sources.ToString(), whichSoruces.ToString(), count.ToString());

    //3
    internal static void AddUpgradesRequaried(int whichUpgrades, bool ifUpgrade)
        => AddRequaried(Wins.upgrades.ToString(), whichUpgrades.ToString(), ifUpgrade.ToString());

    //4
    internal static void AddSoldiersRequaried(int whichSoldiers, int count)
        => AddRequaried(Wins.soldiers.ToString(), whichSoldiers.ToString(), count.ToString());

    //5
    internal static void AddBuildingRequaried(int whichBuildings, int count)
        => AddRequaried(Wins.buldings.ToString(), whichBuildings.ToString(), count.ToString());

    private static void AddRequaried(string type, string which, string cout)
    {
        if (WinRequarieds.Keys.Where(k => k == type).Count() == 0)
        {
            var additionalDictionary = new Dictionary<string, string>();
            additionalDictionary.Add(which, cout);
            WinRequarieds.Add(type, additionalDictionary);
        }
        else
        {
            WinRequarieds[type].Add(which, cout);
        }
    }
    #endregion Requaried
}
