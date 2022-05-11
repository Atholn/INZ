using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CampaignMissionsRequaried
{
    public static void SetRequired(int number)
    {
        switch(number)
        {
            case 0: MapToPlayStorage.AddSourcesRequaried(1, 200); break;
            case 1: MapToPlayStorage.AddSourcesRequaried(2, 300); break;
            case 2: MapToPlayStorage.AddBuildingRequaried(0, 1); break; 
            case 3: MapToPlayStorage.AddSourcesRequaried(4, 100); break;
            case 4: 
                MapToPlayStorage.AddSoldiersRequaried(1, 1);
                MapToPlayStorage.AddSoldiersRequaried(2, 1);
                MapToPlayStorage.AddSoldiersRequaried(3, 1);
                break;
            case 5: MapToPlayStorage.AddUpgradesRequaried(0, true); break;
            case 6: MapToPlayStorage.AddSoldiersRequaried(0, 20); break;
            case 7: MapToPlayStorage.AddDominateRequaried(true); break;
            case 8: MapToPlayStorage.AddSourcesRequaried(1, 3000); break;
            case 9: MapToPlayStorage.AddSourcesRequaried(2, 3000); break;
            case 10: MapToPlayStorage.AddDominateRequaried(true); break;
            case 11:
                MapToPlayStorage.AddSourcesRequaried(0, 1000);
                MapToPlayStorage.AddSourcesRequaried(3, 100);
                MapToPlayStorage.AddSourcesRequaried(4, 90);
                break;
            case 12: MapToPlayStorage.AddDominateRequaried(true); break;
            case 13: MapToPlayStorage.AddDominateRequaried(true); break;
            case 14: MapToPlayStorage.AddDominateRequaried(false); break;
            case 15: MapToPlayStorage.AddDominateRequaried(true); break;
            case 16: MapToPlayStorage.AddDominateRequaried(true); break;
            case 17: MapToPlayStorage.AddDominateRequaried(true); break;
        }

    }
}
