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

    public void SetFoto(GameObject gameObject)
    {
        RawImage[] rawImages = CharacterPanel.GetComponentsInChildren<RawImage>();

        rawImages[0].color = 
            gameObject.transform.GetComponent<MeshRenderer>() == null ?
            gameObject.transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color :
            gameObject.transform.GetComponent<MeshRenderer>().materials[1].color;

        rawImages[1].texture = gameObject.GetComponent<Unit>().Profile;

        CharacterPanel.GetComponentInChildren<Text>().text = gameObject.GetComponent<Unit>().Name;

    }

    #endregion
}
