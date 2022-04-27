using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignUIManager : MonoBehaviour
{
    private CampaignManager campaignManager;

    private void Awake()
    {
        campaignManager = GameObject.FindObjectOfType<CampaignManager>();
    }

    public void NextCampaignMap()
    {
        campaignManager.NextCampaignMap();
    }

    public void PreviousCampaignMap()
    {
        campaignManager.PreviousCampaignMap();
    }
}
