using System.Collections.Generic;

[System.Serializable]
public class Map
{
    public MapWorld MapWorldCreate;
    public MapInfo MapInfo;
}

[System.Serializable]
public class MapWorld
{
    public int MapsCount = 2;//Level 0 - terrain; Level 1 - Nature/Unit
    public int[][][] Maps;
    public int MainGroundID = 0;

    public int SizeMapX;
    public int SizeMapY;

    public List<EditorStartPoint> StartPoints;
}

[System.Serializable]
public class MapInfo
{
    public string Name = "";
    public string Type = "";
    public string Decription = "";
    public float[][][] ViewMap;

    public string CreateTime;
    public string UpdateTime;
}