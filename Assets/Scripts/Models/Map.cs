using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Map
{
    public bool ifExist = false;
    public bool ifGenerated = false;
    public bool saveAs = false;
    public bool nameToChange = false;

    public string Name = "";
    public string Type = "";

    public string CreateTime;
    public string UpdateTime;
    public string Decription;

    public int SizeMap;
    public int[][,] Maps;

    public List<string> UnitMaterials;
    public List<float[]> UnitStartLocations;
}
