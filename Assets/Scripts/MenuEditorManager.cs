using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Linq;

[System.Serializable]
public class Map
{
    public bool ifExist = false;
    public bool saveAs = false;
    public string name = "";
    public string type = "";

    public string CreateTime;
    public string UpdateTime;

    public int firstValue;
    public int secondValue;
    public int thirdValue;
}

public static class SaveSystem
{
    public static void  SaveMap(ref Map map)
    {
        if (map.ifExist && !map.saveAs)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.dataPath + $"/Game/Maps/Editor/{map.name}";

            FileStream stream = new FileStream(path, FileMode.Open);

            formatter.Serialize(stream, map);

            stream.Close();
            return;
        }

        if (!map.ifExist || map.saveAs)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.dataPath + $"/Game/Maps/Editor/{map.name}";

            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, map);
            stream.Close();

            map.ifExist = true;
            map.saveAs = false;
        }
    }

    public static Map LoadMap(string  nameMap)
    {
        string path = Application.dataPath + $"/Game/Maps/Editor/{nameMap}";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Map map = formatter.Deserialize(stream) as Map;
            stream.Close();
            
            return map;
        }
        else
        {
            return null;
        }
    }

    public static List<string> GetNamesMaps()
    {
        string path = Application.dataPath + $"/Game/Maps/Editor/";
        var info = new DirectoryInfo(path);
        var fileInfo = info.GetFiles();

        List<string> NamesOfMaps = new List<string>();

        foreach(var file in fileInfo)
        {
            NamesOfMaps.Add(file.Name as string);
        }

        NamesOfMaps = NamesOfMaps.Where(n => !n.Contains(".meta")).ToList();

        //todo  pliki z koncowkami ktore pomijamy

        return NamesOfMaps;
    }

    public static Map GetMapInfo(string nameMap)
    {
        string path = Application.dataPath + $"/Game/Maps/Editor/{nameMap}";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Map map = formatter.Deserialize(stream) as Map;
            stream.Close();
            return map;
        }
        else
        {
            return null;
        }
    }
}

public class MenuEditorManager : MonoBehaviour
{
    enum TypeOfMap
    {
        Campaign,
        Sirmish,
        FreeGame
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

    private void Start()
    {
        InitializePanels();
        InitializeScrollRectLoadMap();
        InitializeDropDownTypeOfMap();
        InitializeMapLoadInfo();
        InitializeMapInfo();
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
            SaveSystem.SaveMap(ref map);
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

        if(nameOfMapInputField == null)
        {
            ActiveDeactivatePanel(saveMapPanel, false);
            
            return;
        }

        if(nameOfMapInputField.text != "")
        {
            map.name = nameOfMapInputField.text;
            map.type = dropdownTypeOfMap.options[dropdownTypeOfMap.value].text;
            
            SaveSystem.SaveMap(ref map);
        }

        ActiveDeactivatePanel(saveMapPanel, false);
    }

    public void Load()
    {
        ActiveDeactivatePanel(loadMapPanel, !loadMapPanel.activeSelf);

        if (loadMapPanel.activeSelf)
        {
            dropdownMapsToLoad.options.Clear();
            dropdownMapsToLoad.AddOptions(SaveSystem.GetNamesMaps());
        }
    }

    public void LoadMap()
    {
        map = SaveSystem.LoadMap(dropdownMapsToLoad.options[dropdownMapsToLoad.value].text);
        ActiveDeactivatePanel(loadMapPanel, !loadMapPanel.activeSelf);
    }

    public void ChosingMapToLoad()
    {
        Map mapInfo = SaveSystem.GetMapInfo(dropdownMapsToLoad.options[dropdownMapsToLoad.value].text);

        infoLoadTexts[1].text = "Type: " + mapInfo.type;
    }

    public void Generate()
    {

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
