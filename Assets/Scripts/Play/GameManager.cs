using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public GameItemController[] GameItemControllers;
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

    public GameObject FirePrefab;
    public GameObject DustPrefab;
    public GameObject BuildingPlacePrefab;
    internal float buildingPlaceHeigh = 0.75f;

    public ParticleSystem unitCircleGreen;
    public ParticleSystem unitCircleRed;
    private ParticleSystem tmpUnitCircleGreen; //0
    private ParticleSystem tmpUnitCircleRed; //1
    private List<ParticleSystem> tmpUnitCircles;

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

        for (int i = 0; i < GameItemControllers.Length; i++)
        {
            GameItemControllers[i].item.ID = i;
        }

        Pointer.SetActive(true);
        Pointer.GetComponentInChildren<SkinnedMeshRenderer>().material = _playersMaterials[0];

        _gameUI.UpdateRawMaterials(0, 0);
        _gameUI.UpdateRawMaterials(1, 0);
        UpdateUnitPoints(0);

        InitializeCircles();
    }

    private void InitializeCircles()
    {
        tmpUnitCircleGreen = Instantiate(unitCircleGreen, Vector3.zero, unitCircleGreen.transform.rotation);
        tmpUnitCircleGreen.gameObject.SetActive(false);
        tmpUnitCircleRed = Instantiate(unitCircleRed, Vector3.zero, unitCircleRed.transform.rotation);
        tmpUnitCircleRed.gameObject.SetActive(false);
        tmpUnitCircles = new List<ParticleSystem>();
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

            _players.Add(new Player());
            _players[i].typeOfPlayer = i == 0 ? Player.TypeOfPlayer.human : Player.TypeOfPlayer.computer;

            int soldierTypesCount = UnitsPrefabs
                .Where(u => u.GetComponent<Soldier>() != null)
                .ToList().Count;
            _players[i].upgradeFactors = new float[soldierTypesCount];
            for (int j = 0; j < _players[i].upgradeFactors.Length; j++)
            {
                _players[i].upgradeFactors[j] = 1;
            }

            _players[i].whichPlayer = i;
        }

        for (int i = 0; i < _gameStartPoints.Count; i++)
        {
            _playersGameObjects[i].Add(Instantiate(_townHall, new Vector3(_gameStartPoints[i].UnitStartLocation.x, _townHall.GetComponent<ItemGame>().ItemHeightPosY, _gameStartPoints[i].UnitStartLocation.z), _townHall.transform.rotation));
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

            UpdateUnitPoints(i);
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

            foreach (var x in _players[0].upgradeFactors)
            {
                Debug.Log("upgrade " + x);
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

    #region Update sources
    internal void UpdateUnitPoints(int whichPlayer)
    {
        _players[whichPlayer].actualUnitsPoint = UnitsPointsUpdate(whichPlayer);
        _players[whichPlayer].actualMaxUnitsPoint = UnitsMaxPointsUpdate(whichPlayer);

        if (whichPlayer == 0)
            _gameUI.UpdateRawMaterials(2, _players[whichPlayer].actualUnitsPoint, _players[whichPlayer].actualMaxUnitsPoint);
    }

    internal int UnitsPointsUpdate(int whichPlayer)
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
        _players[whichPlayer].upgradeFactors[whichUnit] = UpgradeFactor;

        var name = UnitsPrefabs[whichUnit + 1].GetComponent<Soldier>().Name;

        var soldiersOneType = _playersGameObjects[whichPlayer]
            .Where(u => u.GetComponent<Unit>().Name == name)
            .ToList();

        foreach (var x in soldiersOneType)
        {
            var unit = x.GetComponent<Unit>();

            unit.AttackPower = (int)((float)(unit.AttackPower) * UpgradeFactor);
        }
    }
    #endregion

    #region ChecksWin
    private void UpdateWinRequaired()
    {
        bool ifWin = true;

        foreach (var winRequaried in winRequarieds)
        {
            switch (winRequaried.Key)
            {
                case "free": FreeCheckWin(ref ifWin, winRequaried.Value); break;
                case "dominate": DominateCheckWin(ref ifWin, winRequaried.Value); break;
                case "sources": SourcesCheckWin(ref ifWin, winRequaried.Value); break;
                case "upgrades": UpgradesCheckWin(ref ifWin, winRequaried.Value); break;
                case "soldiers": SoldiersCheckWin(ref ifWin, winRequaried.Value); break;
                case "buldings": BuldingsCheckWin(ref ifWin, winRequaried.Value); break;
            }
        }

        if (ifWin)
        {
            if (sceneToBack == "Campaign")
            {
                CampaignStorage.Win = true;
            }
            ExitToBackScene();
        }
    }

    private void FreeCheckWin(ref bool ifWin, Dictionary<string, string> addional)
    {
        ifWin = false;
    }

    private void DominateCheckWin(ref bool ifWin, Dictionary<string, string> addional)
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

        //todo if we want lose
    }

    private void SourcesCheckWin(ref bool ifWin, Dictionary<string, string> addionals)
    {
        foreach (var addional in addionals)
        {
            switch (addional.Key)
            {
                case "0": if (_players[0].actualWood + _players[0].actualGold < int.Parse(addional.Value)) ifWin = false; break;
                case "1": if (_players[0].actualGold < int.Parse(addional.Value)) ifWin = false; break;
                case "2": if (_players[0].actualWood < int.Parse(addional.Value)) ifWin = false; break;
                case "3": if (_players[0].actualUnitsPoint < int.Parse(addional.Value)) ifWin = false; break;
                case "4": if (_players[0].actualMaxUnitsPoint < int.Parse(addional.Value)) ifWin = false; break;
            }

            if (!ifWin)
                return;
        }

    }

    private void UpgradesCheckWin(ref bool ifWin, Dictionary<string, string> addional)
    {
        if (addional.First().Key != "0")
        {
            ifWin = CheckUpgrade(int.Parse(addional.First().Key) - 1, bool.Parse(addional.First().Value));
            return;
        }

        for (int i = 0; i < _players[0].upgradeFactors.Length; i++)
        {
            ifWin = CheckUpgrade(i, bool.Parse(addional.First().Value));
            if (!ifWin)
                return;
        }
    }

    private bool CheckUpgrade(int numberOfUbgrade, bool ifUpgrade)
    {
        if (_players[0].upgradeFactors[numberOfUbgrade] == UpgradeFactor)
        {
            if (ifUpgrade) return true;
            return false;
        }

        if (ifUpgrade) return false;
        return true;
    }

    private void SoldiersCheckWin(ref bool ifWin, Dictionary<string, string> addionals)
    {
        foreach (var addional in addionals)
        {
            int coutOfSoldiers;
            //Debug.LogError(int.Parse(addional.Key) + " " + addional.Key + " " + addional.Value);
            if (addional.Key == "0")
            {
                coutOfSoldiers = _playersGameObjects[0].Where(u => u.GetComponent<Soldier>() != null).Count();
            }
            else
            {  
                string nameOfUnit = UnitsPrefabs[int.Parse(addional.Key)].GetComponent<Unit>().Name;
                coutOfSoldiers = _playersGameObjects[0].Where(u => u.GetComponent<Unit>().Name == nameOfUnit).Count();
            }

            ifWin = coutOfSoldiers >= int.Parse(addional.Value);
            if (!ifWin)
                return;
        }
    }

    private void BuldingsCheckWin(ref bool ifWin, Dictionary<string, string> values)
    {
        foreach (var value in values)
        {
            if (value.Key == "0")
            {
                for (int i = 0; i < BuildingsPrefabs.Count && ifWin; i++)
                {
                    CheckBuild(ref ifWin, i, int.Parse(value.Value));
                }
                continue;
            }

            CheckBuild(ref ifWin, int.Parse(value.Key) - 1, int.Parse(value.Value));
        }
    }

    private void CheckBuild(ref bool ifWin, int numberOfBuilding, int countOfBuilidng)
    {
        var building = _playersGameObjects[0]
            .Where(b => b.GetComponent<BuildingUnit>() != null)
            .Select(b => b.GetComponent<BuildingUnit>())
            .Where(b => b.Name == BuildingsPrefabs[numberOfBuilding].GetComponent<BuildingUnit>().Name && b.BuildingPercent >= b.CreateTime)
            .ToList();

        if (building == null)
        {
            ifWin = false;
            return;
        }

        ifWin = building.Count >= countOfBuilidng;
    }

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

    internal void ExitToBackScene()
    {
        MapToPlayStorage.WinRequarieds = new Dictionary<string, Dictionary<string, string>>();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToBack);
    }

    #endregion

    internal void DestroyItemImages()
    {
        for (int i = 0; i < GameItemControllers.Length; i++)
        {
            GameItemControllers[i].Clicked = false;
            if (GameItemControllers[i].item.ItemHeightLevel == 0)
            {
                GameItemControllers[i].item.ItemImage.transform.localScale = GameItemControllers[i].firstScale;
            }
        }

        GameObject[] itemImages = GameObject.FindGameObjectsWithTag("ItemImage");

        for (int i = 0; i < itemImages.Length; i++)
        {
            Destroy(itemImages[i]);
        }
    }

    #region Profiles
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
    #endregion

    internal int GetMaxSelected()
        => _maxSelected;

    #region GamePlayer
    public void UnitCreate(int whichPlayer, GameObject unitToCreate, Vector3 position, Vector3 pointerPosition)
    {
        _playersGameObjects[whichPlayer].Add(Instantiate(unitToCreate, position, unitToCreate.transform.rotation));
        _playersGameObjects[whichPlayer][_playersGameObjects[whichPlayer].Count - 1].transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[unitToCreate.GetComponent<HumanUnit>().UnitColorNum].color = _playersMaterials[whichPlayer].color;
        _playersGameObjects[whichPlayer][_playersGameObjects[whichPlayer].Count - 1].GetComponent<Unit>().WhichPlayer = whichPlayer;

        UpdateUnitPoints(whichPlayer);

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

                var index = UnitsPrefabs.IndexOf(unitToCreate) - 1;
                var factor = _players[whichPlayer].upgradeFactors[index];

                s.AttackPower = (int)((float)(s.AttackPower) * factor);
            }
        }
    }
    #endregion

    #region Show hints/errors
    internal void ShowHints(string[] texts)
    {
        _gameUI.ShowHints(texts);
    }

    internal void ShowErrors(bool[] errors)
    {
        _gameUI.ShowErrors(errors);
    }

    internal void ShowGoldmineHint(float goldmineSource)
    {
        _gameUI.ShowGoldmineHint(goldmineSource);
    }
    #endregion

    internal GameObject CheckNearEnemy(int player, Vector3 shooterPosition)
    {
        float nearDistance = -1f;
        GameObject nearEnemy = null;

        for (int i = 0; i < _playersGameObjects.Count; i++)
        {
            if (i == player)
            {
                continue;
            }

            for (int j = 0; j < _playersGameObjects[i].Count; j++)
            {
                var tmpDistance = Vector3.Distance(shooterPosition, _playersGameObjects[i][j].transform.position);
                if (nearEnemy == null || tmpDistance < nearDistance)
                {
                    nearDistance = tmpDistance;
                    nearEnemy = _playersGameObjects[i][j];
                }
            }
        }

        return nearEnemy;
    }

    internal int[] GetSizeMap()
        => new int[2] { _map.SizeMapX, _map.SizeMapY };

    internal void GenerateBuildingPlaceAndDust(GameObject building, float x, float z)
    {
        GameObject buildingPlace = Instantiate(BuildingPlacePrefab,
                        new Vector3(x, buildingPlaceHeigh, z),
                        BuildingPlacePrefab.transform.rotation);
        var buildingUnit = building.GetComponent<BuildingUnit>();
        var size = buildingUnit.Size;

        buildingPlace.transform.localScale =
            new Vector3(size * buildingPlace.transform.localScale.x, size * buildingPlace.transform.localScale.y, buildingPlace.transform.localScale.z);

        var dust = buildingPlace.GetComponentInChildren<ParticleSystem>().gameObject;
        dust.transform.localScale *= size;
        dust.SetActive(false);

        buildingUnit.buildingPlace = buildingPlace;
        buildingUnit.dust = dust;
        buildingPlace.GetComponent<BuildingPlace>().Building = building;
    }


    #region Circles units 
    internal void UpdateCircleUnitFromCamera(int which, GameObject gameObject)
        => UpdateCircleUnit(which == 0 ? tmpUnitCircleGreen : tmpUnitCircleRed, gameObject);

    internal void HideTmpCircle(int which)
        => HideCircle(which == 0 ? tmpUnitCircleGreen : tmpUnitCircleRed);

    internal void CreateCircles(List<GameObject> _selectUnits)
    {
        for (int i = 0; i < _selectUnits.Count; i++)
        {
            var tmpUnitCircle =
                _playersGameObjects[0].Contains(_selectUnits[i]) ?
                unitCircleGreen :
                unitCircleRed;
            tmpUnitCircles.Add(Instantiate(tmpUnitCircle, Vector3.zero, unitCircleRed.transform.rotation));
            UpdateCircleUnit(tmpUnitCircles[i], _selectUnits[i]);
        }
    }

    internal void DeleteCircles()
    {
        for (int i = 0; i < tmpUnitCircles.Count; i++)
        {
            Destroy(tmpUnitCircles[i]);
        }
        tmpUnitCircles.Clear();
    }

    internal void SetNullParentInCircles(GameObject parentGameObject)
    {
        if (tmpUnitCircleGreen.transform.IsChildOf(parentGameObject.transform))
        {
            HideCircle(tmpUnitCircleGreen);
        }

        if (tmpUnitCircleRed.transform.IsChildOf(parentGameObject.transform))
        {
            HideCircle(tmpUnitCircleRed);
        }
    }

    private void UpdateCircleUnit(ParticleSystem unitCircle, GameObject gameObjectWithCircle)
    {
        unitCircle.gameObject.SetActive(true);
        unitCircle.transform.SetParent(gameObjectWithCircle.transform);

        unitCircle.transform.localPosition = new Vector3(0, 0, 0);
        unitCircle.transform.localScale = Vector3.one;

        #region Worker update
        Worker worker = gameObjectWithCircle.GetComponent<Worker>();
        if (worker != null)
        {
            unitCircle.transform.localPosition += new Vector3(0, 0, 2);
            return;
        }
        #endregion

        #region Soldier update
        Soldier soldier = gameObjectWithCircle.GetComponent<Soldier>();
        if (soldier != null)
        {
            unitCircle.transform.localPosition += new Vector3(0, -6, 0);
            return;
        }
        #endregion

        #region Building update
        BuildingUnit buildingUnit = gameObjectWithCircle.GetComponent<BuildingUnit>();
        if (buildingUnit != null)
        {
            unitCircle.transform.localScale *= buildingUnit.Size + 1;
            return;
        }
        #endregion

        #region
        BuildingPlace buildingPlace = gameObjectWithCircle.GetComponent<BuildingPlace>();
        if (buildingPlace != null)
        {
            unitCircle.transform.localScale *= buildingPlace.Building.GetComponent<BuildingUnit>().Size + 1;
            return;
        }
        #endregion
    }

    private void HideCircle(ParticleSystem circle)
    {
        circle.gameObject.SetActive(false);
        circle.transform.SetParent(null);
    }
    #endregion

}
