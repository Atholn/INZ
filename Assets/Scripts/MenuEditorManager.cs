using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuEditorManager : MonoBehaviour
{
    enum TypeOfMap
    {
        Campaign,
        Sirmish,
        FreeGame,
    }

    public GameObject filePanel;

    public GameObject saveMapPanel;
    Dropdown dropdownTypeOfMap;

    public GameObject loadMapPanel;
    Dropdown dropdownMapsToLoad;
    public GameObject mapLoadInfoPanel;
    private Text[] infoLoadTexts = new Text[4];

    public GameObject mapInfoPanel;
    private Text[] infoTexts = new Text[5];

    public GameObject optionsEditorPanel;

    private Map map = new Map();

    private FileMapSystem fileMapSystem;

    private void Start()
    {
        InitializeFileMapSystem();

        InitializePanels();
        InitializeScrollRectLoadMap();
        InitializeDropDownTypeOfMap();
        InitializeMapLoadInfo();
        InitializeMapInfo();
    }

    private void InitializeFileMapSystem()
    {
        fileMapSystem = new FileMapSystem() { FolderName = "Editor" };
    }

    private void InitializePanels()
    {
        ActiveDeactivatePanel(filePanel, false);
        ActiveDeactivatePanel(saveMapPanel, false);
        ActiveDeactivatePanel(loadMapPanel, false);
        ActiveDeactivatePanel(optionsEditorPanel, false);
        ActiveDeactivatePanel(mapLoadInfoPanel, true);
        ActiveDeactivatePanel(mapInfoPanel, false);
    }

    private void InitializeScrollRectLoadMap()
    {
        dropdownMapsToLoad = loadMapPanel.GetComponentInChildren<Dropdown>();
    }

    private void InitializeDropDownTypeOfMap()
    {
        dropdownTypeOfMap = saveMapPanel.GetComponentInChildren<Dropdown>();
        List<string> types = new List<string>(Enum.GetNames(typeof(TypeOfMap)));
        dropdownTypeOfMap.AddOptions(types);
    }

    private void InitializeMapLoadInfo()
    {
        infoLoadTexts = mapLoadInfoPanel.GetComponentsInChildren<Text>().ToArray();
    }

    private void InitializeMapInfo()
    {
        infoTexts = mapInfoPanel.GetComponentsInChildren<Text>().ToArray();
    }

    private void Update()
    {        
        if(Input.GetMouseButtonDown(2))
        {
            map.firstValue++;
        }

        if (Input.GetMouseButtonDown(3))
        {
            map.secondValue++;
        }

        if (Input.GetMouseButtonDown(4))
        {
            map.thirdValue++;
        }

        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log(map.firstValue + " " + map.secondValue + " " + map.thirdValue);
        }
    }

    // File section
    public void File()
    {
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);
        
        if(filePanel.activeSelf)
        {
            GetComponentInChildren<Text>().text = @"File /\";
        }
        else
        {
            GetComponentInChildren<Text>().text = @"File \/";
        } 
    }

    public void New()
    {
        map = new Map();

        //todo
    }

    public void Save()
    {
        if (map.ifExist)
        {
            fileMapSystem.SaveMap(ref map);
            return;
        }

        if (!map.ifExist)
        {
            ActiveDeactivatePanel(saveMapPanel, true);
            return;
        }
    }

    public void SaveAs()
    {
        map.saveAs = true;
        ActiveDeactivatePanel(saveMapPanel, true);
    }


    public void SaveClickAccept()
    {
        InputField nameOfMapInputField = saveMapPanel.GetComponentInChildren<InputField>();

        if(map.nameToChange)
        { 
            //todo
            return;
        }

        if(nameOfMapInputField == null)
        {
            ActiveDeactivatePanel(saveMapPanel, false);
            
            return;
        }

        if(nameOfMapInputField.text != "")
        {
            map.name = nameOfMapInputField.text;
            map.type = dropdownTypeOfMap.options[dropdownTypeOfMap.value].text;

            fileMapSystem.SaveMap(ref map);
        }

        ActiveDeactivatePanel(saveMapPanel, false);
    }

    public void Load()
    {
        ActiveDeactivatePanel(loadMapPanel, !loadMapPanel.activeSelf);

        if (loadMapPanel.activeSelf)
        {
            dropdownMapsToLoad.options.Clear();
            dropdownMapsToLoad.AddOptions(fileMapSystem.GetNamesMaps("Editor"));
        }
    }

    public void LoadMap()
    {
        map = fileMapSystem.LoadMap(dropdownMapsToLoad.options[dropdownMapsToLoad.value].text);
        ActiveDeactivatePanel(loadMapPanel, !loadMapPanel.activeSelf);
    }

    public void ChosingMapToLoad()
    {
        Map mapInfo = fileMapSystem.GetMapInfo("Editor" , dropdownMapsToLoad.options[dropdownMapsToLoad.value].text);

        infoLoadTexts[1].text = "Type: " + mapInfo.type;
    }

    public void Generate()
    {
        fileMapSystem.Generate(map);
    }

    public void Info()
    {
        ActiveDeactivatePanel(mapInfoPanel, !mapInfoPanel.activeSelf);

        if(mapInfoPanel.activeSelf)
        {
            infoTexts[0].text = "Map info:";
            infoTexts[1].text = map.name == "" ? "Name: untitled" : "Name: " + map.name;
            infoTexts[2].text = map.type == "" ? "Type: no chose yet" : "Type: " + map.type;
        }
    }

    public void Options()
    {
        ActiveDeactivatePanel(optionsEditorPanel, !optionsEditorPanel.activeSelf);
    }

    public void Exit()
    {
        OptionsMenu.GoToMainMenu();
    }

    public void Cancel(GameObject panel)
    {
        ActiveDeactivatePanel(panel, false);
        if (map.saveAs) map.saveAs = false;
    }

    private void  ActiveDeactivatePanel(GameObject panel, bool activeDesactive)
    {
        panel.SetActive(activeDesactive);
    }
}
