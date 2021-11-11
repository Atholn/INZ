using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

[System.Serializable]
public class Map
{
    public bool ifExist = false;
    public string name;
    public string type;
}

public static class SaveSystem
{
    public static void  SaveMap(ref Map map)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.dataPath + $"/Game/Maps/{map.type}/{map.name}";

        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, map);
        stream.Close();


        Debug.Log(map.ifExist + " " + map.name + " " + map.type);
        map.ifExist = true;
        Debug.Log(map.ifExist + " " + map.name + " " + map.type);
        //Debug.Log(typeOfMap);

    }

    public static Map LoadMap(string typeOfMap, string nameOfMap)
    {
        string path = Application.dataPath + $"Game/Maps/{typeOfMap}/{nameOfMap}.war";
        if(File.Exists(path))
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

    public GameObject optionsEditorPanel;
    public Dropdown dropdownTypeOfMap;

    public GameObject saveMapPanel;

    private Map map = new Map();

    private void Start()
    {
        ActiveDeactivatePanel(optionsEditorPanel, false);
        ActiveDeactivatePanel(saveMapPanel, false);
        InitializeDropDownTypeOfMap();
    }

    private void InitializeDropDownTypeOfMap()
    {
        List<string> types = new List<string>(Enum.GetNames(typeof(TypeOfMap)));
        dropdownTypeOfMap.AddOptions(types);
    }

    // Save section
    public void Save()
    {
        if(!map.ifExist)
        {
            ActiveDeactivatePanel(saveMapPanel, true);
            return;
        }
        
        if(map.ifExist)
        {
            SaveSystem.SaveMap(ref map);
        }
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
        
        
        //Debug.Log(nameOfMapInputField.text);

        ActiveDeactivatePanel(saveMapPanel, false);
    }


    public void Load()
    {

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
