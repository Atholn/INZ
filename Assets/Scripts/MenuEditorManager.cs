using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEditorManager : MonoBehaviour
{
    public GameObject OptionsEditorPanel;
    public GameObject SaveMapPanel;

    private void Start()
    {
        OptionsEditorPanel.SetActive(false);
        SaveMapPanel.SetActive(false);

    }

    public void Save()
    {
        SaveMapPanel.SetActive(true);
    }

    public void Load()
    {

    }

    public void Generate()
    {

    }

    public void Options()
    {
        OptionsEditorPanel.SetActive(!OptionsEditorPanel.activeSelf);
    }

    public void Exit()
    {
        OptionsMenu.GoToMainMenu();
    }
}
