using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CampaignUIManager : MonoBehaviour
{
    public GameObject MissionsMapPanel;
    public GameObject MissionPanel;

    private List<Button> missionsMapButtons; //0 - preview, 1 - next, 2 -  enter mission, 3 - exit 
    private CampaignManager campaignManager;

    private void Start()
    {
        campaignManager = GameObject.FindObjectOfType<CampaignManager>();

        IntitializePanels();
        IntitializeMissionsMapButtons();
    }

    private void IntitializePanels()
    {
        MissionsMapPanel.SetActive(true);
        MissionPanel.SetActive(false);
    }

    private void IntitializeMissionsMapButtons()
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

    private void Update()
    {
        UpdateNextPreviousEnterKeys();
    }

    private void UpdateNextPreviousEnterKeys()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousCampaignMissionMap();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextCampaignMissionMap();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterMissionDetails();
        }
    }

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

    private void EnterMissionDetails()
    {
        if (!campaignManager.CheckLastAvaiableMission())
        {
            missionsMapButtons[2].gameObject.SetActive(false);
            return;
        }

        MissionsMapPanel.SetActive(false);
        MissionPanel.SetActive(true);
        missionsMapButtons[2].gameObject.SetActive(true);
    }

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
        EnterMissionDetails();
    }

    public void ExitCampaignMap()
    {
        SceneLoader.MainMenu();
    }

    #endregion

    #region Public methods in MissionPanel
    public void BackToMissionsMap()
    {
        MissionsMapPanel.SetActive(true);
        MissionPanel.SetActive(false);
    }

    public void StartMap()
    {
        campaignManager.StartMap();
        SceneLoader.GameScene();
    }

    #endregion

}
