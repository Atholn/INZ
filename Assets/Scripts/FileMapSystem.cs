using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Linq;

public static class FileMapSystem
{
    public static void SaveMap(ref Map map)
    {
        if (map.ifExist && !map.saveAs)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.dataPath + $"/Game/Maps/Editor/{map.name}";

            FileStream stream = new FileStream(path, FileMode.Open);

            formatter.Serialize(stream, map);

            stream.Close();
            return;
        }

        if (!map.ifExist || map.saveAs)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.dataPath + $"/Game/Maps/Editor/{map.name}";

            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, map);
            stream.Close();

            map.ifExist = true;
            map.saveAs = false;
        }
    }

    public static Map LoadMap(string nameMap)
    {
        string path = Application.dataPath + $"/Game/Maps/Editor/{nameMap}";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Map map = formatter.Deserialize(stream) as Map;
            stream.Close();

            return map;
        }
        else
        {
            return null;
        }
    }

    public static List<string> GetNamesMaps()
    {
        string path = Application.dataPath + $"/Game/Maps/Editor/";
        var info = new DirectoryInfo(path);
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

    public static Map GetMapInfo(string nameMap)
    {
        string path = Application.dataPath + $"/Game/Maps/Editor/{nameMap}";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Map map = formatter.Deserialize(stream) as Map;
            stream.Close();
            return map;
        }
        else
        {
            return null;
        }
    }

}
