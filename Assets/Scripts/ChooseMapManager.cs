using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChooseMapManager : MonoBehaviour
{
    public GameObject ChooseMapPanel;
    Dropdown MapsDropdown;
    public GameObject PlaySettingsPanel;
    public GameObject MapInfoPanel;
    Text[] InfoTexts = new Text[2];
    public GameObject ActionPanel;

    FileMapSystem fileMapSystem;

    private void Start()
    {
        fileMapSystem = new FileMapSystem() { FolderName = "Sirmish" };

        MapsDropdown =  ChooseMapPanel.GetComponentInChildren<Dropdown>();

        MapsDropdown.options.Clear();
        MapsDropdown.AddOptions(fileMapSystem.GetNamesMaps(fileMapSystem.FolderName));

         
        InfoTexts = MapInfoPanel.GetComponentsInChildren<Text>();
        if (InfoTexts == null) Debug.Log("sad");
    }

    public void ChooseMap()
    {
        Map map = fileMapSystem.GetMapInfo(fileMapSystem.FolderName, MapsDropdown.options[MapsDropdown.value].text);
        InfoTexts[0].text = map.Name;
        InfoTexts[1].text = map.Decription;
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
