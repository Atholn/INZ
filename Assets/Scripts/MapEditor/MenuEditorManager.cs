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
    private InputField nameOfMapInputField;

    public GameObject loadMapPanel;
    private Dropdown dropdownMapsToLoad;

    public GameObject mapLoadInfoPanel;
    private Text[] infoLoadTexts = new Text[4];

    public GameObject mapInfoPanel;
    private Text[] infoTexts = new Text[6];
    private Image mapViewImage;
    private float[][][] mapViewColors;

    public GameObject optionsEditorPanel;

    private MapInfo _mapInfo = new MapInfo();

    private FileMapSystem fileMapSystem;

    private void Start()
    {
        InitializeFileMapSystem();
        InitializePanels();
        InitializeScrollRectLoadMap();
        InitializeFirstSavePanel();
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

    private void InitializeFirstSavePanel()
    {
        dropdownTypeOfMap = saveMapPanel.GetComponentInChildren<Dropdown>();
        List<string> types = new List<string>(Enum.GetNames(typeof(TypeOfMap)));
        dropdownTypeOfMap.AddOptions(types);

        nameOfMapInputField = saveMapPanel.GetComponentInChildren<InputField>();
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

        UnityEngine.SceneManagement.SceneManager.LoadScene("Editor");
        //MapEditorManager.NewTerrain();
    }

    public void Save()
    {
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);

        if (_mapInfo.Name != "" && fileMapSystem.CheckIfExist(_mapInfo.Name))
        {
            Map map = MapMerging();
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
        Text text = saveMapPanel.GetComponentsInChildren<Text>(true).Where(t => t.CompareTag("InfoSaveErrorText")).FirstOrDefault();
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

        Map map = MapMerging();
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

            UpdateInfoMapToLoad();
        }
    }

    public void LoadMapAccept()
    {
        Map map = fileMapSystem.LoadEditorMap(dropdownMapsToLoad.options[dropdownMapsToLoad.value].text);

        _mapInfo = map.MapInfo;

        mapViewColors = _mapInfo.ViewMap;
        mapInfoPanel.GetComponentInChildren<InputField>().text = _mapInfo.Decription;

        MapEditorManager.ImportMap(map.MapWorldCreate);

        ActiveDeactivatePanel(loadMapPanel, !loadMapPanel.activeSelf);
    }

    public void ChosingMapToLoad()
    {
        UpdateInfoMapToLoad();
    }

    public void Generate()
    {
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);
        fileMapSystem.GenerateEditorMap(MapMerging());
    }

    public void Info()
    {
        ActiveDeactivatePanel(mapInfoPanel, !mapInfoPanel.activeSelf);
        ActiveDeactivatePanel(filePanel, !filePanel.activeSelf);

        if (mapInfoPanel.activeSelf)
        {
            infoTexts[0].text = "Map info:";
            infoTexts[1].text = _mapInfo.Name == "" ? "Name: untitled" : "Name: " + _mapInfo.Name;
            infoTexts[2].text = _mapInfo.Type == "" ? "Type: no chose yet" : "Type: " + _mapInfo.Type;
            infoTexts[3].text = $"Size: {MapEditorManager.GetSizeMap()[0]} x {MapEditorManager.GetSizeMap()[1]}";
            infoTexts[4].text = $"Create time: {_mapInfo.CreateTime}";
            infoTexts[5].text = $"Last update: {_mapInfo.UpdateTime}";

            mapInfoPanel.GetComponentInChildren<InputField>().text = _mapInfo.Decription;

            DrawMapView();
        }
    }

    private void DrawMapView()
    {
        mapViewColors = MapEditorManager.GeneratePixelsColors();

        Texture2D texture = new Texture2D(MapEditorManager.GetSizeMap()[0], MapEditorManager.GetSizeMap()[1]);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, MapEditorManager.GetSizeMap()[0], MapEditorManager.GetSizeMap()[1]), Vector2.zero);
        mapViewImage.sprite = sprite;

        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
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
            mapViewColors = new float[MapEditorManager.GetSizeMap()[0]][][];

            for (int i = 0; i < MapEditorManager.GetSizeMap()[0]; i++)
            {
                mapViewColors[i] = new float[MapEditorManager.GetSizeMap()[1]][];
                for (int j = 0; j < MapEditorManager.GetSizeMap()[1]; j++)
                {
                    mapViewColors[i][j] = new float[3];
                }
            }
        }
    }

    public void SaveDescription()
    {
        _mapInfo.Decription = mapInfoPanel.GetComponentInChildren<InputField>().text;
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

    private void UpdateInfoMapToLoad()
    {
        MapInfo mapInfo = fileMapSystem.GetMapInfo("Editor", dropdownMapsToLoad.options[dropdownMapsToLoad.value].text).MapInfo;
        infoLoadTexts[1].text = "Type: " + mapInfo.Type;
        infoLoadTexts[2].text = $"Create time: {mapInfo.CreateTime}";
        infoLoadTexts[3].text = $"Last update: {mapInfo.UpdateTime}";
    }

    private Map MapMerging()
    {
        _mapInfo.CreateTime = _mapInfo.Name == "" && _mapInfo.Type == "" ? DateTime.UtcNow.ToLocalTime().ToString() : _mapInfo.CreateTime;
        _mapInfo.UpdateTime = DateTime.UtcNow.ToLocalTime().ToString();

        _mapInfo.Name = _mapInfo.Name == "" ? nameOfMapInputField.text : _mapInfo.Name;
        _mapInfo.Type = _mapInfo.Type == "" ? dropdownTypeOfMap.options[dropdownTypeOfMap.value].text : _mapInfo.Type;
        _mapInfo.Decription = mapInfoPanel.GetComponentInChildren<InputField>().text;
        _mapInfo.ViewMap = MapEditorManager.GeneratePixelsColors();

        return new Map() { MapInfo = _mapInfo, MapWorldCreate = MapEditorManager.ExportMap() };
    }
}
