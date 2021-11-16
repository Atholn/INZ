using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class FileMapSystem
{
    enum Flag
    {
        SaveGenerate,
        Load
    }

    private string path = Application.dataPath + $"/Game/Maps/";
    public string FolderName;

    private void SaveLoadMapFile(string tmpPath, ref Map map, FileMode fileMode, Flag flag)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(tmpPath, fileMode);
        switch (flag)
        {
            case Flag.SaveGenerate:
                formatter.Serialize(stream, map);
                break;
            case Flag.Load:
                map = formatter.Deserialize(stream) as Map;
                break;
            default:
                break;
        }
        stream.Close();
    }

    public void SaveMap(ref Map map)
    {
        string tmpPath = path + $"/{FolderName}/{map.Name}";

        if (!map.ifExist && File.Exists(tmpPath))
        {
            map.nameToChange = true;
            return;
        }
        else
        {
            map.nameToChange = false;
        }

        if (map.ifExist && !map.saveAs)
        {
            SaveLoadMapFile(tmpPath, ref map, FileMode.Open, Flag.SaveGenerate);

            return;
        }

        if (!map.ifExist || map.saveAs)
        {
            SaveLoadMapFile(tmpPath, ref map, FileMode.Create, Flag.SaveGenerate);

            map.ifExist = true;
            map.saveAs = false;

            return;
        }
    }

    public Map LoadMap(string nameMap)
    {
        string tmpPath = path + $"/{FolderName}/{nameMap}";

        if (File.Exists(tmpPath))
        {
            Map loadMap = new Map();
            SaveLoadMapFile(tmpPath, ref loadMap, FileMode.Open, Flag.Load);

            return loadMap;
        }

        return null;
    }

    public void Generate(Map map)
    {
        string tmpPath = path + $"/{map.Type}/{map.Name}";

        if (File.Exists(tmpPath))
        {
            SaveLoadMapFile(tmpPath, ref map, FileMode.Open, Flag.SaveGenerate);

            return;
        }

        if (!File.Exists(tmpPath))
        {
            SaveLoadMapFile(tmpPath, ref map, FileMode.Create, Flag.SaveGenerate);
            map.ifGenerated = true;

            return;
        }
    }

    public Map GetMapInfo(string type, string nameMap)
    {
        string tmpPath = path + $"/{type}/{nameMap}";

        if (File.Exists(tmpPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(tmpPath, FileMode.Open);

            Map map = formatter.Deserialize(stream) as Map;
            stream.Close();
            return map;
        }
        else
        {
            return null;
        }
    }

    public List<string> GetNamesMaps(string type)
    {
        string tmpPath = path + $"/{type}/";
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

}
