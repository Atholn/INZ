using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelCreateUnit : MonoBehaviour
{ 
    internal int ButtonID;

    private BuildingUnit buildingParent;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public void CancelUnit()
    {
        buildingParent = gameManager.actualClickBuild;

        GameObject unitToCancel = buildingParent.HowUnitInID(ButtonID);
        gameManager.UpdateWood(0, unitToCancel.GetComponent<Unit>().WoodCost);
        gameManager.UpdateGold(0, unitToCancel.GetComponent<Unit>().GoldCost);

        buildingParent.CancelCreateUnit(ButtonID);
    }
}
