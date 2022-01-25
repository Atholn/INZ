using System;
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

    public enum ComputerTask
    {
        building,
        attacking,
    }

    public enum ComputerTaskBuilding
    {
        checkBuilding,
        getRawSource,
        searchBuildPlace,
        building
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

    ComputerTask computerTask = ComputerTask.building;
    ComputerTaskBuilding computerTaskBuilding = ComputerTaskBuilding.checkBuilding;

    //checkBuilding
    GameObject buildingTarget;

    //getRawSource
    private bool ifCommandWood = false;
    private bool ifCommandGold = false;

    //searchBuildPlace
    private bool ifStartPos = false;
    private GameObject image;
    //building


    internal void UpdateComputer(List<GameObject> units)
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        switch (computerTask)
        {
            case ComputerTask.building: Building(); break;
            case ComputerTask.attacking: Attacking(); break;
        }

        //var x = CheckAllBuildings();
        //if (x == -1) AttackStage();
        //else BuildingStage(x);
    }

    #region Building
    private void Building()
    {
        switch (computerTaskBuilding)
        {
            case ComputerTaskBuilding.checkBuilding:
                CheckBuilding();
                break;
            case ComputerTaskBuilding.getRawSource:
                GetRawSource();
                break;
            case ComputerTaskBuilding.searchBuildPlace:
                SearchBuildPlace();
                break;
            case ComputerTaskBuilding.building:
                break;
        }
    }

    private void CheckBuilding()
    {
        var whichBuilding = CheckAllBuildings();
        if (whichBuilding == -1)
        {
            computerTask = ComputerTask.attacking;
            return;
        }

        buildingTarget = gameManager.BuildingsPrefabs[whichBuilding];
        computerTaskBuilding = ComputerTaskBuilding.getRawSource;
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
                    return 3;
                }
                return 2;
            }
            return 1;
        }
        return 0;
    }

    private void GetRawSource()
    {
        List<GameObject> listOfWorkers = gameManager._playersGameObjects[whichPlayer].Where(g => g.GetComponent<Worker>() != null).ToList();
        if (actualWood < buildingTarget.GetComponent<BuildingUnit>().WoodCost)
        {
            if (!ifCommandWood)
            {
                foreach (GameObject worker in listOfWorkers)
                {
                    worker.SendMessage("SearchTree", null, SendMessageOptions.DontRequireReceiver);
                }
                ifCommandWood = true;
            }

            return;
        }


        if (actualGold < buildingTarget.GetComponent<BuildingUnit>().GoldCost)
        {
            if (!ifCommandGold)
            {
                foreach (GameObject worker in listOfWorkers)
                {
                    worker.SendMessage("SearchGoldmine", null, SendMessageOptions.DontRequireReceiver);
                }
                ifCommandGold = true;
            }

            return;
        }

        //foreach (GameObject worker in listOfWorkers)
        //{
        //    worker.SendMessage("CommandStop", null, SendMessageOptions.DontRequireReceiver);
        //}

        ifCommandWood = false;
        ifCommandGold = false;
        computerTaskBuilding = ComputerTaskBuilding.searchBuildPlace;
    }

    int i = 0;
    private void SearchBuildPlace()
    {
        if (!ifStartPos)
        {
            Vector3 startPos = gameManager._playersGameObjects[whichPlayer].Where(g => g.GetComponent<BuildingUnit>() != null).FirstOrDefault().transform.position;
            startPos = new Vector3(startPos.x - startPos.x % 1, 1, startPos.z - startPos.z % 1);
            image = Instantiate(buildingTarget.GetComponent<Item>().ItemImageComputer, startPos, buildingTarget.transform.rotation);
            ifStartPos = true;
        }

        if(!image.GetComponent<FollowScriptComputer>().ifcollision)
        {
            image.transform.position += new Vector3(1f, 0, 0);
            i++;
            Debug.LogError(i);
            image.GetComponent<FollowScriptComputer>().ifcollision = true;

            return;
        }


        //Debug.LogError("tutaj!");

        //image.GetComponent<FollowScriptComputer>().Check();
        //if (image.GetComponent<FollowScriptComputer>().ifcollision)
        //{
        //    Debug.LogWarning("jest kolizja");
        //}

        //if (!image.GetComponent<FollowScriptComputer>().ifcollision)
        //{
        //   Debug.LogError("nie ma kolizji");
        //}

        //image.GetComponent<FollowScriptComputer>().ifcollision = false;
        //ifStartPos = false;
        //computerTaskBuilding = ComputerTaskBuilding.building;
    }



    #endregion

    #region Attacking
    #endregion

    private void Attacking()
    {

    }


    #region BuildingStage
    private void BuildingStage(int whichBuilding)
    {


        //building.GetComponent<BoxCollider>().c
        //listOfWorkers[0].SendMessage()
        //Debug.LogError("juz");

    }



    private Vector3 SearchNearPlaceToBuild(Vector3 workerPos, GameObject build)
    {
        var x = Instantiate(build, workerPos, build.transform.rotation);

        FollowScript fs = x.GetComponent<FollowScript>();

        //while(fs.GetComponent<BoxCollider>().)


        return new Vector3(0, 0, 0);
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
