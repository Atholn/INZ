using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class ChooseMapManager : MonoBehaviour
{
    public GameObject ChooseMapPanel;
    Dropdown MapsDropdown;
    public GameObject PlaySettingsPanel;
    public GameObject MapInfoPanel;
    Text[] InfoTexts = new Text[2];
    public GameObject ActionPanel;

    FileMapSystem FileMapSystem;

    Vector3 DisplacementVector = new Vector3(-50, 225, 0);

    public GameObject PanelComputerSettings;

    List<GameObject> PanelPlayerList = new List<GameObject>();

    private void Start()
    {
        FileMapSystem = new FileMapSystem() { FolderName = "Sirmish" };

        MapsDropdown = ChooseMapPanel.GetComponentInChildren<Dropdown>();

        MapsDropdown.options.Clear();
        MapsDropdown.AddOptions(FileMapSystem.GetNamesMaps(FileMapSystem.FolderName));

        InfoTexts = MapInfoPanel.GetComponentsInChildren<Text>();

        InitializeListPlayer();
        InitializeComputerPanel();
    }

    private void InitializeComputerPanel()
    {
        List<Dropdown> computerSettings = PanelComputerSettings.GetComponentsInChildren<Dropdown>().ToList();

        computerSettings[0].AddOptions(new List<string>() { "None", "Computer" });
    }

    private void InitializeListPlayer()
    {
        PanelPlayerList.Add(Instantiate(PanelComputerSettings,
            new Vector3(PlaySettingsPanel.transform.position.x + DisplacementVector.x,
            PlaySettingsPanel.transform.position.y + DisplacementVector.y,
            DisplacementVector.z),
            PlaySettingsPanel.transform.localRotation));
        PanelPlayerList[0].gameObject.SetActive(true);
        PanelPlayerList[0].transform.SetParent(PlaySettingsPanel.transform);
        PanelPlayerList[0].GetComponentInChildren<Text>().gameObject.SetActive(true);
        Destroy(PanelPlayerList[0].GetComponentInChildren<Dropdown>().gameObject);
    }

    public void ChooseMap()
    {
        Map map = FileMapSystem.GetMapInfo(FileMapSystem.FolderName, MapsDropdown.options[MapsDropdown.value].text);
        InfoTexts[0].text = map.Name;
        InfoTexts[1].text = map.Decription;

        if (PanelPlayerList.Count > 1)
        {
            for (int i = PanelPlayerList.Count-1; i >1 ; i--)
            {
                Destroy(PanelPlayerList[i].gameObject);
                PanelPlayerList.Remove(PanelPlayerList[i]);
            }
        }

        for (int i = 1; i < map.UnitStartLocations.Count; i++)
        {
            PanelPlayerList.Add(Instantiate(PanelComputerSettings, new Vector3(PlaySettingsPanel.transform.position.x + DisplacementVector.x, PlaySettingsPanel.transform.position.y - i * PanelComputerSettings.GetComponent<RectTransform>().rect.height + DisplacementVector.y, DisplacementVector.z), PlaySettingsPanel.transform.localRotation));
            PanelPlayerList[i].gameObject.SetActive(true);
            PanelPlayerList[i].transform.SetParent(PlaySettingsPanel.transform);

            GenerateChooseNumbers(PanelPlayerList[i], map.UnitStartLocations.Count, i);
        }

        GenerateChooseNumbers(PanelPlayerList[0], map.UnitStartLocations.Count, 0);
    }

    public void GenerateChooseNumbers(GameObject panel, int countPlayers, int whichColour)
    {
        Dropdown[] dropdowns = panel.GetComponentsInChildren<Dropdown>();

        int i;
        if(dropdowns.Length == 3)
        {
            dropdowns[0].value = 0;
            i = 0;
        }
        else
        {
            i = 1;
        }

        dropdowns[1-i].value = whichColour;

        List<string> placeList = new List<string>();
        placeList.Add("Random");
        for (int j = 0; j < countPlayers; j++)
        {
            placeList.Add((j + 1).ToString());
        }
        
        dropdowns[2 - i].options.Clear();
        dropdowns[2 - i].AddOptions(placeList);
        dropdowns[2 - i].value = 0;     
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
