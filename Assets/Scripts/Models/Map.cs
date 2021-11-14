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

    public string name = "";
    public string type = "";

    public string CreateTime;
    public string UpdateTime;

    public int firstValue;
    public int secondValue;
    public int thirdValue;

    public string Decription;
}
