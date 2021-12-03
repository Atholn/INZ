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

    public MapEditorManager MapEditorManager;

    public GameObject filePanel;

    public GameObject mainEditorPanel;

    public GameObject createSettingsPanel;
    private Text locationText;

    public GameObject saveMapPanel;
    private Dropdown dropdownTypeOfMap;

    public GameObject loadMapPanel;
    private Dropdown dropdownMapsToLoad;

    public GameObject mapLoadInfoPanel;
    private Text[] infoLoadTexts = new Text[4];

    public GameObject mapInfoPanel;
    private Text[] infoTexts = new Text[6];
    private Image mapViewImage;
    private float[][][] mapViewColors;

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
        InitializeCreateSettingsPanel();
    }

    private void InitializeFileMapSystem()
    {
        fileMapSystem = new FileMapSystem() { FolderName = "Editor" };
    }

    private void InitializePanels()
    {
        ActiveDeactivatePanel(filePanel, false);
        ActiveDeactivatePanel(createSettingsPanel, false);
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

    private void InitializeCreateSettingsPanel()
    {
        locationText = createSettingsPanel.GetComponentInChildren<Text>(true);
    }

    private void Update()
    {
        UpdateLocationText();
    }

    private void UpdateLocationText()
    {
        locationText.text = (MapEditorManager.v.x - MapEditorManager.v.x % 1).ToString() + " x " + (MapEditorManager.v.z - MapEditorManager.v.z % 1).ToString() + " y";
    }

    public void Size(int size)
    {
        MapEditorManager.InitializeStartTerrain(size, size);
        //todo
    }

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

        MapEditorManager.NewTerrain();
    }

    public void Save()
    {
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);
        DrawMapView();
        map.ViewMap = mapViewColors;

        if (fileMapSystem.CheckIfExist(map.Name))
        {
            fileMapSystem.SaveEditorMap(ref map);
            return;
        }

        ActiveDeactivatePanel(saveMapPanel, true);
        return;
    }

    public void SaveAs()
    {
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);
        ActiveDeactivatePanel(saveMapPanel, true);
    }

    public void SaveClickAccept()
    {
        InputField nameOfMapInputField = saveMapPanel.GetComponentInChildren<InputField>();
        Text text = saveMapPanel.GetComponentsInChildren<Text>(true).Where(t => t.tag == "InfoSaveErrorText").FirstOrDefault();
        text.gameObject.SetActive(false);

        if (nameOfMapInputField.text == "")
        {
            text.text = "Please write name of map";
            text.gameObject.SetActive(true);

            return;
        }

        if (nameOfMapInputField.text != "" && fileMapSystem.CheckIfExist(nameOfMapInputField.text))
        {
            text.text = "Map with this name is already exist";
            text.gameObject.SetActive(true);

            return;
        }

        Map tmpMap = MapEditorManager.ExportMap();

        map.Name = nameOfMapInputField.text;
        map.Type = dropdownTypeOfMap.options[dropdownTypeOfMap.value].text;
        map.ViewMap = mapViewColors;

        map.CreateTime = tmpMap.CreateTime;
        map.UpdateTime = tmpMap.UpdateTime;

        map.SizeMapX = tmpMap.SizeMapX;
        map.SizeMapY = tmpMap.SizeMapY;
        map.Maps = tmpMap.Maps;
        map.UnitStartLocations = tmpMap.UnitStartLocations;
        map.UnitMaterials = tmpMap.UnitMaterials;

        fileMapSystem.SaveEditorMap(ref map);

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

    public void LoadMapAccept()
    {
        map = new Map();
        map = fileMapSystem.LoadEditorMap(dropdownMapsToLoad.options[dropdownMapsToLoad.value].text);
        mapViewColors = map.ViewMap;

        MapEditorManager.ImportMap(map);

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
        fileMapSystem.GenerateEditorMap(map);
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
            infoTexts[3].text = $"Size: {MapEditorManager.sizeMapX} x {MapEditorManager.sizeMapY}";
            infoTexts[4].text = $"Create time: {map.CreateTime}";
            infoTexts[5].text = $"Last update: {map.UpdateTime}";

            mapInfoPanel.GetComponentInChildren<InputField>().text = map.Decription;

            DrawMapView();
        }
    }

    private void DrawMapView()
    {
        InitializePixelsColors();
        GeneratePixelsColors();

        Texture2D texture = new Texture2D(MapEditorManager.sizeMapX, MapEditorManager.sizeMapY);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, MapEditorManager.sizeMapX, MapEditorManager.sizeMapY), Vector2.zero);
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

    private void InitializePixelsColors()
    {
        if (mapViewColors == null)
        {
            mapViewColors = new float[MapEditorManager.sizeMapX][][];

            for (int i = 0; i < MapEditorManager.sizeMapX; i++)
            {
                mapViewColors[i] = new float[MapEditorManager.sizeMapY][];
                for (int j = 0; j < MapEditorManager.sizeMapY; j++)
                {
                    mapViewColors[i][j] = new float[3];
                }
            }
        }
    }

    private void GeneratePixelsColors()
    {
        for (int i = 0; i < MapEditorManager.sizeMapX; i++)
        {
            for (int j = 0; j < MapEditorManager.sizeMapY; j++)
            {
                for (int k = 0; k < MapEditorManager.mapCount; k++)
                {
                    MeshRenderer mesh = MapEditorManager.mapsPrefabs[k][i][j] != null ?
                        MapEditorManager.mapsPrefabs[k][i][j].gameObject.GetComponent<MeshRenderer>() :
                        k == 0 ? MapEditorManager.Terrain.gameObject.GetComponent<MeshRenderer>() : null;

                    if (mesh != null)
                    {
                        mapViewColors[i][j][0] = mesh.material.color.r;
                        mapViewColors[i][j][1] = mesh.material.color.g;
                        mapViewColors[i][j][2] = mesh.material.color.b;
                    }
                }
            }
        }
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void Cancel(GameObject panel)
    {
        ActiveDeactivatePanel(panel, false);
    }

    private void ActiveDeactivatePanel(GameObject panel, bool activeDesactive)
    {
        panel.SetActive(activeDesactive);
    }

    public void SetActivePanel(GameObject panelToHideShow)
    {
        if (mainEditorPanel.activeSelf)
        {
            ActiveDeactivatePanel(mainEditorPanel, false);
            ActiveDeactivatePanel(panelToHideShow, true);
            return;
        }

        ActiveDeactivatePanel(mainEditorPanel, true);
        ActiveDeactivatePanel(panelToHideShow, false);
    }
}
