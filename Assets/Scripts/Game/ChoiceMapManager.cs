using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class ChoiceMapManager : MonoBehaviour
{
    public string TypeOfGame;
    public GameObject ChoiceMapPanel;
    public GameObject PlaySettingsPanel;
    public GameObject MapInfoPanel;
    public GameObject ActionPanel;
    public GameObject PanelSettingsPlayers;

    private Text[] InfoTexts = new Text[2];
    private Dropdown MapsDropdown;
    private Vector3 DisplacementVector = new Vector3(-50, 225, 0);
    private List<GameObject> PanelPlayerList = new List<GameObject>();

    private Image MapView;
    private Text NumberText;
    private List<Text> PlaceNumbersList = new List<Text>();
    private List<int> TypeOfPlayerTmpList = new List<int>();
    private List<int> PlaceNumbersTmpList = new List<int>();
    private List<int> ColorUnitsTmpList = new List<int>();

    private FileMapSystem FileMapSystem;

    private void Start()
    {
        InitializePlaceNumbers();
        InitializeFileMapSystem();
        InitializeMapList();
        InitializeComponents();
        InitializeFirstPlayer();
        InitializeFirstMap();
    }

    private void InitializePlaceNumbers()
    {
        MapView = MapInfoPanel.GetComponentsInChildren<Image>().LastOrDefault();
        NumberText = MapView.GetComponentInChildren<Text>();
        PlaceNumbersList.Add(NumberText);

        TypeOfPlayerTmpList.Add(1);
        PlaceNumbersTmpList.Add(0);
        ColorUnitsTmpList.Add(0);
    }

    private void InitializeFileMapSystem()
    {
        FileMapSystem = new FileMapSystem() { FolderName = TypeOfGame };
    }

    private void InitializeMapList()
    {
        MapsDropdown = ChoiceMapPanel.GetComponentInChildren<Dropdown>();
        MapsDropdown.options.Clear();
        MapsDropdown.AddOptions(FileMapSystem.GetNamesMaps(FileMapSystem.FolderName));
    }

    private void InitializeComponents()
    {
        List<Dropdown> computerSettings = PanelSettingsPlayers.GetComponentsInChildren<Dropdown>().ToList();
        computerSettings[0].AddOptions(new List<string>() { "None", "Computer" });

        InfoTexts = MapInfoPanel.GetComponentsInChildren<Text>();

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (Sprite sprite in MapToPlayStorage.ImportResources<Sprite>("UI/UnitColors/", ".bmp"))
        {
            options.Add(new Dropdown.OptionData(sprite.name, sprite));
        }

        computerSettings[1].AddOptions(options);
    }

    private void InitializeFirstPlayer()
    {
        PanelPlayerList.Add(Instantiate(PanelSettingsPlayers,
            new Vector3(PlaySettingsPanel.transform.position.x + DisplacementVector.x,
            PlaySettingsPanel.transform.position.y + DisplacementVector.y,
            DisplacementVector.z),
            PlaySettingsPanel.transform.localRotation));
        PanelPlayerList[0].gameObject.SetActive(true);
        PanelPlayerList[0].transform.SetParent(PlaySettingsPanel.transform);
        PanelPlayerList[0].GetComponentInChildren<Text>().gameObject.SetActive(true);

        Dropdown dropdownToHide = PanelPlayerList[0].GetComponentInChildren<Dropdown>();

        Text playerText = Instantiate(ChoiceMapPanel.GetComponentInChildren<Text>(), new Vector3(0, 0, 0), PanelPlayerList[0].transform.rotation);
        playerText.transform.SetParent(PanelPlayerList[0].transform);
        playerText.text = "Player";
        playerText.transform.localPosition = new Vector3(dropdownToHide.transform.localPosition.x, 0, 0);

        dropdownToHide.value = 1;
        dropdownToHide.gameObject.SetActive(false);
    }

    private void InitializeFirstMap()
    {
        LoadMapInfo();
    }

    public void LoadMapInfo()
    {
        Map map = FileMapSystem.GetMapInfo(FileMapSystem.FolderName, MapsDropdown.options[MapsDropdown.value].text);
        InfoTexts[0].text = map.Name;
        InfoTexts[1].text = map.Decription;

        if (PanelPlayerList.Count > 1)
        {
            for (int i = PanelPlayerList.Count - 1; i > 0; i--)
            {
                Destroy(PanelPlayerList[i].gameObject);
                PanelPlayerList.Remove(PanelPlayerList[i]);

                Destroy(PlaceNumbersList[i].gameObject);
                PlaceNumbersList.Remove(PlaceNumbersList[i]);

                TypeOfPlayerTmpList.Remove(TypeOfPlayerTmpList[i]);
                PlaceNumbersTmpList.Remove(PlaceNumbersTmpList[i]);
                ColorUnitsTmpList.Remove(ColorUnitsTmpList[i]);
            }
        }

        RectTransform rT = MapView.GetComponent<RectTransform>();
        float scale = rT.rect.height / map.SizeMap;

        GeneratingDifferentFeatures(PanelPlayerList[0], map.UnitStartLocations.Count, 0);
        PlaceNumbersList[0].transform.localPosition = new Vector3(-scale * ((map.SizeMap / 2) - map.UnitStartLocations[0][0]), -scale * ((map.SizeMap / 2) - map.UnitStartLocations[0][2]), 0);

        for (int i = 1; i < map.UnitStartLocations.Count; i++)
        {
            PanelPlayerList.Add(Instantiate(PanelSettingsPlayers, new Vector3(PlaySettingsPanel.transform.position.x + DisplacementVector.x, PlaySettingsPanel.transform.position.y - i * PanelSettingsPlayers.GetComponent<RectTransform>().rect.height + DisplacementVector.y, DisplacementVector.z), PlaySettingsPanel.transform.localRotation));
            PanelPlayerList[i].gameObject.SetActive(true);
            PanelPlayerList[i].transform.SetParent(PlaySettingsPanel.transform);

            GeneratingDifferentFeatures(PanelPlayerList[i], map.UnitStartLocations.Count, i);

            PlaceNumbersList.Add(Instantiate(NumberText, new Vector3(), NumberText.transform.rotation));
            PlaceNumbersList[i].transform.SetParent(MapView.transform);
            PlaceNumbersList[i].transform.localPosition = new Vector3(-scale * (map.SizeMap / 2 - map.UnitStartLocations[i][0]), -scale * (map.SizeMap / 2 - map.UnitStartLocations[i][2]), 0);
            PlaceNumbersList[i].text = (i + 1).ToString();

            TypeOfPlayerTmpList.Add(0);
            PlaceNumbersTmpList.Add(0);
            ColorUnitsTmpList.Add(i);
        }
        ChangeTypeOfPlayer();
    }

    private void GeneratingDifferentFeatures(GameObject panel, int countPlayers, int whichColour)
    {
        Dropdown[] dropdowns = panel.GetComponentsInChildren<Dropdown>(true);

        dropdowns[1].value = whichColour;

        List<string> placeList = new List<string>();
        placeList.Add("Random");
        for (int j = 0; j < countPlayers; j++)
        {
            placeList.Add((j + 1).ToString());
        }

        dropdowns[2].options.Clear();
        dropdowns[2].AddOptions(placeList);
        dropdowns[2].value = 0;
    }

    internal void ChangeTypeOfPlayer()
    {
        for (int i = 0; i < TypeOfPlayerTmpList.Count; i++)
        {
            Dropdown[] dropdowns = PanelPlayerList[i].GetComponentsInChildren<Dropdown>(true);

            if (i != 0 && dropdowns[0].value == 0)
            {
                dropdowns[2].value = 0;

                dropdowns[1].gameObject.SetActive(false);
                dropdowns[2].gameObject.SetActive(false);
            }
            else
            {
                dropdowns[1].gameObject.SetActive(true);
                dropdowns[2].gameObject.SetActive(true);
            }

            TypeOfPlayerTmpList[i] = dropdowns[0].value;
        }
    }

    internal void ChangeColorUnits()
    {
        for (int i = 0; i < ColorUnitsTmpList.Count; i++)
        {
            Dropdown[] dropdowns = PanelPlayerList[i].GetComponentsInChildren<Dropdown>(true);

            if (ColorUnitsTmpList[i] == dropdowns[1].value)
            {
                ColorUnitsTmpList[i] = dropdowns[1].value;
                continue;
            }

            for (int j = 0; j < ColorUnitsTmpList.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                Dropdown[] dropdownsTmp = PanelPlayerList[j].GetComponentsInChildren<Dropdown>(true);
                if (dropdownsTmp[1].value == dropdowns[1].value)
                {
                    dropdownsTmp[1].value = ColorUnitsTmpList[i];
                    break;
                }
            }

            ColorUnitsTmpList[i] = dropdowns[1].value;
        }
    }

    internal void ChangeNumberPlace()
    {
        for (int i = 0; i < PlaceNumbersTmpList.Count; i++)
        {
            Dropdown[] dropdowns = PanelPlayerList[i].GetComponentsInChildren<Dropdown>(true);

            if (PlaceNumbersTmpList[i] == dropdowns[2].value || dropdowns[2].value == 0)
            {
                PlaceNumbersTmpList[i] = dropdowns[2].value;
                continue;
            }

            for (int j = 0; j < PlaceNumbersTmpList.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                Dropdown[] dropdownsTmp = PanelPlayerList[j].GetComponentsInChildren<Dropdown>(true);
                if (dropdownsTmp[2].value == dropdowns[2].value)
                {
                    dropdownsTmp[2].value = 0;
                    break;
                }
            }

            PlaceNumbersTmpList[i] = dropdowns[2].value;
        }
    }

    ///--------------------------------------------------------
    internal void Play()
    {
        Map map = FileMapSystem.LoadMap(MapsDropdown.options[MapsDropdown.value].text);
        MapToPlayStorage.Map = map;

        List<GameStartPoint> gameStartPoints = new List<GameStartPoint>();
        List<Material> materialList = MapToPlayStorage.ImportResources<Material>("Materials/Units/", ".mat");

        for (int i = 0; i < PanelPlayerList.Count; i++)
        {
            Dropdown[] dropdowns = PanelPlayerList[i].GetComponentsInChildren<Dropdown>(true);

            if (dropdowns[0].value == 0)
            {
                continue;
            }

            if (dropdowns[2].value == 0)
            {
                dropdowns[2].value = RandomNumberPlace();
                PlaceNumbersTmpList[i] = dropdowns[2].value;
            }

            gameStartPoints.Add(new GameStartPoint()
            {
                UnitMaterial = materialList.Where(n => n.name == dropdowns[1].options[dropdowns[1].value].text).FirstOrDefault(),
                UnitStartLocation = new Vector3(map.UnitStartLocations[dropdowns[2].value - 1][0], map.UnitStartLocations[dropdowns[2].value - 1][1], map.UnitStartLocations[dropdowns[2].value - 1][2]),
            });
        }

        MapToPlayStorage.GameStartPoints = gameStartPoints;
    }

    private int RandomNumberPlace()
    {
        List<int> possibleValues = new List<int>();

        for (int i = 0; i < PlaceNumbersTmpList.Count; i++)
        {
            if(!PlaceNumbersTmpList.Any(t => t == i + 1))
            possibleValues.Add(i + 1);
        }
        //possibleValues = possibleValues.Where(n => !PlaceNumbersTmpList.Any(t => t == n)).ToList();

        System.Random rnd = new System.Random();
        return possibleValues[rnd.Next(0, possibleValues.Count)];
    }

}
