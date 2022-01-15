using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateHumanUnitButton : MonoBehaviour
{
    public GameObject Building;
    public GameObject Unit;

    private bool isCreate;
    private Unit unitToCreate;
    private float unitCreateProgress;
    private BuildingUnit buildingParent;
    GameManager gameManager;

    private void Start()
    {
        unitToCreate = Unit.GetComponent<Unit>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        UpdateCreate();
    }

    private void UpdateCreate()
    {
        if(!isCreate)
        {
            return;
        }
        unitCreateProgress += Time.deltaTime;
        //Debug.LogError(unitCreateProgress % 60);

        if(unitCreateProgress > unitToCreate.CreateTime)
        {
            //Debug.LogError("Create!");
            //Debug.LogError(buildingParent.transform.position);

            gameManager.UnitCreate(0, Unit, buildingParent.transform.position);

            isCreate = false;
            buildingParent = null;
        }
    }

    public void Create()
    {
        if(isCreate)
        {
            //todo 
            //sprawdzaj czy jest full do zrobienia
           
            return;
        }
        isCreate = true;
        unitCreateProgress = 0f;
        buildingParent = gameManager.actualClickBuild;
    }

}
