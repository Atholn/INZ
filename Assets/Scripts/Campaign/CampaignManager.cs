using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    private List<CampaignMapPoint> campaignMapPoints;
    private CampaignCameraControll campaignCameraControll;
    private int actualTarget = 0;
    private int availableTarget;

    void Start()
    {
        campaignCameraControll = GameObject.FindObjectOfType<CampaignCameraControll>();
        campaignMapPoints = GameObject.FindObjectsOfType<CampaignMapPoint>().OrderBy(cmp => cmp.name).ToList();
    }

    private void InitializePlayerCampaignSettings()
    {
        //todo
        //read files , or dont files create

    }

    void Update()
    {
        UpdateNextPreviousKeys();
    }

    private void UpdateNextPreviousKeys()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextCampaignMap();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousCampaignMap();
        }
    }

    internal void NextCampaignMap()
    {
        if (actualTarget + 1 >= campaignMapPoints.Count)
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

}
