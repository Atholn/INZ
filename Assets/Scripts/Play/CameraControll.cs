using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraControll : MonoBehaviour
{
    public float Speed;
    public GameObject hpSliderPlayer;

    public ParticleSystem unitCircleRed;
    public ParticleSystem unitCircleGreen;
    private ParticleSystem tmpUnitCircleRed;
    private ParticleSystem tmpUnitCircleGreen;
    private List<ParticleSystem> tmpUnitCircles;
    static CameraControll cameraControl;
    new Camera camera;

    private Vector2 mousePos, mousePosScreen, _keyboardInput;

    private RectTransform _selectionBox;
    private Rect _selectionRect, _boxRect;

    private List<GameObject> _selectUnits = new List<GameObject>();
    private GameManager _gameManager;

    private bool _ifPlayerUnits = false;


    private const float _hpBarsShift = 0.001f;

    private void Awake()
    {
        cameraControl = this;
        camera = GetComponent<Camera>();
        if (GetComponentInChildren<Image>() != null)
        {
            _selectionBox = GetComponentInChildren<Image>().transform as RectTransform;
            _selectionBox.gameObject.SetActive(false);
        }
        _gameManager = GameObject.FindObjectOfType<GameManager>();

        hpSliderPlayer.transform.rotation = transform.rotation;

        hpSliderPlayer.SetActive(true);

        tmpUnitCircleRed = Instantiate(unitCircleRed, Vector3.zero, unitCircleRed.transform.rotation);
        tmpUnitCircleRed.gameObject.SetActive(false);
        tmpUnitCircleGreen = Instantiate(unitCircleGreen, Vector3.zero, unitCircleGreen.transform.rotation);
        tmpUnitCircleGreen.gameObject.SetActive(false);
        tmpUnitCircles = new List<ParticleSystem>();
    }

    void Update()
    {
        _keyboardInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        mousePos = Input.mousePosition;
        mousePosScreen = camera.ScreenToViewportPoint(mousePos);

        Vector2 movementDirection = _keyboardInput;

        var delta = new Vector3(movementDirection.x, 0, movementDirection.y);
        delta *= Speed * Time.deltaTime;
        transform.localPosition += delta;

        UpdateSelection();
        UpdateMouseDetection();
        UpdateGoldMineHint();

        UpdateCodes();
    }

    private void UpdateGoldMineHint()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out rayHit))
        {
            GoldMine goldMine = rayHit.collider.gameObject.GetComponent<GoldMine>();
            if (goldMine == null)
            {
                _gameManager.ShowGoldmineHint(-1);
                return;
            }

            _gameManager.ShowGoldmineHint(goldMine.GoldResource);
        }
    }

    private void UpdateMouseDetection()
    {
        if (!Input.GetMouseButton(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out rayHit))
            {
                if (rayHit.collider.gameObject.GetComponent<Unit>() != null)
                {
                    #region hpbar
                    hpSliderPlayer.SetActive(true);
                    hpSliderPlayer.transform.rotation = camera.transform.rotation;
                    hpSliderPlayer.transform.position = rayHit.collider.gameObject.transform.position + new Vector3(0, 5, 0);

                    Unit unit = rayHit.collider.gameObject.GetComponent<Unit>();

                    hpSliderPlayer.transform.localScale = new Vector3((float)unit.HpMax / 100, 1, 1);

                    RectTransform rectTransform = hpSliderPlayer.GetComponentsInChildren<RectTransform>()[1];
                    float hp = (float)unit.Hp / (float)unit.HpMax;
                    float posX = -0.5f + (hp / 2 - _hpBarsShift);

                    rectTransform.localScale = new Vector3(hp, 1, 1);
                    rectTransform.localPosition = new Vector3(Input.mousePosition.x < Screen.width / 2 ? -posX : posX, 0, 0);
                    #endregion

                    #region circles
                    if (_gameManager._playersGameObjects[0].Contains(rayHit.collider.gameObject))
                    {
                        UpdateCircleUnit(tmpUnitCircleGreen, rayHit.collider.gameObject);
                    }
                    else
                    {
                        UpdateCircleUnit(tmpUnitCircleRed, rayHit.collider.gameObject);
                    }
                    #endregion
                }
                else
                {
                    hpSliderPlayer.SetActive(false);

                    tmpUnitCircleRed.gameObject.SetActive(false);
                    tmpUnitCircleRed.transform.SetParent(null);
                    tmpUnitCircleGreen.gameObject.SetActive(false);
                    tmpUnitCircleGreen.transform.SetParent(null);
                }
            }
        }
    }

    private void UpdateCodes()
    {
        if (Input.GetMouseButtonDown(4))
        {
            _gameManager._players[0].actualWood += 1000;
            _gameManager._players[0].actualGold += 1000;
            _gameManager._players[1].actualWood += 1000;
            _gameManager._players[1].actualGold += 1000;
        }
    }

    private void UpdateSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _selectionBox.gameObject.SetActive(true);
            _selectionRect.position = mousePos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _selectionBox.gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(0))
        {
            _selectionRect.size = mousePos - _selectionRect.position;
            _boxRect = AbsRect(_selectionRect);
            _selectionBox.anchoredPosition = _boxRect.position;
            _selectionBox.sizeDelta = _boxRect.size;

            if (_boxRect.size.x != 0 || _boxRect.size.y != 0)
            {
                UpdateSelecting();
            }
        }
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (_gameManager.building && Input.GetMouseButtonDown(0))
        {
            if (_gameManager._players[0].actualGold < _gameManager.GameItemControllers[_gameManager.CurrentButtonPressed].item.GetComponent<Unit>().GoldCost ||
                _gameManager._players[0].actualWood < _gameManager.GameItemControllers[_gameManager.CurrentButtonPressed].item.GetComponent<Unit>().WoodCost)
            {
                _gameManager.DestroyItemImages();
                _gameManager.building = false;
                return;
            }
            _gameManager.UpdateWood(0, -_gameManager.GameItemControllers[_gameManager.CurrentButtonPressed].item.GetComponent<Unit>().WoodCost);
            _gameManager.UpdateGold(0, -_gameManager.GameItemControllers[_gameManager.CurrentButtonPressed].item.GetComponent<Unit>().GoldCost);

            GameObject building = (Instantiate(_gameManager.GameItemControllers[_gameManager.CurrentButtonPressed].item.ItemPrefab,
                new Vector3(_gameManager.v.x, -(_gameManager.GameItemControllers[_gameManager.CurrentButtonPressed].item as ItemGame).HeightBuilding, _gameManager.v.z),
                _gameManager.GameItemControllers[_gameManager.CurrentButtonPressed].item.ItemPrefab.transform.rotation));

            _gameManager._playersGameObjects[0].Add(building);
            _gameManager.DestroyItemImages();
            _gameManager.building = false;

            _gameManager._playersGameObjects[0][_gameManager._playersGameObjects[0].Count - 1].GetComponent<MeshRenderer>().materials[1].color = _gameManager._playersMaterials[0].color;
            _gameManager._playersGameObjects[0][_gameManager._playersGameObjects[0].Count - 1].GetComponent<Unit>().WhichPlayer = 0;

            GiveCommands(building, "Command");
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            GiveCommand();
        }
    }

    void GiveCommand()
    {
        if (_ifPlayerUnits)
        {
            if (_selectUnits.Count == 0)
            {
                return;
            }

            ray = camera.ViewportPointToRay(mousePosScreen);
            Physics.Raycast(ray, out rayHit, 1000, commandLayerMask);

            object commandData = null;

            foreach (GameObject gameObject in _selectUnits)
            {
                if (gameObject.GetComponent<BuildingUnit>() != null)
                {
                    gameObject.GetComponent<BuildingUnit>().PointerPosition = new Vector3(rayHit.point.x, 0.45f, rayHit.point.z);
                    _gameManager.Pointer.transform.position = new Vector3(rayHit.point.x, 0.45f, rayHit.point.z);
                    continue;
                }

                if (gameObject.GetComponent<Soldier>() != null)
                {
                    if (rayHit.collider is TerrainCollider)
                    {
                        commandData = rayHit.point;

                        GiveCommands(commandData, "Command");
                        continue;
                    }

                    int i = 0;
                    int k = -1;
                    for (i = 0; i < _gameManager._playersGameObjects.Count; i++)
                    {
                        if (_gameManager._playersGameObjects[i].Where(x => x == rayHit.collider.gameObject).FirstOrDefault() != null)
                        {
                            k = i;
                            break;
                        }
                    }

                    if (k == -1) continue;

                    commandData = rayHit.collider.gameObject;

                    if (k == 0)
                    {
                        GiveCommands(commandData, "CommandPlayer");
                        continue;
                    }

                    GiveCommands(commandData, "CommandEnemy");

                    continue;
                }

                if (gameObject.GetComponent<Worker>() != null)
                {
                    if (rayHit.collider.gameObject.GetComponent<Tree>() != null)
                    {
                        commandData = rayHit.collider.gameObject.GetComponent<Tree>();
                        GiveCommands(commandData, "Command");
                        continue;
                    }
                    if (rayHit.collider.gameObject.GetComponent<GoldMine>() != null)
                    {
                        commandData = rayHit.collider.gameObject.GetComponent<GoldMine>();
                        GiveCommands(commandData, "Command");
                        continue;
                    }

                    if (rayHit.collider is TerrainCollider)
                    {
                        commandData = rayHit.point;

                        GiveCommands(commandData, "Command");
                        continue;
                    }
                }
            }
        }
    }

    Ray ray;
    RaycastHit rayHit;
    public LayerMask commandLayerMask = -1;
    void GiveCommands()
    {
        ray = camera.ViewportPointToRay(mousePosScreen);
        if (Physics.Raycast(ray, out rayHit, 1000, commandLayerMask))
        {
            if (_selectUnits.Count == 0)
            {
                return;
            }

            object commandData = null;
            if (rayHit.collider is TerrainCollider)
            {
                commandData = rayHit.point;
            }

            else if (_selectUnits[0].GetComponent<Worker>() != null && rayHit.collider is BoxCollider)
            {
                if (rayHit.collider.gameObject.GetComponent<Tree>() != null)
                {
                    commandData = rayHit.collider.gameObject.GetComponent<Tree>();
                }

                if (rayHit.collider.gameObject.GetComponent<GoldMine>() != null)
                {
                    commandData = rayHit.collider.gameObject.GetComponent<GoldMine>();
                }

                if (rayHit.collider.gameObject.GetComponent<BuildingUnit>() != null)
                {
                    _selectUnits[0].GetComponent<NavMeshAgent>().velocity = Vector3.zero;

                    commandData = rayHit.collider.gameObject;
                }
            }
            else
            {
                commandData = rayHit.collider.gameObject.GetComponent<Unit>();
            }

            GiveCommands(commandData, "Command");
        }
    }

    void GiveCommands(object dataCommand, string commandName)
    {
        foreach (GameObject selectable in _selectUnits)
        {
            selectable.GetComponent<Unit>().SendMessage(commandName, dataCommand, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void UpdateSelecting()
    {
        _ifPlayerUnits = false;
        _selectUnits.Clear();
        DeleteCircles();

        int i;
        for (i = 0; i < _gameManager._playersGameObjects.Count; i++)
        {
            List<GameObject> objects = new List<GameObject>();
            for (int j = 0; j < _gameManager._playersGameObjects[i].Count && objects.Count < _gameManager.GetMaxSelected(); j++)
            {
                var position = _gameManager._playersGameObjects[i][j].transform.position;
                var positionScreen = camera.WorldToScreenPoint(position);
                bool inRect = IsPointInRect(_boxRect, positionScreen);

                if (inRect)
                {
                    if (TryAddGameObject(_gameManager._playersGameObjects[i][j], ref objects))
                    {
                        objects.Add(_gameManager._playersGameObjects[i][j]);
                    }
                }
            }

            objects.ForEach(o => _selectUnits.Add(o));
            CreateCircles();

            if (i == 0 && objects.Count != 0)
            {
                break;
            }
        }

        _ifPlayerUnits = i == 0 ? true : false;

        if (_selectUnits.Count == 0)
        {
            _gameManager.SetNonProfile();
            return;
        }

        if (_selectUnits.Count == 1)
        {
            _gameManager.SetProfile(_selectUnits[0], i);
            return;
        }

        if (_selectUnits.Count > 0)
        {
            if (i > 0)
            {
                return;
            }

            _gameManager.SetProfiles(_selectUnits);
            return;
        }
    }

    #region Circles units 
    private void UpdateCircleUnit(ParticleSystem unitCircle, GameObject gameObject)
    {
        unitCircle.gameObject.SetActive(true);
        unitCircle.transform.SetParent(gameObject.transform);

        unitCircle.transform.localPosition = new Vector3(0, 0, 0);
        unitCircle.transform.localScale = Vector3.one;

        #region Worker update
        Worker worker = gameObject.GetComponent<Worker>();
        if (worker != null)
        {
            unitCircle.transform.localPosition += new Vector3(0, 0, 2);
            return;
        }
        #endregion

        #region Soldier update
        Soldier soldier = gameObject.GetComponent<Soldier>();
        if (soldier != null)
        {
            unitCircle.transform.localPosition += new Vector3(0, -6, 0);
            return;
        }
        #endregion

        #region Building update
        BuildingUnit buildingUnit = gameObject.GetComponent<BuildingUnit>();
        if (buildingUnit != null)
        {
            unitCircle.transform.localScale *= buildingUnit.Size + 1;
            return;
        }
        #endregion
    }

    private void CreateCircles()
    {
        for (int i = 0; i < _selectUnits.Count; i++)
        {
            var tmpUnitCircle =
                _gameManager._playersGameObjects[0].Contains(_selectUnits[i]) ?
                unitCircleGreen :
                unitCircleRed;
            tmpUnitCircles.Add(Instantiate(tmpUnitCircle, Vector3.zero, unitCircleRed.transform.rotation));
            UpdateCircleUnit(tmpUnitCircles[i], _selectUnits[i]);
        }
    }

    private void DeleteCircles()
    {
        for (int i = 0; i < tmpUnitCircles.Count; i++)
        {
            Destroy(tmpUnitCircles[i]);
        }
        tmpUnitCircles.Clear();
    }
    #endregion

    private bool TryAddGameObject(GameObject gameObject, ref List<GameObject> objects)
    {
        Unit unit = gameObject.GetComponent<Unit>();

        if (unit is HumanUnit)
        {
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                Unit tmpUnit = objects[i].GetComponent<Unit>();
                if (tmpUnit is BuildingUnit)
                {
                    objects.Remove(objects[i]);
                }
            }

            return true;
        }

        if (unit is BuildingUnit)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                Unit tmpUnit = objects[i].GetComponent<Unit>();
                if (tmpUnit is HumanUnit)
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    bool IsPointInRect(Rect rect, Vector2 point)
    {
        return point.x >= rect.position.x && point.x <= (rect.position.x + rect.size.x) &&
            point.y >= rect.position.y && point.y <= (rect.position.y + rect.size.y);
    }

    private Rect AbsRect(Rect selectionRect)
    {
        if (selectionRect.width < 0)
        {
            selectionRect.x += selectionRect.width;
            selectionRect.width *= -1;
        }
        if (selectionRect.height < 0)
        {
            selectionRect.y += selectionRect.height;
            selectionRect.height *= -1;
        }
        return selectionRect;
    }
}
