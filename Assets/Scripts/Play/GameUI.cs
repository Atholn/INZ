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
    public GameObject CharacterPanel;

    private Image _mainCharacterImage;
    private RawImage[] _characterImages;
    private Text[] _characterInfos;

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
        _mainCharacterImage = CharacterPanel.GetComponent<Image>();
        _mainCharacterImage.color = color;
        _characterImages = CharacterPanel.GetComponentsInChildren<RawImage>();
        _characterInfos = CharacterPanel.GetComponentsInChildren<Text>();

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
    }

    internal void SetCharacterInfo(GameObject gameObject)
    {
        _characterImages[0].color =
            gameObject.transform.GetComponent<MeshRenderer>() == null ?
            gameObject.transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color :
            gameObject.transform.GetComponent<MeshRenderer>().materials[1].color;
        _characterImages[1].texture = gameObject.GetComponent<Unit>().Profile;
        _characterImages[1].color = new Color(255, 255, 255);
        _characterImages[2].color = new Color(_characterImages[2].color.r, _characterImages[2].color.g, _characterImages[2].color.b, 0);

        Unit unit = gameObject.GetComponent<Unit>();
        _characterInfos[0].text = unit.Name;
        _characterInfos[1].text = unit.Attack.ToString();
        _characterInfos[2].text = unit.Defense.ToString();
        _characterInfos[3].text = $"{unit.Hp} / {unit.Hp}";
    }

    internal void SetNonProfile()
    {
        _characterImages[0].color = _mainCharacterImage.color;
        _characterImages[2].color = new Color(_characterImages[2].color.r, _characterImages[2].color.g, _characterImages[2].color.b, 255);

        foreach (Text text in _characterInfos)
        {
            text.text = "";
        }
    }



    #endregion
}
