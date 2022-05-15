using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject TopPanel;
    public GameObject MenuPanel;
    public GameObject BottomPanel;
    public GameObject WinLosePanel;

    public GameObject OneCharacterPanel;
    public GameObject ManyCharactersPanel;
    public GameObject CreateProgressPanel;

    public GameObject RawMaterialsPanel;

    public GameObject ActionPanel;
    private GameObject _tmpSpecialPanel;
    public GameObject WorkerSpecialPanel;
    public GameObject TownHallSpecialPanel;
    public GameObject BarracksSpecialPanel;
    public GameObject BlackSmithsSpecialPanel;
    public GameObject HintsPanel;
    public GameObject ErrorsPanel;

    private List<GameObject> _tmpActions = new List<GameObject>();

    private Text[] RawMaterials = new Text[3];

    private RawImage[] _characterImages;
    private Text[] _characterInfos;
    private List<RawImage> _images = new List<RawImage>();

    private Button[] buttonsCancelCreate;

    private List<Text> hintsTexts;
    private List<Image> hintsIcons;

    private List<Text> errorsTexts;
    private List<Image> errorsIcons;
    private float showTime = 0f;
    private float showTimeMax = 2f;
    private bool showErrors = false;

    private void Start()
    {
        WinLosePanel.SetActive(false);

        WorkerSpecialPanel.SetActive(false);
        TownHallSpecialPanel.SetActive(false);
        BarracksSpecialPanel.SetActive(false);
        BlackSmithsSpecialPanel.SetActive(false);
        HintsPanel.SetActive(false);
        ErrorsPanel.SetActive(false);

        RawMaterials = RawMaterialsPanel.GetComponentsInChildren<Text>();
        buttonsCancelCreate = CreateProgressPanel.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttonsCancelCreate.Length; i++)
        {
            buttonsCancelCreate[i].GetComponent<CancelCreate>().ButtonID = i;
        }

        List<UpgradeUnit> upgradeUnitsNumbers = BlackSmithsSpecialPanel.GetComponentsInChildren<UpgradeUnit>().ToList();
        for (int i = 0; i < upgradeUnitsNumbers.Count; i++)
        {
            upgradeUnitsNumbers[i].NumberOfUpgrade = i;
        }

        hintsTexts = HintsPanel.GetComponentsInChildren<Text>().ToList();
        hintsIcons = HintsPanel.GetComponentsInChildren<Image>().ToList();
        errorsTexts = ErrorsPanel.GetComponentsInChildren<Text>().ToList();
        errorsIcons = ErrorsPanel.GetComponentsInChildren<Image>().ToList();
    }

    private void Update()
    {
        if(showErrors)
        {
            showTime += Time.deltaTime;
            if(showTime > showTimeMax)
            {
                showErrors = false;
                ErrorsPanel.SetActive(false);
            }
        }

    }

    #region TopPanel
    public void Menu()
    {
        MenuPanel.SetActive(true);
        TopPanel.SetActive(false);
        BottomPanel.SetActive(false);
        WinLosePanel.SetActive(false);
    }
    #endregion

    #region MenuPanel

    public void Exit()
    {
        SceneLoader.MainMenu();
    }

    public void Back()
    {
        MenuPanel.SetActive(false);
        TopPanel.SetActive(true);
        BottomPanel.SetActive(true);
    }

    internal void UpdateRawMaterials(int whichMaterial, int newValue, int maxPointsUnit = 0)
    {
        RawMaterials[whichMaterial].text = maxPointsUnit == 0 ? $"{newValue}" : $"{newValue}/{maxPointsUnit}";
    }
    #endregion

    #region BottomPanel

    public void SetLookBottomPanel(Color color)
    {
        BottomPanel.SetActive(true);
        BottomPanel.GetComponent<Image>().color = color;
        OneCharacterPanel.SetActive(false);
        ManyCharactersPanel.SetActive(false);

        _characterImages = OneCharacterPanel.GetComponentsInChildren<RawImage>();
        _characterInfos = OneCharacterPanel.GetComponentsInChildren<Text>();

        foreach (RawImage image in _characterImages)
        {
            image.color = color;
        }

        Color textColor = color == new Color(0, 0, 0) ? new Color(1, 1, 1) : new Color(0, 0, 0);
        foreach (Text text in _characterInfos)
        {
            text.color = textColor;
            text.text = "";
        }

        _images.Add(ManyCharactersPanel.GetComponentInChildren<RawImage>(true));
    }

    internal void SetNonProfile()
    {
        OneCharacterPanel.SetActive(false);
        ManyCharactersPanel.SetActive(false);

        //HideSpecialButtons();
        HideSpecialPanel();
    }

    internal void SetCharacterInfo(GameObject gameObject, int i)
    {
        OneCharacterPanel.SetActive(true);
        ManyCharactersPanel.SetActive(false);

        _characterImages[0].color =
            gameObject.transform.GetComponent<MeshRenderer>() == null ?
            gameObject.transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color :
            gameObject.transform.GetComponent<MeshRenderer>().materials[1].color;
        _characterImages[1].texture = gameObject.GetComponent<Unit>().Profile;
        _characterImages[1].color = new Color(255, 255, 255);

        Unit unit = gameObject.GetComponent<Unit>();
        _characterInfos[0].text = unit.Name;
        _characterInfos[1].text = $"Attack {unit.AttackPower}";
        _characterInfos[2].text = $"Defense {unit.Defense}";
        _characterInfos[3].text = $"{unit.Hp} / {unit.HpMax}";

        //HideSpecialButtons();
        HideSpecialPanel();

        BuildingUnit buildingUnit = gameObject.GetComponent<BuildingUnit>();
        if (buildingUnit != null && buildingUnit.CreateTime > buildingUnit.BuildingPercent)
        {
            return;
        }

        if (i == 0)
        {
            switch (gameObject.GetComponent<Unit>().Name)
            {
                //Human Units
                case "Worker":
                    //ShowSpecialButtons(WorkerActions); 
                    ShowSpecialPanel(WorkerSpecialPanel);
                    break;

                // Building Units
                case "TownHall":
                    ShowSpecialPanel(TownHallSpecialPanel);
                    break;
                case "Barracks":
                    ShowSpecialPanel(BarracksSpecialPanel);
                    break;
                case "Blacksmiths":
                    ShowSpecialPanel(BlackSmithsSpecialPanel);
                    break;
                default: break;
            }
        }
    }

    internal void ShowProgressCreateUnitPanel(GameObject gameObject)
    {
        BuildingUnit buildingUnit = gameObject.GetComponent<BuildingUnit>();

        if (buildingUnit != null && buildingUnit.createUnit)
        {
            CreateProgressPanel.SetActive(true);

            int queuesLength = buildingUnit.GetActualQueueLength();
            Texture2D[] textures = new Texture2D[queuesLength > buttonsCancelCreate.Length ? buttonsCancelCreate.Length : queuesLength];
            float value = 0f;
            buildingUnit.UpdateProgressInfo(ref textures, ref value);

            for (int i = 0; i < buttonsCancelCreate.Length; i++)
            {
                if (i < textures.Length)
                {
                    RawImage rawImage = buttonsCancelCreate[i].GetComponent<RawImage>();
                    rawImage.texture = textures[i];
                    rawImage.color = new Color(255, 255, 255, 255);
                    buttonsCancelCreate[i].gameObject.SetActive(true);
                }
                else
                {
                    buttonsCancelCreate[i].gameObject.SetActive(false);
                }
            }
            CreateProgressPanel.GetComponentInChildren<Slider>().value = value;
        }
        else
        {
            CreateProgressPanel.SetActive(false);
        }
    }

    private void HideSpecialPanel()
    {
        if (_tmpSpecialPanel != null) _tmpSpecialPanel.SetActive(false);
        _tmpSpecialPanel = null;
    }

    private void ShowSpecialPanel(GameObject gameObject)
    {
        _tmpSpecialPanel = gameObject;
        _tmpSpecialPanel.SetActive(true);
    }

    private void HideSpecialButtons()
    {
        foreach (GameObject button in _tmpActions)
        {
            button.SetActive(false);
        }
        _tmpActions.Clear();
    }

    private void ShowSpecialButtons(List<GameObject> gameObjects)
    {
        foreach (GameObject button in gameObjects)
        {
            button.SetActive(true);
            _tmpActions.Add(button);
        }
    }

    internal void SetCharactersProfiles(List<GameObject> selectUnits, int maxSelected)
    {
        HideSpecialPanel();

        OneCharacterPanel.SetActive(false);
        ManyCharactersPanel.SetActive(true);

        for (int i = _images.Count - 1; i > 0; i--)
        {
            Destroy(_images[i].gameObject);
            _images.Remove(_images[i]);
        }

        _images[0].texture = selectUnits[0].GetComponent<Unit>().Profile;

        for (int i = 0; i < selectUnits.Count; i++)
        {
            _images.Add(Instantiate(_images[0],
                new Vector3(
                    _images[0].transform.position.x + (((i + 1) % (maxSelected / 2)) * (_images[0].GetComponent<RectTransform>().rect.width + 10)),
                    i + 1 > (maxSelected / 2) - 1 ? (int)_images[0].transform.position.y - (_images[0].GetComponent<RectTransform>().rect.height + 10) : (int)_images[0].transform.position.y,
                    0),
                _images[0].transform.rotation));
            _images[i].transform.SetParent(ManyCharactersPanel.transform);
            _images[i].texture = selectUnits[i].GetComponent<Unit>().Profile;
        }
    }
    #region ActionPanel
    public void ActionPanelShowHide(GameObject panel)
    {
        if (ActionPanel.gameObject.activeSelf)
        {
            ActionPanel.gameObject.SetActive(false);
            panel.SetActive(true);
            return;
        }

        ActionPanel.gameObject.SetActive(true);
        panel.SetActive(false);
    }
    #endregion

    #endregion


    #region WinLosePanel
    public void ShowWinner(int winner, Color winnerColor)
    {
        TopPanel.SetActive(false);
        MenuPanel.SetActive(false);
        BottomPanel.SetActive(false);
        WinLosePanel.SetActive(true);

        WinLosePanel.GetComponent<Image>().color = new Color(winnerColor.r, winnerColor.g, winnerColor.b, 150 / 255f);
        WinLosePanel.GetComponentInChildren<Text>().text = winner == 0 ? "You win!" : $"You lose!\nPlayer {winner} win!";
    }
    #endregion

    #region hints
    internal void ShowHints(string[] texts)
    {
        hintsTexts[0].text = texts[0];
        hintsTexts[1].text = texts[1];

        if (texts.Length == 3)
        {
            hintsTexts[2].gameObject.SetActive(true);
            hintsIcons[3].gameObject.SetActive(true);
            hintsTexts[2].text = texts[2];
        }
        else
        {
            hintsTexts[2].gameObject.SetActive(false);
            hintsIcons[3].gameObject.SetActive(false);
        }

        HintsPanel.SetActive(true);
    }

    public void HideHints()
    {
        HintsPanel.SetActive(false);
    }

    public void ShowErrors(bool[] errors)
    {
        if(!errors[0] && !errors[1] && !errors[2])
        {
            ErrorsPanel.SetActive(false);
            showErrors = false;
            return;
        }

        ErrorsPanel.SetActive(true);
        for (int i = 0; i < errors.Length; i++)
        {
            errorsTexts[i].gameObject.SetActive(errors[i]);
            errorsIcons[i+1].gameObject.SetActive(errors[i]);          
        }

        if(errors.Length == 2)
        {
            errorsTexts[2].gameObject.SetActive(false);
            errorsIcons[3].gameObject.SetActive(false);
        }

        showTime = 0f;
        showErrors = true;
    }

    #endregion
}
