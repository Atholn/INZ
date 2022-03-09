using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUnit : MonoBehaviour
{
    public int GoldCost;
    public int WoodCost;
    public GameObject Unit;

    private BuildingUnit buildingParent;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        GetComponent<RawImage>().texture = Unit.GetComponent<Unit>().Profile;
    }

    void Update()
    {

    }

    public void ClickUpdate()
    {
        //if (gameManager._players[0].actualGold >= Unit.GetComponent<Unit>().GoldCost &&
        //    gameManager._players[0].actualWood >= Unit.GetComponent<Unit>().WoodCost)
        {
            //gameManager.UpdateWood(0, -Unit.GetComponent<Unit>().WoodCost);
            //gameManager.UpdateGold(0, -Unit.GetComponent<Unit>().GoldCost);
            gameManager.UnitsPrefabs.IndexOf(Unit);
            Debug.LogError("a"+gameManager.UnitsPrefabs.IndexOf(Unit));
            //buildingParent = gameManager.actualClickBuild;
            //buildingParent.CreateUnit(Unit, 0);
        }
    }
}
