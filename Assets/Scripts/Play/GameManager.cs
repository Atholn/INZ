using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private MapWorld _map;
    private List<GameStartPoint> _gameStartPoints;
    private List<List<GameObject>> _playersGameObjects;
    private List<Material> _playersMaterials;

    public List<Item> TerrainPrefabs;
    public List<GameObject> UnitsPrefabs;
    public List<GameObject> BuildingsPrefabs;

    private GameObject[][][] _gameObjects;
    private GameObject _terrain;
    private GameObject _worker;
    private int _countOfWorkers = 5;
    private GameObject _townHall;

    private GameObject _minimapCamera;

    private GameObject _gameObjectToMove;
    private GameObject _profileCamera;
    private Vector3 _shiftProfileCamera = new Vector3(-0.6f, 6f, 3.7f);

    void Start()
    {
        _map = new MapWorld();
        _gameStartPoints = MapToPlayStorage.GameStartPoints;

        _worker = UnitsPrefabs.Where(w => w.name == "Worker").FirstOrDefault();
        _townHall = BuildingsPrefabs.Where(w => w.name == "TownHall").FirstOrDefault();

        MapLoader.ResetAndLoad(ref _map, ref MapToPlayStorage.Map.MapWorldCreate , ref _gameObjects, ref _terrain, TerrainPrefabs);
        InitializePlayers();

        _profileCamera = GameObject.FindGameObjectWithTag("ProfileCamera");
        _minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera");

        RenderTexture texture = _minimapCamera.GetComponent<Camera>().targetTexture;
        int size = _map.SizeMapX > _map.SizeMapY ? _map.SizeMapX : _map.SizeMapY;
        texture.width = size;
        texture.height = size;

        _minimapCamera.transform.position = new Vector3(_terrain.transform.position.x, size, _terrain.transform.position.z);
    }


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
            _playersGameObjects[i].Add(Instantiate(_townHall, _gameStartPoints[i].UnitStartLocation, _townHall.transform.rotation));
            _playersGameObjects[i][0].transform.GetComponent<MeshRenderer>().materials[1].color = _playersMaterials[i].color;

            for (int j = 0; j < _countOfWorkers; j++)
            {
                _playersGameObjects[i].Add(Instantiate(_worker, _gameStartPoints[i].UnitStartLocation + new Vector3(5 + j * 1, 0, 5), _worker.transform.rotation));
                _playersGameObjects[i][j + 1].transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = _playersMaterials[i].color;
            }
        }
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    _gameObjectToMove = null;
        //    _profileCamera.transform.SetParent(null);
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        foreach (GameObject obj in _playersGameObjects[0])
        //        {
        //            if (hit.collider.gameObject == obj)
        //            {
        //                Debug.Log(obj.name);

        //                _gameObjectToMove = obj;

        //                _profileCamera.transform.SetParent(_gameObjectToMove.transform);
        //                _profileCamera.transform.localPosition = _shiftProfileCamera;
        //                break;
        //            }
        //        }
        //    }
        //}

        //if (Input.GetMouseButtonDown(2))
        //{
        //    if (_gameObjectToMove != null)
        //    {
        //        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //        RaycastHit hit;


        //        if (Physics.Raycast(ray, out hit, 1000.0f))
        //            _gameObjectToMove.transform.position = hit.point;
        //    }
        //}
    }
}
