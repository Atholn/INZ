using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Map
{
    public bool ifExist = false;
    public bool ifGenerated = false;
    public bool saveAs = false;
    public bool nameToChange = false;

    public string Name = "";
    public string Type = "";
    public string Decription;

    public string CreateTime;
    public string UpdateTime;

    public int SizeMap;
    public int[][,] Maps;
    public float[][][] ViewMap;

    public List<string> UnitMaterials;
    public List<float[]> UnitStartLocations;
}
