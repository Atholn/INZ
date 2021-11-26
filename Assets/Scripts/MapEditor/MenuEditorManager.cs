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

    public MapEditorManager levelEditorManager;
    public GameObject filePanel;

    public GameObject saveMapPanel;
    private Dropdown dropdownTypeOfMap;

    public GameObject loadMapPanel;
    private Dropdown dropdownMapsToLoad;

    public GameObject mapLoadInfoPanel;
    private Text[] infoLoadTexts = new Text[4];

    public GameObject mapInfoPanel;
    private Text[] infoTexts = new Text[5];
    private Image mapViewImage;
    float[][][] mapViewColors;

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
        mapViewImage = mapInfoPanel.GetComponentsInChildren<Image>()[2];
    }

    // File section
    public void File()
    {
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);

        if (filePanel.activeSelf)
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
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);
        map = new Map();

        levelEditorManager.NewTerrain();
        //todo
    }

    public void Save()
    {
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);
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
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);
        map.saveAs = true;
        ActiveDeactivatePanel(saveMapPanel, true);
    }

    public void SaveClickAccept()
    {
        InputField nameOfMapInputField = saveMapPanel.GetComponentInChildren<InputField>();

        if (map.nameToChange)
        {
            //todo
            return;
        }

        if (nameOfMapInputField == null)
        {
            ActiveDeactivatePanel(saveMapPanel, false);

            return;
        }

        if (nameOfMapInputField.text != "")
        {
            map.Name = nameOfMapInputField.text;
            map.Type = dropdownTypeOfMap.options[dropdownTypeOfMap.value].text;
            //map.Decription = ""; //todo

            Map tmpMap = levelEditorManager.ExportMap();

            map.SizeMap = tmpMap.SizeMap;
            map.Maps = tmpMap.Maps;
            map.UnitStartLocations = tmpMap.UnitStartLocations;
            map.UnitMaterials = tmpMap.UnitMaterials;
            map.ViewMap = mapViewColors;

            fileMapSystem.SaveMap(ref map);
        }

        ActiveDeactivatePanel(saveMapPanel, false);
    }

    public void Load()
    {
        ActiveDeactivatePanel(loadMapPanel, !loadMapPanel.activeSelf);
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);

        if (loadMapPanel.activeSelf)
        {
            dropdownMapsToLoad.options.Clear();
            dropdownMapsToLoad.AddOptions(fileMapSystem.GetNamesMaps("Editor"));
        }
    }

    public void LoadMap()
    {
        map = new Map();
        map = fileMapSystem.LoadMap(dropdownMapsToLoad.options[dropdownMapsToLoad.value].text);
        levelEditorManager.ImportMap(map);

        ActiveDeactivatePanel(loadMapPanel, !loadMapPanel.activeSelf);
    }

    public void ChosingMapToLoad()
    {
        Map mapInfo = fileMapSystem.GetMapInfo("Editor", dropdownMapsToLoad.options[dropdownMapsToLoad.value].text);

        infoLoadTexts[1].text = "Type: " + mapInfo.Type;
    }

    public void Generate()
    {
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);
        fileMapSystem.Generate(map);
    }

    public void Info()
    {
        ActiveDeactivatePanel(mapInfoPanel, !mapInfoPanel.activeSelf);
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);

        if (mapInfoPanel.activeSelf)
        {
            infoTexts[0].text = "Map info:";
            infoTexts[1].text = map.Name == "" ? "Name: untitled" : "Name: " + map.Name;
            infoTexts[2].text = map.Type == "" ? "Type: no chose yet" : "Type: " + map.Type;
            infoTexts[3].text = $"Size: {levelEditorManager.sizeMap} x {levelEditorManager.sizeMap}";
            mapInfoPanel.GetComponentInChildren<InputField>().text = map.Decription;

            DrawMapView();
        }
    }

    private void DrawMapView()
    {
        if (mapViewColors == null)
        {
            mapViewColors = new float[levelEditorManager.sizeMap][][];

            for (int i = 0; i < levelEditorManager.sizeMap; i++)
            {
                mapViewColors[i] = new float[levelEditorManager.sizeMap][];
                for (int j = 0; j < levelEditorManager.sizeMap; j++)
                {
                    mapViewColors[i][j] = new float[3];
                }
            }
        }

        for (int i = 0; i < levelEditorManager.sizeMap; i++)
        {
            for (int j = 0; j < levelEditorManager.sizeMap; j++)
            {
                for (int k = 0; k < levelEditorManager.mapCount; k++)
                {
                    MeshRenderer mesh = levelEditorManager.mapsPrefabs[k][i, j] != null ?
                        levelEditorManager.mapsPrefabs[k][i, j].gameObject.GetComponent<MeshRenderer>() :
                        k == 0 ? levelEditorManager.Terrain.gameObject.GetComponent<MeshRenderer>() : null;

                    if (mesh != null)
                    {
                        mapViewColors[i][j][0] = mesh.material.color.r;
                        mapViewColors[i][j][1] = mesh.material.color.g;
                        mapViewColors[i][j][2] = mesh.material.color.b;
                    }
                }
            }
        }

        Texture2D texture = new Texture2D(levelEditorManager.sizeMap, levelEditorManager.sizeMap);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, levelEditorManager.sizeMap, levelEditorManager.sizeMap), Vector2.zero);
        mapViewImage.sprite = sprite;

        for (int i = 0; i < texture.height; i++)
        {
            for (int j = 0; j < texture.width; j++)
            {
                Color pixelColour = new Color(mapViewColors[i][j][0], mapViewColors[i][j][1], mapViewColors[i][j][2], 1);
                texture.SetPixel(i, j, pixelColour);
            }
        }
        texture.Apply();
    }

    public void SaveDescription()
    {
        map.Decription = mapInfoPanel.GetComponentInChildren<InputField>().text;
        ActiveDeactivatePanel(mapInfoPanel, false);
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

    private void ActiveDeactivatePanel(GameObject panel, bool activeDesactive)
    {
        panel.SetActive(activeDesactive);
    }
}
