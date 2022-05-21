using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    private readonly string _typeOfGame = "Campaign";
    private readonly string _campaignColorPlayer = "Blue";
    private readonly string _campaignColorEnemy = "Red";

    private List<CampaignMapPoint> campaignMapPoints;
    private CampaignCameraControll campaignCameraControll;
    private int actualTarget = 0;
    private int availableTarget = 0;
    private CampaignSettings campaignSettings;
    private CampaignSettingsFileSystem campaignSettingsFileSystem;
    private FileMapSystem fileMapSystem;

    void Start()
    {
        InitializeMapGameObjects();
        InitializePlayerCampaignSettings();
        InitializeFileMapLoader();
    }

    private void InitializeMapGameObjects()
    {
        campaignCameraControll = GameObject.FindObjectOfType<CampaignCameraControll>();
        campaignMapPoints = GameObject.FindObjectsOfType<CampaignMapPoint>().OrderBy(cmp => cmp.name).ToList();
    }

    private void InitializePlayerCampaignSettings()
    {
        campaignSettingsFileSystem = new CampaignSettingsFileSystem();
        campaignSettings = campaignSettingsFileSystem.LoadSettings();

        actualTarget = campaignSettings.AvailableTarget;
        availableTarget = campaignSettings.AvailableTarget;

        SetNewTargetPos(campaignMapPoints[actualTarget]);
    }

    private void InitializeFileMapLoader()
    {
        fileMapSystem = new FileMapSystem();
        fileMapSystem.FolderName = _typeOfGame;
    }

    internal Vector2 Getsss()
    {
        Transform transform = campaignMapPoints[actualTarget].GetComponent<Transform>();
        return new Vector2(transform.position.x, transform.position.z);
    }

    void Update()
    {
        if (CampaignStorage.Win)
        {
            if (CampaignStorage.MissionNumber == availableTarget)
            {
                availableTarget++;
                actualTarget++;
                campaignSettingsFileSystem.SaveSettings(availableTarget);
            }

            CampaignStorage.Win = false;
            SetNewTargetPos(campaignMapPoints[actualTarget]);
        }
    }

    internal void NextCampaignMap()
    {
        if (actualTarget + 1 >= campaignMapPoints.Count || actualTarget + 1 > availableTarget)
        {
            return;
        }

        actualTarget++;
        SetNewTargetPos(campaignMapPoints[actualTarget]);
    }

    internal void PreviousCampaignMap()
    {
        if (actualTarget - 1 < 0)
        {
            return;
        }

        actualTarget--;
        SetNewTargetPos(campaignMapPoints[actualTarget]);
    }

    internal void StartMap()
    {
        CampaignMissionsRequaried.SetRequired(actualTarget);
        MapToPlayStorage.SceneToBack = _typeOfGame;

        Map map = fileMapSystem.LoadEditorMap($"m{actualTarget}");
        MapToPlayStorage.Map = map;
        List<GameStartPoint> gameStartPoints = new List<GameStartPoint>();
        List<Material> materialList = MapToPlayStorage.ImportResources<Material>("Materials/Units/", ".mat");

        Material playerMaterial = materialList.Where(n => n.name == _campaignColorPlayer).FirstOrDefault();
        Material enemyMaterial = materialList.Where(n => n.name == _campaignColorEnemy).FirstOrDefault();

        for (int i = 0; i < map.MapWorldCreate.StartPoints.Count; i++)
        {
            if (map.MapWorldCreate.StartPoints[i].UnitStartLocation[0] != 0 && map.MapWorldCreate.StartPoints[i].UnitStartLocation[1] != 0 && map.MapWorldCreate.StartPoints[i].UnitStartLocation[1] != 0)
                gameStartPoints.Add(new GameStartPoint()
                {
                    UnitMaterial = i == 0 ? playerMaterial : enemyMaterial,
                    UnitStartLocation = new Vector3(map.MapWorldCreate.StartPoints[i].UnitStartLocation[0], 1, map.MapWorldCreate.StartPoints[i].UnitStartLocation[2]),
                });
        }

        MapToPlayStorage.GameStartPoints = gameStartPoints;

        //Campaign Storage
        CampaignStorage.MissionNumber = actualTarget;
    }

    private void SetNewTargetPos(CampaignMapPoint campaignMapPoint)
    {
        Transform transform = campaignMapPoint.GetComponent<Transform>();
        Vector2 newTargetPos = new Vector2(transform.position.x, transform.position.z);
        campaignCameraControll.GoToCampaignPoint(newTargetPos);
    }

    internal bool CheckFirstOrLastMission() =>
        actualTarget == 0 || actualTarget == campaignMapPoints.Count - 1;

    internal bool CheckLastAvaiableMission() =>
        actualTarget <= availableTarget;

    internal bool CheckZeroMission() =>
        availableTarget == 0;
}
