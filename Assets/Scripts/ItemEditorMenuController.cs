using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemEditorMenuController : MonoBehaviour
{
    LevelEditorManager lem;
    public void Save()
    {
        GameObject saveMapEditorPanel = GameObject.FindGameObjectWithTag("SaveMapEditorPanel");
        if(saveMapEditorPanel != null)
        saveMapEditorPanel.SetActive(true);
    }

    public void Load()
    {

    }

    public void Generate()
    {

    }

    public void Exit()
    {
        OptionsMenu.GoToMainMenu();
        //SceneManager.LoadScene("MainMenu");
    }
}
