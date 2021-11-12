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
    public string name;
    public string type;
    public DateTime CreateTime;
    public DateTime UpdateTime;
    public int firstValue;
    public int secondValue;
    public int thirdValue;
}

public static class SaveSystem
{
    public static void  SaveMap(ref Map map)
    {
        if (map.ifExist)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.dataPath + $"/Game/Maps/Editor/{map.type}/{map.name}";

            FileStream stream = new FileStream(path, FileMode.Open);

            formatter.Serialize(stream, map);

            stream.Close();
        }

        if (!map.ifExist)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.dataPath + $"/Game/Maps/Editor/{map.type}/{map.name}";

            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, map);
            stream.Close();

            map.ifExist = true;
        }
        //Debug.Log(map.ifExist + " " + map.name + " " + map.type);
        //Debug.Log(typeOfMap);
    }

    public static Map LoadMap(ref Map map)
    {
        string path = Application.dataPath + $"Game/Maps/{map.type}/{map.name}.war";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            map = formatter.Deserialize(stream) as Map;
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
        string path = Application.dataPath + $"/Game/Maps/Editor/Campaign/";
        var info = new DirectoryInfo(path);
        var fileInfo = info.GetFiles();

        List<string> NamesOfMaps = new List<string>();

        foreach(var file in fileInfo)
        {
            NamesOfMaps.Add(file.Name as string);
        }

        NamesOfMaps = NamesOfMaps.Where(n => !n.Contains(".meta")).ToList();

        return NamesOfMaps;
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

    public GameObject optionsEditorPanel;


    private Map map = new Map();

    private void Start()
    {
        ActiveDeactivatePanel(filePanel, false);
        ActiveDeactivatePanel(saveMapPanel, false);
        ActiveDeactivatePanel(loadMapPanel, false);
        ActiveDeactivatePanel(optionsEditorPanel, false);


        InitializeScrollRectLoadMap();
        InitializeDropDownTypeOfMap();
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
        //todo
    }

    public void SaveClickCancel()
    {
        ActiveDeactivatePanel(saveMapPanel, false);
    }

    public void SaveClickAccept()
    {
        InputField nameOfMapInputField = GetComponentInChildren<InputField>();

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

        if(loadMapPanel.activeSelf)
        {
            dropdownMapsToLoad.AddOptions(SaveSystem.GetNamesMaps());
        }
    }

    public void LoadMap()
    {
        SaveSystem.LoadMap(ref map);

        Debug.Log(map.firstValue + " " + map.secondValue + " " + map.thirdValue);
    }

    public void Generate()
    {

    }

    public void Options()
    {
        ActiveDeactivatePanel(optionsEditorPanel, !optionsEditorPanel.activeSelf);
    }



    public void Exit()
    {
        OptionsMenu.GoToMainMenu();
    }

    private void  ActiveDeactivatePanel(GameObject panel, bool activeDesactive)
    {
        panel.SetActive(activeDesactive);
    }


}
