using System.Collections.Generic;

[System.Serializable]
public class Map
{
    public string CreateTime;//
    public string UpdateTime;//

    public string Name = "";//
    public string Type = "";//
    public string Decription;//

    public int SizeMapX;
    public int SizeMapY;
    public float[][][] ViewMap;

    public List<string> UnitMaterials;
    public List<float[]> UnitStartLocations;
    
    public int[][][] Maps;
}

public class MapPlay
{
    public int[][][] Maps;
}

public class MapInfo
{
    public string Name = "";
    public string Type = "";
    public int PlayersCount;
    public string Decription = "";
    public float[][][] ViewMap;

    public int SizeMapX;
    public int SizeMapY;

}

public class MapSettingsEditor
{
    public string CreateTime;
    public string UpdateTime;
}