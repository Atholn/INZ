using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum TypeOfPlayer
    {
        human,
        computer
    }

    internal TypeOfPlayer typeOfPlayer;
    internal int whichPlayer;

    internal int actualUnitsPoint = 0;
    internal int actualMaxUnitsPoint = 0;
    internal int actualWood = 0;
    internal int actualGold = 0;

    GameManager gameManager;

    bool ifHaveTownHall = true;
    bool ifHaveBarracks = false;
    bool ifHaveBlackSmith = false;
    bool ifHave100UnitsPoint = false;

    bool isInProccess = false;

    internal void UpdateComputer(List<GameObject> units)
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();


        if (CheckAllBuildings() == -1) AttackStage();
        else BuildingStage();
    }

    private int CheckAllBuildings()
    {
        if (ifHaveTownHall)
        {
            if (ifHaveBarracks)
            {
                if (ifHaveBlackSmith)
                {
                    if (ifHave100UnitsPoint)
                    {
                        return -1;
                    }
                    return 4;
                }
                return 3;
            }
            return 2;
        }
        return 1;
    }

    #region BuildingStage
    private void BuildingStage()
    {
        BuildBulding();
        //switch (CheckAllBuildings())
        //{
        //    case 1: BuildBulding(); break;
        //    case 2: break;
        //    case 3: break;
        //    case 4: break;
        //}

            List<GameObject> listOfWorkers = gameManager._playersGameObjects[whichPlayer].Where(g => g.GetComponent<Worker>() !=null).ToList();
        if(actualWood < gameManager.BuildingsPrefabs[0].GetComponent<BuildingUnit>().WoodCost)
        {

            foreach (GameObject worker in listOfWorkers)
            {
                if(!isInProccess)
                {
                    worker.SendMessage("SearchTree", null, SendMessageOptions.DontRequireReceiver);
                    isInProccess = true;
                }
            }
            return;
        }
        else 
        {
            isInProccess = false;
        }


        if (actualGold < gameManager.BuildingsPrefabs[0].GetComponent<BuildingUnit>().GoldCost)
        {


            foreach (GameObject worker in listOfWorkers)
            {
                if (!isInProccess)
                {
                    worker.SendMessage("SearchGoldmine", null, SendMessageOptions.DontRequireReceiver);
                    isInProccess = true;
                }
            }
            return;
        }
        else
        {
            isInProccess = false;
        }

        Debug.LogError("juz");

    }

    private void BuildBulding()
    {



    }

    #endregion

    #region AttackStage
    private void AttackStage()
    {

    }
    #endregion
}
