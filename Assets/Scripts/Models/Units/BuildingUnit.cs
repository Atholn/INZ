using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUnit : Unit
{
    public int Size;
    public bool PlaceWood;
    public bool PlaceGold;

    public float HeightBuilding;
    public float SizeBuilding;
    internal float BuildingPercent = 0f;
}
