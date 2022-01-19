using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public ItemController[] ItemControllers;
    internal int CurrentButtonPressed;

    public List<Item> TerrainPrefabs;
    public List<GameObject> UnitsPrefabs;
    public List<GameObject> BuildingsPrefabs;

    internal List<List<GameObject>> _playersGameObjects;
    internal BuildingUnit actualClickBuild;

    private MapWorld _map;
    private List<GameStartPoint> _gameStartPoints;
    internal List<Material> _playersMaterials;

    private GameObject[][][] _gameObjects;
    private GameObject _terrain;
    private GameObject _worker;
    private GameObject _townHall;

    private GameObject _minimapCamera;

    private GameUI _gameUI;

    private int _countOfWorkers = 5;
    private int _maxSelected = 24;

    internal bool building = false;

    private GameObject onlyOneSelectGO = null;

    RaycastHit hit;
    internal Vector3 v;
    public NavMeshBaker navMeshBaker;

    internal List<GameObject> Nature = new List<GameObject>();

    void Start()
    {
        _gameUI = FindObjectOfType<GameUI>();

        _map = new MapWorld();
        _gameStartPoints = MapToPlayStorage.GameStartPoints;

        _worker = UnitsPrefabs.Where(w => w.name == "Worker").FirstOrDefault();
        _townHall = BuildingsPrefabs.Where(w => w.name == "TownHall").FirstOrDefault();

        MapLoader.ResetAndLoad(ref _map, ref MapToPlayStorage.Map.MapWorldCreate, ref _gameObjects, ref _terrain, TerrainPrefabs);

        for (int i = 0; i < _gameObjects.Length; i++)
        {
            for (int j = 0; j < _gameObjects[i].Length; j++)
            {
                for (int k = 0; k < _gameObjects[i][j].Length; k++)
                {
                    if (_gameObjects[i][j][k] != null)
                    {
                        Nature.Add(_gameObjects[i][j][k]);
                    }
                }
            }
        }


        //for (int j = 0; j < _gameObjects[0].Length; j++)
        //{
        //    for (int k = 0; k < _gameObjects[0][j].Length; k++)
        //    {
        //        if (_gameObjects[0][j][k] != null)
        //        {
        //            navMeshBaker.navMeshSurfaces.Add(_gameObjects[0][j][k].GetComponent<NavMeshSurface>());
        //        }
        //    }
        //}
        InitializePlayers();
        navMeshBaker.navMeshSurfaces.Add(_terrain.GetComponent<NavMeshSurface>());
        navMeshBaker.Bake();
        NavMeshData nmd = _terrain.GetComponent<NavMeshSurface>().navMeshData;
        nmd.sourceBounds.center.Set(500f, 0f, 500f);
        nmd.sourceBounds.extents.Set(500f, 0f, 500f);

        navMeshBaker.navMeshSurfaces[0].navMeshData = nmd;

        _minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera");

        RenderTexture texture = _minimapCamera.GetComponent<Camera>().targetTexture;
        int size = _map.SizeMapX > _map.SizeMapY ? _map.SizeMapX : _map.SizeMapY;
        texture.width = size;
        texture.height = size;

        _minimapCamera.transform.position = new Vector3(_terrain.transform.position.x, size - 0.1f * size, _terrain.transform.position.z);

        _gameUI.SetLookBottomPanel(_playersMaterials[0].color);

        for (int i = 0; i < ItemControllers.Length; i++)
        {
            ItemControllers[i].item.ID = i;
        }
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
            _playersGameObjects[i][0].GetComponent<BuildingUnit>().BuildingPercent = _playersGameObjects[i][0].GetComponent<BuildingUnit>().CreateTime + 0.01f;

            //_playersGameObjects[i][0].GetComponent<NavMeshSurface>().BuildNavMesh();
            //navMeshBaker.navMeshSurfaces.Add(_playersGameObjects[i][0].GetComponent<NavMeshSurface>());
            //

            for (int j = 0; j < _countOfWorkers; j++)
            {
                _playersGameObjects[i].Add(Instantiate(_worker, _gameStartPoints[i].UnitStartLocation + new Vector3(5 + j * 1, 0, 5), _worker.transform.rotation));
                _playersGameObjects[i][j + 1].transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = _playersMaterials[i].color;
            }
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            v = hit.point;
        }




        if (Input.GetMouseButtonDown(1))
        {
            DestroyItemImages();

            building = false;
        }

        //if(building && Input.GetMouseButtonDown(0))
        //{
        //    Instantiate(ItemControllers[CurrentButtonPressed].item.ItemPrefab,
        //        new Vector3(v.x, ItemControllers[CurrentButtonPressed].item.ItemHeightPosY, v.z),
        //        ItemControllers[CurrentButtonPressed].item.ItemPrefab.transform.rotation);

        //    DestroyItemImages();
        //    building = false;
        //}



        if(onlyOneSelectGO != null)
        {
            _gameUI.ShowProgressCreateUnitPanel(onlyOneSelectGO);
        }
    }

    internal void DestroyItemImages()
    {
        for (int i = 0; i < ItemControllers.Length; i++)
        {
            ItemControllers[i].Clicked = false;
            if (ItemControllers[i].item.ItemHeightLevel == 0)
            {
                ItemControllers[i].item.ItemImage.transform.localScale = ItemControllers[i].firstScale;
            }
        }

        GameObject[] itemImages = GameObject.FindGameObjectsWithTag("ItemImage");

        for (int i = 0; i < itemImages.Length; i++)
        {
            Destroy(itemImages[i]);
        }
    }

    internal void SetNonProfile()
    {
        worker = null;

        actualClickBuild = null;

        _gameUI.SetNonProfile();

        onlyOneSelectGO = null;
    }

    HumanUnit worker = null;

    public void SetProfile(GameObject gameObject, int i)
    {
        _gameUI.SetCharacterInfo(gameObject, i);
        if (gameObject.GetComponent<HumanUnit>())
        {
            worker = gameObject.GetComponent<HumanUnit>();
        }

        if (gameObject.GetComponent<BuildingUnit>() != null)
        {
            actualClickBuild = gameObject.GetComponent<BuildingUnit>();
        }
        onlyOneSelectGO = gameObject;
    }

    internal void SetProfiles(List<GameObject> selectUnits)
    {
        worker = null;
        selectUnits = selectUnits.OrderBy(x => x.GetComponent<Unit>().Priority).ToList();
        _gameUI.SetCharactersProfiles(selectUnits, _maxSelected);

        actualClickBuild = null;
        onlyOneSelectGO = null;
    }

    internal int GetMaxSelected()
    {
        return _maxSelected;
    }


    #region GamePlayer

    public void UnitCreate(int whichPlayer, GameObject unitToCreate, Vector3 position)
    {
        _playersGameObjects[whichPlayer].Add(Instantiate(unitToCreate, position + new Vector3(3, 0, 3), unitToCreate.transform.rotation));
        _playersGameObjects[whichPlayer][_playersGameObjects[whichPlayer].Count-1].transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = _playersMaterials[whichPlayer].color;
    }


    #endregion
}
