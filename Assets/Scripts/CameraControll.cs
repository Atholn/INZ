using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraControll : MonoBehaviour
{
    public float Speed;

    static CameraControll cameraControl;
    new Camera camera;

    private Vector2 mousePos, mousePosScreen, _keyboardInput;

    private RectTransform _selectionBox;
    private Rect _selectionRect, _boxRect;

    private List<GameObject> _selectUnits = new List<GameObject>();
    private GameManager _gameManager;

    private bool _ifPlayerUnits = false;

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
    }

    void Update()
    {
        _keyboardInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        mousePos = Input.mousePosition;
        if(_gameManager !=null)mousePosScreen = camera.ScreenToViewportPoint(mousePos);

        Vector2 movementDirection = _keyboardInput;

        var delta = new Vector3(movementDirection.x, 0, movementDirection.y);
        delta *= Speed * Time.deltaTime;
        transform.localPosition += delta;

        if (_gameManager != null) UpdateSelection();
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

        if (EventSystem.current.IsPointerOverGameObject()) return;

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

        if (Input.GetMouseButtonDown(0))
        {
            if (_ifPlayerUnits)
            {
                GiveCommands();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            _selectUnits.Clear();
            _gameManager.SetNonProfile();
        }

        if (_gameManager.building && Input.GetMouseButtonDown(0))
        {
            GameObject building = (Instantiate(_gameManager.ItemControllers[_gameManager.CurrentButtonPressed].item.ItemPrefab,
                new Vector3(_gameManager.v.x, -(_gameManager.ItemControllers[_gameManager.CurrentButtonPressed].item as ItemGame).HeightBuilding, _gameManager.v.z),
                _gameManager.ItemControllers[_gameManager.CurrentButtonPressed].item.ItemPrefab.transform.rotation));

            _gameManager._playersGameObjects[0].Add(building);
            _gameManager.DestroyItemImages();
            _gameManager.building = false;

            _gameManager._playersGameObjects[0][_gameManager._playersGameObjects[0].Count-1].GetComponent<MeshRenderer>().materials[1].color = _gameManager._playersMaterials[0].color;

            GiveCommands(building, "Command");
        }

        //if (Input.GetMouseButtonDown(2))
        //{
        //    foreach (GameObject obj in _selectUnits)
        //    {
        //        Debug.LogError(obj.name);
        //    }
        //}
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

                if (rayHit.collider.gameObject.GetComponent<BuildingUnit>() != null)
                {
                    BuildingUnit buildingUnittt = rayHit.collider.gameObject.GetComponent<BuildingUnit>();

                    _selectUnits[0].GetComponent<NavMeshAgent>().velocity = Vector3.zero;

                    commandData = rayHit.collider.gameObject;

                }
            }
            else
            {
                commandData = rayHit.collider.gameObject.GetComponent<Unit>();
                // todo follow
                //return;
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

            foreach (GameObject gameObject in objects)
            {
                _selectUnits.Add(gameObject);
            }

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
                //todo show health 
                return;
            }

            _gameManager.SetProfiles(_selectUnits);
            return;
        }
    }

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
