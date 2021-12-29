using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void Awake()
    {
        cameraControl = this;
        _selectionBox = GetComponentInChildren<Image>().transform as RectTransform;
        camera = GetComponent<Camera>();
        _selectionBox.gameObject.SetActive(false);

        _gameManager = GameObject.FindObjectOfType<GameManager>();
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
                UpdateSelecting();
        }

        if (Input.GetMouseButtonDown(2))
        {
            foreach (GameObject obj in _selectUnits)
            {
                Debug.LogError(obj.name);
            }
        }
    }



    private void UpdateSelecting()
    {
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
                    objects.Add(_gameManager._playersGameObjects[i][j]);
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

        if (_selectUnits.Count == 0)
        {
            _gameManager.SetNonProfile();
            return;
        }

        if (_selectUnits.Count == 1)
        {
            _gameManager.SetProfile(_selectUnits[0]);
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
