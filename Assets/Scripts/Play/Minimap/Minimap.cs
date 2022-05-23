using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    private Camera _minimapCamera;
    private RenderTexture _renderTexture;
    private GameManager _gameManager;
    private Image _image;

    private void Start()
    {
        _minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera").GetComponent<Camera>();
        _renderTexture = _minimapCamera.targetTexture;
        _gameManager = FindObjectOfType<GameManager>();
        _image = gameObject.GetComponentInParent<Image>();
    }

    private void Update()
    {
        UpdateUnitPixels();
    }

    private void UpdateUnitPixels()
    {
        Texture2D texture = GetRTPixels(_renderTexture);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        _image.sprite = sprite;

        if (_gameManager._playersGameObjects == null) UnityEngine.SceneManagement.SceneManager.LoadScene("Sirmish");

        for (int i = 0; i < _gameManager.Nature.Count; i++)
        {
            Color pixelColor =
            _gameManager.Nature[i].transform.GetComponentInChildren<MeshRenderer>().materials[0].color;

            texture.SetPixel((int)_gameManager.Nature[i].transform.position.x, (int)_gameManager.Nature[i].transform.position.z, pixelColor);
        }

        foreach (List<GameObject> objects in _gameManager._playersGameObjects)
        {
            if (objects.Count == 0) continue;

            Color pixelColor =
            objects[0].transform.GetComponentInChildren<MeshRenderer>() == null ?
            objects[0].transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color :
            objects[0].transform.GetComponentInChildren<MeshRenderer>().materials[1].color;


            foreach (GameObject obj in objects)
            {
                int size = obj.GetComponent<BuildingUnit>() == null ? 1 : obj.GetComponent<BuildingUnit>().Size;

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        texture.SetPixel((int)obj.transform.position.x - size / 2 + i, (int)obj.transform.position.z - size / 2 + j, pixelColor);
                    }
                }
            }
        }

        texture.Apply();
    }

    static public Texture2D GetRTPixels(RenderTexture rt)
    {
        RenderTexture activeRT = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        RenderTexture.active = activeRT;
        return tex;
    }
}
