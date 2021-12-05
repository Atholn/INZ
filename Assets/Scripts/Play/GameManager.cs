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
    private List<Material> _playersMaterials;

    private int sizeMapX;
    private int sizeMapY;
    private int mapCount = 2; //Level 0 - terrain; Level 1 - Nature/Unit
    private int[][][] maps;
    private GameObject[][][] mapsPrefabs;
    private GameObject Terrain;
    public int BasicTerrainID = 0;

    public GameObject basicTerrain;
    private Vector3 basicScale;

    public List<GameObject> TerrainPrefabs;
    public List<GameObject> UnitsPrefabs;
    public List<GameObject> BuildingsPrefabs;

    private GameObject _worker;
    private GameObject _townHall;
    private int _countOfWorkers = 5;
    private GameObject _gameObjectToMove;
    private GameObject _profileCamera;
    private Vector3 _shiftProfileCamera = new Vector3(-0.6f, 6f, 3.7f);

    void Start()
    {
        _map = MapToPlayStorage.Map;
        _gameStartPoints = MapToPlayStorage.GameStartPoints;

        _profileCamera = GameObject.FindGameObjectWithTag("ProfileCamera");

        basicScale = new Vector3(basicTerrain.transform.localScale.x, basicTerrain.transform.localScale.y, basicTerrain.transform.localScale.z);

        InitializeStartMaps();
        ImportMap(_map);

        _worker = UnitsPrefabs.Where(w => w.name == "Worker").FirstOrDefault();
        _townHall = BuildingsPrefabs.Where(w => w.name == "TownHall").FirstOrDefault();

        InitializePlayers();
    }

    private void InitializeStartMaps()
    {
        maps = new int[mapCount][][];
        mapsPrefabs = new GameObject[mapCount][][];
    }

    private void ImportMap(Map map)
    {
        InitializeSizesMaps(map.SizeMapX, map.SizeMapY);
        InitializeStartTerrain();
        InitializeTerrainArrays();
        //InitializeNewMap(map);
    }

    private void InitializeSizesMaps(int sizeMapX, int sizeMapY)
    {
        this.sizeMapX = sizeMapX;
        this.sizeMapY = sizeMapY;
    }

    private void InitializeStartTerrain()
    {
        GameObject basicTerrainPrefab = basicTerrain;

        basicTerrainPrefab.gameObject.transform.localScale = new Vector3(sizeMapX * basicScale.x, basicScale.y, sizeMapY * basicScale.x);
        Terrain = Instantiate(basicTerrainPrefab, new Vector3(sizeMapX / 2 - 0.5f, -0.5f, sizeMapY / 2 - 0.5f), basicTerrainPrefab.transform.rotation);
        basicTerrainPrefab.gameObject.transform.localScale = basicScale;
    }

    private void InitializeTerrainArrays()
    {
        for (int i = 0; i < mapCount; i++)
        {
            maps[i] = new int[sizeMapX][];
            mapsPrefabs[i] = new GameObject[sizeMapX][];

            for (int j = 0; j < sizeMapX; j++)
            {
                maps[i][j] = new int[sizeMapY];
                mapsPrefabs[i][j] = new GameObject[sizeMapY];
            }
        }
    }


    //private void InitializeNewMap(Map map)
    //{
    //    maps = map.Maps;
    //    for (int i = 0; i < mapCount; i++)
    //    {
    //        for (int j = 0; j < sizeMapX; j++)
    //        {
    //            for (int k = 0; k < sizeMapY; k++)
    //            {
    //                if (maps[i][j][k] > 0)
    //                {

    //                    bool ifCreate = true;
    //                    foreach (float[] gameStartPoint in map.UnitStartLocations)
    //                    {
    //                        if (gameStartPoint[0] == j && gameStartPoint[2] == k)
    //                        {
    //                            ifCreate = false;
    //                            break;
    //                        }
    //                    }

    //                    if (ifCreate)
    //                    {
    //                        mapsPrefabs[i][j][k] = Instantiate(TerrainPrefabs[maps[i][j][k]], new Vector3(j, i, k), TerrainPrefabs[maps[i][j][k]].transform.rotation);

    //                    }
    //                    continue;
    //                }

    //                maps[i][j][k] = 0;
    //            }
    //        }
    //    }
    //}

    private void InitializePlayers()
    {
        _playersGameObjects = new List<List<GameObject>>();
        _playersMaterials = new List<Material>();

        for (int i = 0; i < _gameStartPoints.Count; i++)
        {
            _playersGameObjects.Add(new List<GameObject>());
            _playersMaterials.Add(_gameStartPoints[i].UnitMaterial);
        }

        for (int i = 0; i < _gameStartPoints.Count; i++)
        {
            MeshRenderer mesh = _townHall.GetComponent<MeshRenderer>();
            mesh.material = _playersMaterials[i];
            _playersGameObjects[i].Add(Instantiate(_townHall, _gameStartPoints[i].UnitStartLocation, _townHall.transform.rotation));

            SkinnedMeshRenderer skinnedMesh = _worker.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMesh.material = _playersMaterials[i];

            for (int j = 0; j < _countOfWorkers; j++)
            {
                _playersGameObjects[i].Add(Instantiate(_worker, _gameStartPoints[i].UnitStartLocation + new Vector3(5 + j * 1, 0, 5), _worker.transform.rotation));
            }
        }

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                foreach (GameObject obj in _playersGameObjects[0])
                {
                    if (hit.collider.gameObject == obj)
                    {
                        Debug.Log(obj.name);

                        _gameObjectToMove = obj;


                        _profileCamera.transform.SetParent(_gameObjectToMove.transform);
                        _profileCamera.transform.localPosition = _shiftProfileCamera;
                        break;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            if (_gameObjectToMove != null)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;


                if (Physics.Raycast(ray, out hit, 1000.0f))
                    _gameObjectToMove.transform.position = hit.point;
                _profileCamera.transform.SetParent(null);
            }
        }
    }
}
