using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Map
{
    public string CreateTime;
    public string UpdateTime;

    public string Name = "";
    public string Type = "";
    public string Decription;


    public int SizeMap;
    public int[][,] Maps;
    public float[][][] ViewMap;

    public List<string> UnitMaterials;
    public List<float[]> UnitStartLocations;
}

public class MapPlay
{
    public int[][,] Maps;
}

public class MapInfo
{
    public string Name = "";
    public string Type = "";
    public int PlayersCount;

    public int SizeMapX;
    public int SizeMapY;

    public string Decription = "";
    public float[][][] ViewMap;
}

public class MapSettingsEditor
{
    public bool ifExist = false;
    public bool ifGenerated = false;
    public bool saveAs = false;
    public bool nameToChange = false;

    public string CreateTime;
    public string UpdateTime;
}