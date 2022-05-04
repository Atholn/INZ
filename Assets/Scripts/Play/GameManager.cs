using System.Collections.Generic;
using System.Linq;
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
    internal List<Player> _players;

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

    private RaycastHit hit;
    internal Vector3 v;
    public NavMeshBaker navMeshBaker;

    internal List<GameObject> Nature = new List<GameObject>();

    public GameObject Pointer;

    private readonly int _maxUnitsPoint = 100;

    //internal int[][][] maps;

    public GameObject Fire;
    public GameObject Dust;

    public float UpgradeFactor = 1.2f;

    private Dictionary<string, Dictionary<string, string>> winRequarieds;
    private string sceneToBack;

    void Start()
    {
        _gameUI = FindObjectOfType<GameUI>();

        _map = new MapWorld();
        _gameStartPoints = MapToPlayStorage.GameStartPoints;
        winRequarieds = MapToPlayStorage.WinRequarieds;
        sceneToBack = MapToPlayStorage.SceneToBack;

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

        Pointer.SetActive(true);
        Pointer.GetComponentInChildren<SkinnedMeshRenderer>().material = _playersMaterials[0];

        CheckWinLose();

        ///
        _gameUI.UpdateRawMaterials(0, 0);
        _gameUI.UpdateRawMaterials(1, 0);
        UpdateUnitPoints(0);
    }

    internal void UpdateUnitPoints(int whichPlayer)
    {
        _players[whichPlayer].actualUnitsPoint = UnitsPointsUpdate(whichPlayer);
        _players[whichPlayer].actualMaxUnitsPoint = UnitsMaxPointsUpdate(whichPlayer);

        if (whichPlayer == 0)
            _gameUI.UpdateRawMaterials(2, _players[whichPlayer].actualUnitsPoint, _players[whichPlayer].actualMaxUnitsPoint);
    }

    public int UnitsPointsUpdate(int whichPlayer)
    {
        var points = _playersGameObjects[whichPlayer]
            .Where(g => g.GetComponent<HumanUnit>() != null && g.GetComponent<HumanUnit>().Hp > 0)
            .Select(g => g.GetComponent<HumanUnit>().UnitPoint)
            .Sum();
        return _players[whichPlayer].actualUnitsPoint = points;
    }

    internal int UnitsMaxPointsUpdate(int whichPlayer)
    {
        var points = _playersGameObjects[whichPlayer]
            .Where(g => g.GetComponent<BuildingUnit>() != null && g.GetComponent<BuildingUnit>().BuildingPercent > g.GetComponent<BuildingUnit>().CreateTime)
            .Select(g => g.GetComponent<BuildingUnit>().UnitToCreatePoints)
            .Sum();
        return _players[whichPlayer].actualMaxUnitsPoint = points < _maxUnitsPoint ? points : _maxUnitsPoint;
    }

    internal void UpdateWood(int whichPlayer, int woodCount)
    {
        _players[whichPlayer].actualWood += woodCount;
        if (whichPlayer == 0)
            _gameUI.UpdateRawMaterials(1, _players[0].actualWood);
    }

    internal void UpdateGold(int whichPlayer, int goldCount)
    {
        _players[whichPlayer].actualGold += goldCount;
        if (whichPlayer == 0)
            _gameUI.UpdateRawMaterials(0, _players[0].actualGold);
    }

    internal void UpgradeUnit(int whichPlayer, int whichUnit)
    {
        _players[whichPlayer].upgradeFactor[0] *= UpgradeFactor;
    }

    private void InitializePlayers()
    {
        _playersGameObjects = new List<List<GameObject>>();
        _playersMaterials = new List<Material>();
        _players = new List<Player>();

        for (int i = 0; i < _gameStartPoints.Count; i++)
        {
            _playersGameObjects.Add(new List<GameObject>());
            _playersMaterials.Add(_gameStartPoints[i].UnitMaterial);

            if (i == 0) _players.Add(new Player { typeOfPlayer = Player.TypeOfPlayer.human });
            else _players.Add(new Player { typeOfPlayer = Player.TypeOfPlayer.computer, whichPlayer = i });
        }

        for (int i = 0; i < _gameStartPoints.Count; i++)
        {
            _playersGameObjects[i].Add(Instantiate(_townHall, _gameStartPoints[i].UnitStartLocation, _townHall.transform.rotation));
            _playersGameObjects[i][0].transform.GetComponent<MeshRenderer>().materials[1].color = _playersMaterials[i].color;
            _playersGameObjects[i][0].GetComponent<BuildingUnit>().BuildingPercent = _playersGameObjects[i][0].GetComponent<BuildingUnit>().CreateTime + 0.01f;
            _playersGameObjects[i][0].GetComponent<Unit>().WhichPlayer = i;
            _playersGameObjects[i][0].GetComponent<BuildingUnit>().PointerPosition = new Vector3(_playersGameObjects[i][0].transform.position.x, 0.45f, _playersGameObjects[i][0].transform.position.z - (_playersGameObjects[i][0].GetComponent<BuildingUnit>().SizeBuilding / 2) - 1);

            for (int j = 0; j < _countOfWorkers; j++)
            {
                _playersGameObjects[i].Add(Instantiate(_worker, _gameStartPoints[i].UnitStartLocation + new Vector3(5 + j * 1, 0, 5), _worker.transform.rotation));
                _playersGameObjects[i][j + 1].transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = _playersMaterials[i].color;
                _playersGameObjects[i][j + 1].GetComponent<Unit>().WhichPlayer = i;
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

        if (Input.GetMouseButtonDown(2))
        {
            foreach (List<GameObject> gos in _playersGameObjects)
            {
                string playerObjects = "";
                foreach (GameObject go in gos)
                {
                    playerObjects += $"{go.name} ";
                }
                Debug.Log(playerObjects);
            }
        }

        if (onlyOneSelectGO != null)
        {
            _gameUI.ShowProgressCreateUnitPanel(onlyOneSelectGO);
        }

        foreach (Player player in _players)
        {
            if (player.typeOfPlayer == Player.TypeOfPlayer.human)
            {
                continue;
            }

            player.UpdateComputer(_playersGameObjects[player.whichPlayer]);
        }

        UpdateWinRequaired();
    }

    private void UpdateWinRequaired()
    {
        bool ifWin = true;

        foreach (KeyValuePair<string, Dictionary<string, string>> winRequaried in winRequarieds)
        {

            switch (winRequaried.Key)
            {
                case "free": ifWin = false; break;
                case "dominate": DominateCheckWin(ref ifWin); break;
                case "wood": break;
                case "gold": break;
                case "units": break;
                case "upgrades": break;
                case "soldiers": break;
                case "soldiersType": break;
                case "time": break;
                case "attacks": break;
            }


        }

        if(ifWin)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToBack);
        }
    }

    #region ChecksWin

    private void DominateCheckWin(ref bool ifWin)
    {
        int winner = -1;
        for (int i = 0; i < _playersGameObjects.Count; i++)
        {
            if (_playersGameObjects[i].Count > 0)
            {
                if (winner != -1)
                {
                    ifWin = false;
                    return;
                }

                winner = i;
            }
        }
    }

    #endregion

    internal void CheckWinLose()
    {
        int winner = -1;
        for (int i = 0; i < _playersGameObjects.Count; i++)
        {
            if (_playersGameObjects[i].Count > 0)
            {
                if (winner != -1)
                {
                    return;
                }

                winner = i;
            }
        }

        _gameUI.ShowWinner(winner, _playersMaterials[winner].color);
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
        actualClickBuild = null;

        _gameUI.SetNonProfile();

        onlyOneSelectGO = null;

        Pointer.SetActive(false);
    }

    public void SetProfile(GameObject gameObject, int i)
    {
        _gameUI.SetCharacterInfo(gameObject, i);

        if (gameObject.GetComponent<BuildingUnit>() != null)
        {
            actualClickBuild = gameObject.GetComponent<BuildingUnit>();
            if (gameObject.GetComponent<BuildingUnit>().CanCreateUnit)
            {
                Pointer.SetActive(true);
                Pointer.transform.position = gameObject.GetComponent<BuildingUnit>().PointerPosition;
            }
        }
        onlyOneSelectGO = gameObject;
    }

    internal void SetProfiles(List<GameObject> selectUnits)
    {
        selectUnits = selectUnits.OrderBy(x => x.GetComponent<Unit>().Priority).ToList();
        _gameUI.SetCharactersProfiles(selectUnits, _maxSelected);

        actualClickBuild = null;
        onlyOneSelectGO = null;

        Pointer.SetActive(false);
    }

    internal int GetMaxSelected()
    {
        return _maxSelected;
    }

    #region GamePlayer
    public void UnitCreate(int whichPlayer, GameObject unitToCreate, Vector3 position, Vector3 pointerPosition)
    {
        _playersGameObjects[whichPlayer].Add(Instantiate(unitToCreate, position, unitToCreate.transform.rotation));
        _playersGameObjects[whichPlayer][_playersGameObjects[whichPlayer].Count - 1].transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[unitToCreate.GetComponent<HumanUnit>().UnitColorNum].color = _playersMaterials[whichPlayer].color;
        _playersGameObjects[whichPlayer][_playersGameObjects[whichPlayer].Count - 1].GetComponent<Unit>().WhichPlayer = whichPlayer;

        if (_playersGameObjects[whichPlayer][_playersGameObjects[whichPlayer].Count - 1].GetComponent<HumanUnit>() != null)
        {
            if (_playersGameObjects[whichPlayer][_playersGameObjects[whichPlayer].Count - 1].GetComponent<Worker>() != null)
            {
                Worker w = _playersGameObjects[whichPlayer][_playersGameObjects[whichPlayer].Count - 1].GetComponent<Worker>();
                w.SendMessage("Command", pointerPosition, SendMessageOptions.DontRequireReceiver);
            }

            if (_playersGameObjects[whichPlayer][_playersGameObjects[whichPlayer].Count - 1].GetComponent<Soldier>() != null)
            {
                Soldier s = _playersGameObjects[whichPlayer][_playersGameObjects[whichPlayer].Count - 1].GetComponent<Soldier>();
                s.SendMessage("Command", pointerPosition, SendMessageOptions.DontRequireReceiver);
            }
        }

        _gameUI.UpdateRawMaterials(2, UnitsPointsUpdate(whichPlayer), UnitsMaxPointsUpdate(whichPlayer));
    }


    #endregion
}
