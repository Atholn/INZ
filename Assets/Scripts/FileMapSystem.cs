using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public static class Pathe
{
    public static string GameAppPath
    {
        get
        {
#if UNITY_EDITOR
            return Application.dataPath;
#else
            return Directory.GetCurrentDirectory();
#endif
        }
    }
}

public class FileMapSystem
{
    enum Flag
    {
        SaveGenerate,
        Load
    }

    private string _path = Pathe.GameAppPath + $"/Game/Maps/";
    public string FolderName;

    private void SaveLoadMapFile(string tmpPath, ref Map map, FileMode fileMode, Flag flag)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = new FileStream(tmpPath, fileMode);
        switch (flag)
        {
            case Flag.SaveGenerate:
                binaryFormatter.Serialize(stream, map);
                break;
            case Flag.Load:
                map = binaryFormatter.Deserialize(stream) as Map;
                break;
            default:
                break;
        }
        stream.Close();
    }

    public bool CheckIfExist(string mapName)
    {
        string tmpPath = _path + $"/{FolderName}/{mapName}";

        if (File.Exists(tmpPath))
        {
            return true;
        }

        return false;
    }

    public void SaveEditorMap(ref Map map)
    {
        string tmpPath = _path + $"/{FolderName}/{map.Name}";

        if (File.Exists(tmpPath))
        {
            SaveLoadMapFile(tmpPath, ref map, FileMode.Open, Flag.SaveGenerate);

            return;
        }

        SaveLoadMapFile(tmpPath, ref map, FileMode.Create, Flag.SaveGenerate);
    }

    public Map LoadEditorMap(string nameMap)
    {
        string tmpPath = _path + $"/{FolderName}/{nameMap}";

        if (File.Exists(tmpPath))
        {
            Map loadMap = new Map();
            SaveLoadMapFile(tmpPath, ref loadMap, FileMode.Open, Flag.Load);

            return loadMap;
        }

        return null;
    }

    public void GenerateEditorMap(Map map)
    {
        string tmpPath = _path + $"/{map.Type}/{map.Name}";

        if (File.Exists(tmpPath))
        {
            SaveLoadMapFile(tmpPath, ref map, FileMode.Open, Flag.SaveGenerate);

            return;
        }

        if (!File.Exists(tmpPath))
        {
            SaveLoadMapFile(tmpPath, ref map, FileMode.Create, Flag.SaveGenerate);

            return;
        }
    }

    public List<string> GetNamesMaps(string type)
    {
        string tmpPath = _path + $"/{type}/";
        var info = new DirectoryInfo(tmpPath);
        var fileInfo = info.GetFiles();

        List<string> NamesOfMaps = new List<string>();

        foreach (var file in fileInfo)
        {
            NamesOfMaps.Add(file.Name as string);
        }

        NamesOfMaps = NamesOfMaps.Where(n => !n.Contains(".meta")).ToList();

        //todo  pliki z koncowkami ktore pomijamy

        return NamesOfMaps;
    }

    public Map GetMapInfo(string type, string nameMap)
    {
        string tmpPath = _path + $"/{type}/{nameMap}";

        if (File.Exists(tmpPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream stream = new FileStream(tmpPath, FileMode.Open);

            Map map = binaryFormatter.Deserialize(stream) as Map;
            stream.Close();
            return map;
        }

        return null;
    }
}
