using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateHumanUnitButton : MonoBehaviour
{
    public GameObject Building;
    public GameObject Unit;

    private BuildingUnit buildingParent;
    GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
    }


    public void Create()
    {
        if(_gameManager._players[0].actualUnitsPoint + Unit.GetComponent<HumanUnit>().UnitPoint <= _gameManager._players[0].actualMaxUnitsPoint &&
            _gameManager._players[0].actualGold >= Unit.GetComponent<Unit>().GoldCost &&
            _gameManager._players[0].actualWood >= Unit.GetComponent<Unit>().WoodCost)
        {

            _gameManager.UpdateWood(0, -Unit.GetComponent<Unit>().WoodCost);
            _gameManager.UpdateGold(0, -Unit.GetComponent<Unit>().GoldCost);

            buildingParent = _gameManager.actualClickBuild;

            if (buildingParent.createUnit)
            {
                return;
            }

            buildingParent.CreateUnit(Unit, 0);
        }
    }
}
