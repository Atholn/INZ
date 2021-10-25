using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMapInitialize : MonoBehaviour
{
    public GameObject BasicTerrain;
    void Start()
    {
        TerrainInitialize();
    }

    private void TerrainInitialize()
    {
        for (int i = 0; i <100; i++)
        {
            for (int j = 0; j <100; j++)
            {
                Instantiate(BasicTerrain, new Vector3(i + 0.5f, 0, j + 0.5f), BasicTerrain.transform.rotation);
            }
        }
    }
}
