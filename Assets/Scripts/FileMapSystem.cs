using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class FileMapSystem 
{
    private string path = Application.dataPath + $"/Game/Maps/";

    public void SaveMap(ref Map map)
    {
        string tmpPath = path + $"/Editor/{map.name}";
        if (map.ifExist && !map.saveAs)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(tmpPath, FileMode.Open);

            formatter.Serialize(stream, map);

            stream.Close();
            return;
        }

        if (!map.ifExist || map.saveAs)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(tmpPath, FileMode.Create);

            formatter.Serialize(stream, map);
            stream.Close();

            map.ifExist = true;
            map.saveAs = false;
        }
    }

    public Map LoadMap(string nameMap)
    {
        string tmpPath = path + $"/Editor/{nameMap}";

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

    public void Generate(Map map)
    {
        string tmpPath = path + $"/{map.type}/{map.name}";

        if (File.Exists(tmpPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(tmpPath, FileMode.Open);

            formatter.Serialize(stream, map);

            stream.Close();
            Debug.Log("Whoo1");
            return;
        }

        if (!File.Exists(tmpPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(tmpPath, FileMode.Create);

            formatter.Serialize(stream, map);
            stream.Close();

            map.ifGenerated = true;
            Debug.Log("Whoo2");
            return;
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

}
