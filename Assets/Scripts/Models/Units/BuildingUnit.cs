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

    internal Vector3 PointerPosition;

    public bool CanCreateUnit = false;
    /// <summary>
    /// unit
    /// </summary>
    internal bool createUnit = false;
    //internal GameObject acutalUnitCreate;
    private List<GameObject> queueCreateUnit = new List<GameObject>();
    private List<float> queueCreateUnitProgress = new List<float>();
    internal float unitCreateProgress;
    private int whichPlayerUnit;

    private GameManager gameManager;

    public int UnitToCreatePoints = 0;
    protected override void Start()
    {
        base.Start();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }
    protected override void Update()
    {
        UpdateCreate();

        base.Update();
        if (Hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    internal void CreateUnit(GameObject unitToCreate, int whichPlayer)
    {
        createUnit = true;
        whichPlayerUnit = whichPlayer;
        queueCreateUnit.Add(unitToCreate);
        queueCreateUnitProgress.Add(0f);
    }

    internal GameObject HowUnitInID(int i)
    {
        return queueCreateUnit[i];
    }

    internal void CancelCreateUnit(int i)
    {
        queueCreateUnit.Remove(queueCreateUnit[i]);
        queueCreateUnitProgress.Remove(queueCreateUnitProgress[i]);

        if (queueCreateUnit.Count == 0 && queueCreateUnitProgress.Count == 0)
        {
            createUnit = false;
        }
    }

    private void UpdateCreate()
    {
        if (!createUnit)
        {
            return;
        }

        if(gameManager._players[0].actualUnitsPoint + queueCreateUnit[0].GetComponent<HumanUnit>().UnitPoint > gameManager._players[0].actualMaxUnitsPoint)
        {
            return;
        }

        queueCreateUnitProgress[0] += Time.deltaTime;

        if (queueCreateUnitProgress[0] > queueCreateUnit[0].GetComponent<Unit>().CreateTime)
        {
            gameManager.UnitCreate(whichPlayerUnit, queueCreateUnit[0], transform.position + new Vector3(0, 0, -SizeBuilding / 2));
            queueCreateUnit.Remove(queueCreateUnit[0]);
            queueCreateUnitProgress.Remove(queueCreateUnitProgress[0]);

            if (queueCreateUnit.Count == 0 && queueCreateUnitProgress.Count == 0)
            {
                createUnit = false;
            }
        }
    }

    internal void UpdateProgressInfo(ref Texture2D[] textures, ref float val)
    {
        for (int i = 0; i < textures.Length; i++)
        {
            textures[i] = queueCreateUnit[i].GetComponent<Unit>().Profile;
            
        }
        val = queueCreateUnitProgress[0] / queueCreateUnit[0].GetComponent<Unit>().CreateTime;
    }

    internal int GetActualQueueLength()
    {
        return queueCreateUnit.Count;
    }



    ////
    public void UpdateUnitPoints(int whichPlayer)
    {
        gameManager.UpdateUnitPoints(whichPlayer);
    }


}
