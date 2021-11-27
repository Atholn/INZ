using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Map _map;
    private List<GameStartPoint> _gameStartPoints;
    private List<List<GameObject>> _playersGameObjects;

    private int sizeMap;
    private int mapCount = 2; //Level 0 - terrain; Level 1 - Nature/Unit
    private int[][,] maps;
    private GameObject[][,] mapsPrefabs;
    private GameObject Terrain;
    public int BasicTerrainID = 0;

    public int[,,] hakuna = new int[3, 3, 3];

    public GameObject basicTerrain;
    private Vector3 basicScale;

    public GameObject[] TerrainPrefabs;
    public GameObject[] UnitsPrefabs;
    public GameObject[] BuildingsPrefabs;

    private GameObject _worker;

    void Start()
    {
        _map = MapToPlayStorage.Map;
        _gameStartPoints = MapToPlayStorage.GameStartPoints;

        foreach (GameStartPoint gameStartPoint in _gameStartPoints)
        {
            Debug.Log(gameStartPoint.UnitStartLocation);
        }

        basicScale = new Vector3(basicTerrain.transform.localScale.x, basicTerrain.transform.localScale.y, basicTerrain.transform.localScale.z);
        InitializeStartMaps();
        ImportMap(_map);

        InitializePlayers();

        _worker = UnitsPrefabs.Where(w => w.name == "Worker").FirstOrDefault();
    }

    private void InitializeStartMaps()
    {
        maps = new int[mapCount][,];
        mapsPrefabs = new GameObject[mapCount][,];
    }

    public void ImportMap(Map map)
    {
        InitializeTerrainArrays(map.SizeMap);
        InitializeStartTerrain(sizeMap);
        InitializeNewMap(map);
    }

    private void InitializeTerrainArrays(int size)
    {
        sizeMap = size;

        for (int i = 0; i < mapCount; i++)
        {
            maps[i] = new int[size, size];
            mapsPrefabs[i] = new GameObject[size, size];
        }
    }

    internal void InitializeStartTerrain(int size)
    {
        GameObject basicTerrainPrefab = basicTerrain;

        basicTerrainPrefab.gameObject.transform.localScale = new Vector3(size * basicScale.x, basicScale.y, size * basicScale.x);
        Terrain = Instantiate(basicTerrainPrefab, new Vector3(size / 2 - 0.5f, -0.5f, size / 2 - 0.5f), basicTerrainPrefab.transform.rotation);
        basicTerrainPrefab.gameObject.transform.localScale = basicScale;
        InitializeTerrainArrays(size);
    }

    private void InitializeNewMap(Map map)
    {
        maps = map.Maps;
        for (int i = 0; i < mapCount; i++)
        {
            for (int j = 0; j < sizeMap; j++)
            {
                for (int k = 0; k < sizeMap; k++)
                {
                    if (maps[i][j, k] >= 0)
                    {

                        bool ifCreate = true;
                        foreach (GameStartPoint gameStartPoint in _gameStartPoints)
                        {
                            if (gameStartPoint.UnitStartLocation.x == j && gameStartPoint.UnitStartLocation.y == i && gameStartPoint.UnitStartLocation.z == k)
                            {
                                ifCreate = false;
                                break;
                            }
                        }

                        if (ifCreate)
                        {
                            mapsPrefabs[i][j, k] = Instantiate(TerrainPrefabs[maps[i][j, k]], new Vector3(j, i, k), TerrainPrefabs[maps[i][j, k]].transform.rotation);

                        }
                        continue;
                    }


                    maps[i][j, k] = 0;

                }
            }
        }
    }

    //private void InitializeNewMap(Map map)
    //{
    //    maps = map.Maps;
    //    for (int i = 0; i < mapCount; i++)
    //    {
    //        for (int j = 0; j < sizeMap; j++)
    //        {
    //            for (int k = 0; k < sizeMap; k++)
    //            {
    //                if (maps[i][j, k] > 0)
    //                {
    //                    InstantiateAsync(i, j, k);
    //                    //mapsPrefabs[i][j, k] = Instantiate(Prefabs[maps[i][j, k]], new Vector3(j, i, k), Prefabs[maps[i][j, k]].transform.rotation);
    //                }
    //            }
    //        }
    //    }
    //}

    //async void InstantiateAsync(int i, int j, int k)
    //{
    //    await Task.Delay(1000);
    //    mapsPrefabs[i][j, k] = GameObject.Instantiate(Prefabs[maps[i][j, k]], new Vector3(j, i, k), Prefabs[maps[i][j, k]].transform.rotation);
    //}

    private void InitializePlayers()
    {
        _playersGameObjects = new List<List<GameObject>>();
        for (int i = 0; i < _gameStartPoints.Count; i++)
        {
            _playersGameObjects.Add(new List<GameObject>());
        }



        // robotnicy 
        // 
    }

    void Update()
    {

    }
}
