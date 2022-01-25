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

    private enum Directors
    {
        right = 0,
        down = 1,
        left = 2,
        up = 3
    }

    Directors directors = Directors.right;
    int acutalSteps = 1;
    int steps = 2; // +2 
    int stepsMax = 8; // +8 
    float sizeStep;

    private void SearchBuildPlace()
    {
        if (!ifStartPos)
        {
            GameObject startBuilding = gameManager._playersGameObjects[whichPlayer].Where(g => g.GetComponent<BuildingUnit>() != null).FirstOrDefault();
            Vector3 startPos = startBuilding.transform.position;
            sizeStep = buildingTarget.GetComponent<ItemGame>().ItemPrefab.GetComponent<BuildingUnit>().SizeBuilding;
            startPos = new Vector3(startPos.x - startPos.x % 1, 1, startPos.z - startPos.z % 1 + sizeStep);

            //sizeStep = sizeStep/10f;

            image = Instantiate(buildingTarget.GetComponent<ItemGame>().ItemImageComputer, startPos, buildingTarget.transform.rotation);
            ifStartPos = true;
        }

        if (!image.GetComponent<FollowScriptComputer>().ifcollision)
        {

            switch (directors)
            {
                case Directors.right:
                    image.transform.position += new Vector3(sizeStep, 0, 0);
                    Debug.Log(0 + " " + acutalSteps);
                    if(acutalSteps >= steps)
                    {
                        acutalSteps = 0;
                        directors = Directors.down;
                    }
                    break;
                case Directors.down:
                    image.transform.position += new Vector3(0, 0, -sizeStep);
                    Debug.Log(1 + " " + acutalSteps);
                    if (acutalSteps >= steps)
                    {
                        acutalSteps = 0;
                        directors = Directors.left;
                    }
                    break;
                case Directors.left:
                    image.transform.position += new Vector3(-sizeStep, 0, 0);
                    Debug.Log(2 + " " + acutalSteps);
                    if (acutalSteps >= steps)
                    {
                        acutalSteps = 0;
                        directors = Directors.up;
                    }
                    break;
                case Directors.up:
                    image.transform.position += new Vector3(0, 0, sizeStep);
                    Debug.Log(3 + " " + acutalSteps);
                    if (acutalSteps >= steps +1)
                    {
                        acutalSteps = 0;
                        directors = Directors.right;
                        //image.transform.position += new Vector3(0, 0, sizeStep);
                        steps += 2;
                    }
                    break;
            }
            acutalSteps += 1;
            image.GetComponent<FollowScriptComputer>().ifcollision = true;
            return;
        }


        gameManager.UpdateWood(whichPlayer, -gameManager.ItemControllers[gameManager.CurrentButtonPressed].item.GetComponent<Unit>().WoodCost);
        gameManager.UpdateGold(whichPlayer, -gameManager.ItemControllers[gameManager.CurrentButtonPressed].item.GetComponent<Unit>().GoldCost);

        buildingTarget = (Instantiate(buildingTarget,
            new Vector3(image.transform.position.x, -(gameManager.ItemControllers[gameManager.CurrentButtonPressed].item as ItemGame).HeightBuilding, image.transform.position.z),
            gameManager.ItemControllers[gameManager.CurrentButtonPressed].item.ItemPrefab.transform.rotation));

        gameManager._playersGameObjects[whichPlayer].Add(buildingTarget);
        gameManager.DestroyItemImages();
        gameManager.building = false;

        gameManager._playersGameObjects[whichPlayer][gameManager._playersGameObjects[whichPlayer].Count - 1].GetComponent<MeshRenderer>().materials[1].color = gameManager._playersMaterials[whichPlayer].color;
        gameManager._playersGameObjects[whichPlayer][gameManager._playersGameObjects[whichPlayer].Count - 1].GetComponent<Unit>().whichPlayer = whichPlayer;



        //todo zmiana do tego jak computer bedzie chcial budowac

        gameManager._playersGameObjects[whichPlayer].Where(g => g.GetComponent<Worker>() != null).FirstOrDefault().GetComponent<Worker>().SendMessage("Command", buildingTarget, SendMessageOptions.DontRequireReceiver); ;

        directors = Directors.right;
        ifStartPos = false;
        computerTaskBuilding = ComputerTaskBuilding.building;
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
