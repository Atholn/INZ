using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject[] ItemPrefab;
    public int[,] mapFirstLevel = {
        {0,0,0,0,0,0,0,0,0,0 },
        {0,0,0,0,0,0,0,0,0,0 },
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
        {0,0,0,0,0,0,0,0,0,0 }, 
        {0,0,0,0,0,0,0,0,0,0 }, 
        {0,0,0,0,0,0,0,0,0,0 }, 
        {0,0,0,0,0,0,0,0,0,0 }, 
        {0,0,0,0,0,0,0,0,0,0 }, 
        {0,0,0,0,0,0,0,0,0,0 }, 
        {0,0,0,0,0,0,0,0,0,0 }, 
    };

    public int[,] mapSecondLevel = {
        {2, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    };

    private void Start()
    {

    }

    public void CreateWorld()
    {
        Create(mapFirstLevel, 0);
        Create(mapSecondLevel, 1);
    }

    private void Create(int[,] map, int heightLevel)
    {
        for(int i=0; i<map.GetLength(0); i++)
        {
            for(int j=0; j<map.GetLength(1); j++)
            {
                if (map[i, j] == -1) continue;

                Instantiate(ItemPrefab[map[i,j]], new Vector3(i + 0.5f, heightLevel* 0.5f, j + 0.5f), ItemPrefab[map[i, j]].transform.rotation);
            }
        }
    }
}
