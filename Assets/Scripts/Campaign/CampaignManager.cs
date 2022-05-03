using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    private List<CampaignMapPoint> campaignMapPoints;
    private CampaignCameraControll campaignCameraControll;
    private int actualTarget = 0;
    private int availableTarget = 100; // todo 

    void Start()
    {
        InitializeMapGameObjects();
        InitializePlayerCampaignSettings();
    }

    private void InitializeMapGameObjects()
    {
        campaignCameraControll = GameObject.FindObjectOfType<CampaignCameraControll>();
        campaignMapPoints = GameObject.FindObjectsOfType<CampaignMapPoint>().OrderBy(cmp => cmp.name).ToList();
    }

    private void InitializePlayerCampaignSettings()
    {
        //todo
        //read files , or dont files create
        //availableTarget load
    }

    void Update()
    {

    }

    internal void NextCampaignMap()
    {
        if (actualTarget + 1 >= campaignMapPoints.Count || actualTarget + 1 >= availableTarget)
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
}
