using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUnit : Unit
{
    public int Size;
    public bool PlaceWood;
    public bool PlaceGold;

    public float HeightBuilding;
    public float SizeBuilding;
    internal float BuildingPercent = 0f;

    internal bool createUnit = false;
    private float unitCreateProgress;
    private GameObject acutalUnitCreate;
    private int whichPlayerUnit;

    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }
    private void Update()
    {
        UpdateCreate();
    }

    internal void CreateUnit(GameObject unitToCreate, int whichPlayer)
    {
        createUnit = true;
        unitCreateProgress = 0f;
        whichPlayerUnit = whichPlayer;
        acutalUnitCreate = unitToCreate;
    }

    private void UpdateCreate()
    {
        if (!createUnit)
        {
            return;
        }

        unitCreateProgress += Time.deltaTime;
        Debug.LogError(unitCreateProgress % 60);

        if (unitCreateProgress > acutalUnitCreate.GetComponent<Unit>().CreateTime)
        {
            //Debug.LogError("Create!");
            //Debug.LogError(buildingParent.transform.position);

            gameManager.UnitCreate(whichPlayerUnit, acutalUnitCreate, transform.position);
            unitCreateProgress = 0f;
            acutalUnitCreate = null;
            createUnit = false;
        }
    }
}
