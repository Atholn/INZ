using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CampaignUIManager : MonoBehaviour
{
    public GameObject MissionsMapPanel;
    public GameObject MissionDetailsPanel;

    private CampaignManager campaignManager;
    private List<Button> missionsMapButtons; //0 - preview, 1 - next, 2 -  enter mission, 3 - exit 

    private Text missionDetailsText;

    private void Start()
    {
        InitializeCampaignManager();
        InitializePanels();
        InitializeMissionsMapButtons();
        InitializeMissionDetails();
    }

    #region Initialize
    private void InitializeCampaignManager()
    {
        campaignManager = GameObject.FindObjectOfType<CampaignManager>();
    }

    private void InitializePanels()
    {
        EnterMissionsMapPanel();
    }

    private void InitializeMissionsMapButtons()
    {
        missionsMapButtons = MissionsMapPanel.GetComponentsInChildren<Button>().ToList();

        if (campaignManager.CheckZeroMission())
        {
            missionsMapButtons[0].gameObject.SetActive(false);
            missionsMapButtons[1].gameObject.SetActive(false);
            return;
        }

        if (campaignManager.CheckLastAvaiableMission())
        {
            missionsMapButtons[0].gameObject.SetActive(true);
            missionsMapButtons[1].gameObject.SetActive(false);
            return;
        }

        missionsMapButtons[0].gameObject.SetActive(false);
        missionsMapButtons[1].gameObject.SetActive(true);
    }

    private void InitializeMissionDetails()
    {
        missionDetailsText = MissionDetailsPanel.GetComponentInChildren<Text>();
    }
    #endregion

    private void Update()
    {
        UpdateMissionMapKeys();
        UpdateMissionDetailsKeys();
    }

    #region Update Keys
    private void UpdateMissionMapKeys()
    {
        if (!MissionsMapPanel.activeSelf)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousCampaignMissionMap();
            return;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextCampaignMissionMap();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterMissionDetailsPanel();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCampaignScene();
            return;
        }
    }

    private void UpdateMissionDetailsKeys()
    {
        if (!MissionDetailsPanel.activeSelf)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EnterMissionsMapPanel();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCampaignMap();
            return;
        }
    }

    #endregion

    #region Public methods in MissionsMapPanel
    public void PreviousCampaignMap()
    {
        PreviousCampaignMissionMap();
    }

    public void NextCampaignMap()
    {
        NextCampaignMissionMap();
    }

    public void EnterCampaignMap()
    {
        EnterMissionDetailsPanel();
    }

    public void ExitCampaignMap()
    {
        ExitCampaignScene();
    }

    #endregion

    #region Public methods in MissionDetailsPanel
    public void BackToMissionsMap()
    {
        EnterMissionsMapPanel();
    }

    public void StartMap()
    {
        StartCampaignMap();
    }

    #endregion

    #region Private methods for MissionsMapPanel
    private void PreviousCampaignMissionMap()
    {
        if (MissionsMapPanel.activeSelf)
        {
            campaignManager.PreviousCampaignMap();
        }

        if (campaignManager.CheckZeroMission())
        {
            missionsMapButtons[0].gameObject.SetActive(false);
            missionsMapButtons[1].gameObject.SetActive(false);
        }
        else
        {
            missionsMapButtons[0].gameObject.SetActive(!campaignManager.CheckFirstOrLastMission());
            missionsMapButtons[1].gameObject.SetActive(true);
        }
    }

    private void NextCampaignMissionMap()
    {
        if (MissionsMapPanel.activeSelf)
        {
            campaignManager.NextCampaignMap();
        }

        if (campaignManager.CheckZeroMission())
        {
            missionsMapButtons[0].gameObject.SetActive(false);
            missionsMapButtons[1].gameObject.SetActive(false);
        }
        else
        {
            missionsMapButtons[0].gameObject.SetActive(true);
            missionsMapButtons[1].gameObject.SetActive(!campaignManager.CheckFirstOrLastMission());
        }
    }

    private void EnterMissionDetailsPanel()
    {
        if (!campaignManager.CheckLastAvaiableMission())
        {
            missionsMapButtons[2].gameObject.SetActive(false);
            return;
        }

        MissionsMapPanel.SetActive(false);
        MissionDetailsPanel.SetActive(true);
        missionsMapButtons[2].gameObject.SetActive(true);

        missionDetailsText.text = campaignManager.SetRequaried();
    }

    private void ExitCampaignScene()
    {
        SceneLoader.MainMenu();
    }
    #endregion

    #region Private methods for MissionDetailsPanel
    private void EnterMissionsMapPanel()
    {
        MissionsMapPanel.SetActive(true);
        MissionDetailsPanel.SetActive(false);
    }

    private void StartCampaignMap()
    {
        campaignManager.StartMap();
        SceneLoader.GameScene();
    }

    #endregion
}
