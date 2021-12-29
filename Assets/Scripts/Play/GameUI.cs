using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject TopPanel;
    public GameObject MenuPanel;
    public GameObject BottomPanel;

    public GameObject OneCharacterPanel;
    public GameObject ManyCharactersPanel;

    private RawImage[] _characterImages;
    private Text[] _characterInfos;
    private List<RawImage> _images = new List<RawImage>();

    #region TopPanel
    public void Menu()
    {
        MenuPanel.SetActive(true);
        TopPanel.SetActive(false);
        BottomPanel.SetActive(false);
    }
    #endregion

    #region MenuPanel

    public void Save()
    {

    }

    public void Load()
    {

    }

    public void Exit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void Back()
    {
        MenuPanel.SetActive(false);
        TopPanel.SetActive(true);
        BottomPanel.SetActive(true);
    }
    #endregion

    #region BottomPanel

    public void SetLookBottomPanel(Color color)
    {
        BottomPanel.SetActive(true);
        BottomPanel.GetComponent<Image>().color = color;
        OneCharacterPanel.SetActive(false);
        ManyCharactersPanel.SetActive(false);

        _characterImages = OneCharacterPanel.GetComponentsInChildren<RawImage>();
        _characterInfos = OneCharacterPanel.GetComponentsInChildren<Text>();

        foreach (RawImage image in _characterImages)
        {
            image.color = color;
        }

        Color textColor = color == new Color(0, 0, 0) ? new Color(1, 1, 1) : new Color(0, 0, 0);
        foreach (Text text in _characterInfos)
        {
            text.color = textColor;
            text.text = "";
        }

        _images.Add(ManyCharactersPanel.GetComponentInChildren<RawImage>(true));
    }

    internal void SetNonProfile()
    {
        OneCharacterPanel.SetActive(false);
        ManyCharactersPanel.SetActive(false);
    }

    internal void SetCharacterInfo(GameObject gameObject)
    {
        OneCharacterPanel.SetActive(true);
        ManyCharactersPanel.SetActive(false);

        _characterImages[0].color =
            gameObject.transform.GetComponent<MeshRenderer>() == null ?
            gameObject.transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color :
            gameObject.transform.GetComponent<MeshRenderer>().materials[1].color;
        _characterImages[1].texture = gameObject.GetComponent<Unit>().Profile;
        _characterImages[1].color = new Color(255, 255, 255);

        Unit unit = gameObject.GetComponent<Unit>();
        _characterInfos[0].text = unit.Name;
        _characterInfos[1].text = $"Attack {unit.Attack}";
        _characterInfos[2].text = $"Defense {unit.Defense}";
        _characterInfos[3].text = $"{unit.Hp} / {unit.Hp}";
    }

    internal void SetCharactersProfiles(List<GameObject> selectUnits, int maxSelected)
    {
        OneCharacterPanel.SetActive(false);
        ManyCharactersPanel.SetActive(true);

        for (int i = _images.Count - 1; i > 0; i--)
        {
            Destroy(_images[i].gameObject);
            _images.Remove(_images[i]);
        }

        _images[0].texture = selectUnits[0].GetComponent<Unit>().Profile;

        for (int i = 0; i < selectUnits.Count; i++)
        {
            _images.Add(Instantiate(_images[0],
                new Vector3(
                    _images[0].transform.position.x + (((i + 1) % (maxSelected / 2)) * (_images[0].GetComponent<RectTransform>().rect.width + 10)),
                    i + 1 > (maxSelected / 2) - 1 ? (int)_images[0].transform.position.y - (_images[0].GetComponent<RectTransform>().rect.height + 10) : (int)_images[0].transform.position.y,
                    0),
                _images[0].transform.rotation));
            _images[i].transform.SetParent(ManyCharactersPanel.transform);
            _images[i].texture = selectUnits[i].GetComponent<Unit>().Profile;
        }
    }

    #endregion
}
