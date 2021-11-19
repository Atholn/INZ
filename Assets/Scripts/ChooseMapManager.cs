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


    Vector3 DisplacementVector = new Vector3(-100, 225, 0);

    public GameObject PanelComputerSettings;
    private GameObject[] PanelsPlayer;

    List<GameObject> PanelPlayerList = new List<GameObject>();
    public Image colorPlayers;

    private void Start()
    {
        FileMapSystem = new FileMapSystem() { FolderName = "Sirmish" };

        MapsDropdown = ChooseMapPanel.GetComponentInChildren<Dropdown>();

        MapsDropdown.options.Clear();
        MapsDropdown.AddOptions(FileMapSystem.GetNamesMaps(FileMapSystem.FolderName));


        InfoTexts = MapInfoPanel.GetComponentsInChildren<Text>();

        InitializeComputerPanel();
    }

    private void InitializeComputerPanel()
    {
        List<Dropdown> computerSettings = PanelComputerSettings.GetComponentsInChildren<Dropdown>().ToList();

        computerSettings[0].AddOptions(new List<string>() { "None", "Computer" });
    }




    public void ChooseMap()
    {
        Map map = FileMapSystem.GetMapInfo(FileMapSystem.FolderName, MapsDropdown.options[MapsDropdown.value].text);
        InfoTexts[0].text = map.Name;
        InfoTexts[1].text = map.Decription;

        //--------------------
        if (PanelsPlayer != null)
        {
            for (int i = 0; i < PanelsPlayer.Length; i++)
            {               
                Destroy(PanelsPlayer[i].gameObject);
            }
        }

        PanelsPlayer = new GameObject[map.UnitStartLocations.Count - 1];

        for (int i = 0; i < PanelsPlayer.Length; i++)
        {
            //float height = Dropdown.GetComponent<RectTransform>().rect.height;
            //TypeOfPlayer[i] = Instantiate(Dropdown, new Vector3(PlaySettingsPanel.transform.position.x + DisplacementVector.x, PlaySettingsPanel.transform.position.y - i * (height + 5) + DisplacementVector.y, DisplacementVector.z), PlaySettingsPanel.transform.localRotation);
            //TypeOfPlayer[i].gameObject.SetActive(true);
            //TypeOfPlayer[i].transform.SetParent(PlaySettingsPanel.transform);


            PanelsPlayer[i] = Instantiate(PanelComputerSettings, new Vector3(PlaySettingsPanel.transform.position.x + DisplacementVector.x, PlaySettingsPanel.transform.position.y - i *PanelComputerSettings.GetComponent<RectTransform>().rect.height + DisplacementVector.y, DisplacementVector.z), PlaySettingsPanel.transform.localRotation);
            PanelsPlayer[i].GetComponentInChildren<Text>().text = "Computer" + (i + 1);
            PanelsPlayer[i].gameObject.SetActive(true);
            PanelsPlayer[i].transform.SetParent(PlaySettingsPanel.transform);
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
