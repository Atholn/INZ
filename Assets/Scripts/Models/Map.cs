using System.Collections.Generic;

[System.Serializable]
public class Map
{
    #region Map create Info 
    public int MapsCount = 2;//Level 0 - terrain; Level 1 - Nature/Unit
    public int[][][] Maps;
    public List<float[]> UnitStartLocations;

    public int SizeMapX;
    public int SizeMapY;
    #endregion

    #region Map selection info 
    public string Name = "";
    public string Type = "";
    public string Decription = "";
    public float[][][] ViewMap;
    #endregion

    #region Map editor info
    public string CreateTime;
    public string UpdateTime;
    #endregion
}
