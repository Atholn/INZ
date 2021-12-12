using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    internal static void ResetAndLoad(ref MapWorld actualMap, ref MapWorld newMap, ref GameObject[][][] mapsPrefabs, ref GameObject terrain, ItemController[] itemControllers, int mainGroundID = 0)
    {
        DeleteMapGameObjects(ref actualMap, ref mapsPrefabs, ref terrain);

        InitializeStartTerrain(newMap.SizeMapX, newMap.SizeMapY, itemControllers[mainGroundID], ref terrain);
        InitializeTerrainArrays(newMap.SizeMapX, newMap.SizeMapY, ref actualMap, ref mapsPrefabs);

        actualMap = newMap;
        InitializeNewMap(ref actualMap, ref mapsPrefabs, itemControllers);
    }

    internal static void InitializeNewMap(ref MapWorld newMap, ref GameObject[][][] mapsPrefabs, ref GameObject terrain, ItemController[] itemControllers, int mainGroundID = 0)
    {
        InitializeStartTerrain(newMap.SizeMapX, newMap.SizeMapY, itemControllers[mainGroundID], ref terrain);
        InitializeTerrainArrays(newMap.SizeMapX, newMap.SizeMapY, ref newMap, ref mapsPrefabs);
    }

    public static void DeleteMapGameObjects(ref MapWorld map, ref GameObject[][][] mapsPrefabs, ref GameObject terrain)
    {
        for (int k = 0; k < map.MapsCount; k++)
        {
            for (int i = 0; i < map.SizeMapX; i++)
            {
                for (int j = 0; j < map.SizeMapY; j++)
                {
                    if (mapsPrefabs[k][i][j] != null)
                    {
                        Destroy(mapsPrefabs[k][i][j]);
                    }
                }
            }
        }

        Destroy(terrain);
    }

    private static void InitializeStartTerrain(int sizeX, int sizeY, ItemController mainGround, ref GameObject terrain)
    {
        Vector3 firstScale = mainGround.item.ItemPrefab.transform.localScale;

        GameObject basicTerrainPrefab = mainGround.item.ItemPrefab;
        basicTerrainPrefab.gameObject.transform.localScale = new Vector3(sizeX * firstScale.x, sizeY * firstScale.y, firstScale.z);

        terrain = Instantiate(basicTerrainPrefab, new Vector3(sizeX % 2 == 0 ? sizeX / 2 - 0.5f : sizeX / 2, (mainGround.item as ItemTerrain).HeightCreateAsBasicTerrain, sizeY % 2 == 0 ? sizeY / 2 - 0.5f : sizeY / 2), basicTerrainPrefab.transform.rotation);

        basicTerrainPrefab.gameObject.transform.localScale = firstScale;
    }

    private static void InitializeTerrainArrays(int sizeX, int sizeY, ref MapWorld actualMap, ref GameObject[][][] mapsPrefabs)
    {
        actualMap.Maps = new int[actualMap.MapsCount][][];
        mapsPrefabs = new GameObject[actualMap.MapsCount][][];

        actualMap.SizeMapX = sizeX;
        actualMap.SizeMapY = sizeY;

        for (int i = 0; i < actualMap.MapsCount; i++)
        {
            actualMap.Maps[i] = new int[sizeX][];
            mapsPrefabs[i] = new GameObject[sizeX][];

            for (int j = 0; j < sizeX; j++)
            {
                actualMap.Maps[i][j] = new int[sizeY];
                mapsPrefabs[i][j] = new GameObject[sizeY];
            }
        }
    }

    private static void InitializeNewMap(ref MapWorld actualMap, ref GameObject[][][] mapsPrefabs, ItemController[] itemControllers)
    {
        for (int i = 0; i < actualMap.MapsCount; i++)
        {
            for (int j = 0; j < actualMap.SizeMapX; j++)
            {
                for (int k = 0; k < actualMap.SizeMapY; k++)
                {
                    if (actualMap.Maps[i][j][k] > 0)
                    {
                        mapsPrefabs[i][j][k] = Instantiate(itemControllers[actualMap.Maps[i][j][k]].item.ItemPrefab, new Vector3(j, itemControllers[actualMap.Maps[i][j][k]].item.ItemHeightPosY, k), itemControllers[actualMap.Maps[i][j][k]].item.ItemPrefab.transform.rotation);
                    }
                }
            }
        }
    }
}
