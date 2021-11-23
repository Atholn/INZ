using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class ChoiceMapManager : MonoBehaviour
{
    public GameObject ChoiceMapPanel;
    public GameObject PlaySettingsPanel;
    public GameObject MapInfoPanel;
    public GameObject ActionPanel;
    public GameObject PanelComputerSettings;

    private Text[] InfoTexts = new Text[2];
    private Dropdown MapsDropdown;
    private Vector3 DisplacementVector = new Vector3(-50, 225, 0);
    private List<GameObject> PanelPlayerList = new List<GameObject>();

    private FileMapSystem FileMapSystem;

    private void Start()
    {
        InitializeFileMapSystem();
        InitializeMapList();
        InitializeListPlayer();
        InitializeComponents();
        InitializeFirstMap();
    }
    private void InitializeFileMapSystem()
    {
        FileMapSystem = new FileMapSystem() { FolderName = "Sirmish" };
    }

    private void InitializeMapList()
    {
        MapsDropdown = ChoiceMapPanel.GetComponentInChildren<Dropdown>();
        MapsDropdown.options.Clear();
        MapsDropdown.AddOptions(FileMapSystem.GetNamesMaps(FileMapSystem.FolderName));
    }

    private void InitializeFirstMap()
    {
        LoadMapInfo();
    }

    private void InitializeComponents()
    {
        List<Dropdown> computerSettings = PanelComputerSettings.GetComponentsInChildren<Dropdown>().ToList();
        computerSettings[0].AddOptions(new List<string>() { "None", "Computer" });

        InfoTexts = MapInfoPanel.GetComponentsInChildren<Text>();
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

        Dropdown dropdownToDelete = PanelPlayerList[0].GetComponentInChildren<Dropdown>();

        Text playerText = Instantiate(ChoiceMapPanel.GetComponentInChildren<Text>(), new Vector3(0, 0, 0), PanelPlayerList[0].transform.rotation);
        playerText.transform.SetParent(PanelPlayerList[0].transform);
        playerText.text = "Player";
        playerText.transform.localPosition = new Vector3(dropdownToDelete.transform.localPosition.x, 0, 0);

        Destroy(dropdownToDelete.gameObject);
    }

    public void LoadMapInfo()
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

        GeneratingDifferentFeatures(PanelPlayerList[0], map.UnitStartLocations.Count, 0);
        for (int i = 1; i < map.UnitStartLocations.Count; i++)
        {
            PanelPlayerList.Add(Instantiate(PanelComputerSettings, new Vector3(PlaySettingsPanel.transform.position.x + DisplacementVector.x, PlaySettingsPanel.transform.position.y - i * PanelComputerSettings.GetComponent<RectTransform>().rect.height + DisplacementVector.y, DisplacementVector.z), PlaySettingsPanel.transform.localRotation));
            PanelPlayerList[i].gameObject.SetActive(true);
            PanelPlayerList[i].transform.SetParent(PlaySettingsPanel.transform);

            GeneratingDifferentFeatures(PanelPlayerList[i], map.UnitStartLocations.Count, i);
        }
    }

    public void GeneratingDifferentFeatures(GameObject panel, int countPlayers, int whichColour)
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

    internal void Play()
    {
        Map map = FileMapSystem.LoadMap(MapsDropdown.options[MapsDropdown.value].text);
        MapToPlayStorage.Map = map;
    }
}
