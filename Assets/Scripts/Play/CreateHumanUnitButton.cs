using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateHumanUnitButton : MonoBehaviour
{
    public GameObject Building;
    public GameObject Unit;

    private BuildingUnit buildingParent;
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }


    public void Create()
    {
        if(gameManager.actualUnitsPoint + Unit.GetComponent<HumanUnit>().UnitPoint <= gameManager.actualMaxUnitsPoint)
        {
            buildingParent = gameManager.actualClickBuild;

            if (buildingParent.createUnit)
            {
                return;
            }

            buildingParent.CreateUnit(Unit, 0);
        }
    }
}
