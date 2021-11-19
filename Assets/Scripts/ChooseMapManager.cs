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
    public Image colorPlayers;

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
            PanelPlayerList[i].GetComponentInChildren<Text>().text = "Computer" + (i + 1);
            PanelPlayerList[i].gameObject.SetActive(true);
            PanelPlayerList[i].transform.SetParent(PlaySettingsPanel.transform);

            Dropdown[] dropdowns = PanelPlayerList[i].GetComponentsInChildren<Dropdown>();
            dropdowns[0].value = 0;
            dropdowns[1].value = i;

            List<string> placeList = new List<string>();
            placeList.Add("-");
            for (int j = 0; j < map.UnitStartLocations.Count; j++)
            {
                placeList.Add((j+1).ToString());
            }
            placeList.Add("Random");
            dropdowns[2].AddOptions(placeList);
            dropdowns[2].value = 0;
        }
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
