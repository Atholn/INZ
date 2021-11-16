using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChoseMapManager : MonoBehaviour
{
    public GameObject ChooseMapPanel;
    Dropdown MapsDropdown;
    public GameObject PlaySettingsPanel;
    public GameObject MapInfoPanel;
    public GameObject ActionPanel;

    FileMapSystem fileMapSystem;

    private void Start()
    {
        fileMapSystem = new FileMapSystem() { FolderName = "Sirmish" };

        MapsDropdown =  ChooseMapPanel.GetComponentInChildren<Dropdown>();

        MapsDropdown.options.Clear();
        MapsDropdown.AddOptions(fileMapSystem.GetNamesMaps(fileMapSystem.FolderName));
    }

    public void ChooseMap()
    {

    }

    public void PlayButton()
    {
        SceneManager.LoadScene("Sirmish");

        //todo create tmp file to load in skirmish
    }

    public void BackButton()
    {
        OptionsMenu.GoToMainMenu();
    }
}
