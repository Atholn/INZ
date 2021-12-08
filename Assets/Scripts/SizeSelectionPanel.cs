using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SizeSelectionPanel : MonoBehaviour
{
    public Text SizeMapText;
    public Slider[] Sliders;
    public Image TypeOfMapImage;
    public MapEditorManager MapEditorManager;

    private int[] _sizes = new int[2];
    private int _groundId = 0;

    private void Start()
    {
        _sizes[0] = (int)Sliders[0].minValue;
        _sizes[1] = (int)Sliders[1].minValue;
    }

    private void Update()
    {
        SizeMapText.text = $"{_sizes[0]} x {_sizes[1]}";
    }

    public void SetSizeButtons(int size)
    {
        for (int i = 0; i < Sliders.Length; i++)
        {
            _sizes[i] = size;
            Sliders[i].value = size;
        }
    }

    public void SetSizeSliders()
    {
        for (int i = 0; i < Sliders.Length; i++)
        {
            _sizes[i] = (int)Sliders[i].value;
        }
    }

    public void SetMainGround(int id)
    {
        _groundId = id;



        //Texture2D texture = new Texture2D(_sizes[0], _sizes[1]);
        //Sprite sprite = Sprite.Create(texture, new Rect(0, 0, _sizes[0], _sizes[1]), Vector2.zero);
        //TypeOfMapImage.sprite = sprite;

        //for (int i = 0; i < texture.height; i++)
        //{
        //    for (int j = 0; j < texture.width; j++)
        //    {
        //        Color pixelColour = new Color(mapViewColors[i][j][0], mapViewColors[i][j][1], mapViewColors[i][j][2], 1);
        //        texture.SetPixel(i, j, pixelColour);
        //    }
        //}
        //texture.Apply();
    }

    public void AcceptParametersMap()
    {
        MapEditorManager.InitializeStartTerrain(_sizes[0], _sizes[1], _groundId);
    }   

    public void CancelParametersMap()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
