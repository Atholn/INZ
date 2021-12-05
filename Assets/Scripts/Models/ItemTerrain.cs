using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTerrain : Item
{
    public bool AllowsBuild = true;
    internal float HeightCreateAsBasicTerrain;

    internal void Initialize()
    {
        HeightCreateAsBasicTerrain = ItemHeightPosY - 0.01f;
    }
}
