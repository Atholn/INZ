using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public Camera MinimapCamera;

    private Texture2D _renderTexture;
    private GameManager _gameManager;

    public RenderTexture renderTexture;
    public Image image;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        UpdateUnitPixels();
    }

    private void UpdateUnitPixels()
    {
        Texture2D texture = GetRTPixels(renderTexture);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 100, 100), Vector2.zero);
        image.sprite = sprite;

        foreach (List<GameObject> objects in _gameManager._playersGameObjects)
        {
            if (objects.Count == 0) continue;

            Color pixelColor =
            objects[0].transform.GetComponent<MeshRenderer>() == null ?
            objects[0].transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color :
            objects[0].transform.GetComponent<MeshRenderer>().materials[1].color;

            foreach (GameObject obj in objects)
            {
                texture.SetPixel((int)obj.transform.position.x, (int)obj.transform.position.z, pixelColor);
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
