using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SizeTypeSelectionPanel : MonoBehaviour
{
    public Text SizeMapText;
    public Slider[] Sliders;
    public Image TypeOfMapImage;
    public MapEditorManager MapEditorManager;
    public Button TypeTerrainButton;
    public Scrollbar typeTerrainScrollbar;

    private int _minSize = 100, _maxSize = 400;
    private int[] _sizes = new int[2];
    private int _groundId = 0;
    private List<Button> TypeTerrainButtons = new List<Button>();
    private List<ItemController> TypeTerrainLevel0 = new List<ItemController>();

    private void Start()
    {
        for (int i = 0; i < Sliders.Length; i++)
        {
            Sliders[i].minValue = _minSize;
            Sliders[i].maxValue = _maxSize;

            _sizes[i] = (int)Sliders[i].minValue;
        }

        TypeTerrainLevel0 = MapEditorManager.ItemControllers.Where(t => t.item.ItemHeightLevel == 0 &&
                t.item.GetComponent<ItemTerrain>() != null &&
                t.item.GetComponent<ItemTerrain>().AllowsBuild)
            .ToList();

        for (int i = 0; i < TypeTerrainLevel0.Count; i++)
        {
            TypeTerrainButtons.Add(Instantiate(TypeTerrainButton,
                new Vector3(
                     TypeTerrainButton.transform.position.x,
                     0,
                    0),
                TypeTerrainButton.transform.rotation));

            TypeTerrainButtons[i].transform.SetParent(TypeTerrainButton.GetComponentInParent<Transform>().parent.GetComponentInParent<Transform>());
            TypeTerrainButtons[i].transform.localScale = new Vector3(1, 1, 1);

            TypeTerrainButtons[i].GetComponent<Image>().sprite = TypeTerrainLevel0[i].GetComponent<Image>().sprite;
        }

        typeTerrainScrollbar.value = 1;
        TypeTerrainButton.gameObject.SetActive(false);

        UpdateMapImage();
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

        UpdateMapImage();
    }

    public void SetSizeSliders()
    {
        for (int i = 0; i < Sliders.Length; i++)
        {
            _sizes[i] = (int)Sliders[i].value;
        }

        UpdateMapImage();
    }

    public void SetMainGround()
    {
        Button actualClickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        _groundId = TypeTerrainButtons.IndexOf(actualClickedButton);

        UpdateMapImage();
    }

    public void AcceptParametersMap()
    {
        MapEditorManager.InitializeStartTerrain(_sizes[0], _sizes[1], TypeTerrainLevel0[_groundId].item.ID);
    }

    public void CancelParametersMap()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void UpdateMapImage()
    {
        Texture2D texture = new Texture2D(_maxSize, _maxSize);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, _maxSize, _maxSize), Vector2.zero);
        TypeOfMapImage.sprite = sprite;

        Color tmp = TypeTerrainLevel0[_groundId].item.ItemPrefab.GetComponent<MeshRenderer>().sharedMaterials[0].color;
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                texture.SetPixel(i, j, new Color(tmp.r, tmp.g, tmp.b, (i > (_maxSize - _sizes[0]) / 2 && i < _maxSize - (_maxSize - _sizes[0]) / 2 && j > (_maxSize - _sizes[1]) / 2 && j < _maxSize - (_maxSize - _sizes[1]) / 2) ? 1 : 0.3f));
            }
        }
        texture.Apply();
    }
}
