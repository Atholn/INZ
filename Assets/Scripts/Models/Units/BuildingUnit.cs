using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUnit : Unit
{
    public int Size;
    public bool PlaceWood;
    public bool PlaceGold;

    public float SizeBuilding;
    internal float BuildingPercent = 0f;


    /// <summary>
    /// unit
    /// </summary>
    internal bool createUnit = false;
    internal GameObject acutalUnitCreate;
    internal float unitCreateProgress;
    private int whichPlayerUnit;

    private GameManager gameManager;
    protected override void Start()
    {
        base.Start();
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

    internal void UpdateProgressInfo(ref Texture2D texture, ref float val)
    {
        texture = acutalUnitCreate.GetComponent<Unit>().Profile;
        val = unitCreateProgress / acutalUnitCreate.GetComponent<Unit>().CreateTime;
    }
}
