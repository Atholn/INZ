using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameObject TopPanel;
    public GameObject MenuPanel;
    public GameObject BottomPanel;


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


    #endregion



}
