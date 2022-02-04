using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    internal enum TypeOfPlayer
    {
        human,
        computer
    }

    private enum ComputerTask
    {
        building,
        attacking,
    }

    private enum ComputerTaskBuilding
    {
        checkBuilding,
        getRawSourceBuilding,
        searchBuildPlace,
        buildingBuilding
    }

    private enum ComputerTaskAttacking
    {
        soldierSelection,
        getRawSourceSoldier,
        creatingSoldiers,
        searchingEnemy,
        attackingEnemies,
        returnSoldiers,
    }

    internal TypeOfPlayer typeOfPlayer;
    internal int whichPlayer;

    internal int actualUnitsPoint = 0;
    internal int actualMaxUnitsPoint = 0;
    internal int actualWood = 0;
    internal int actualGold = 0;

    GameManager gameManager;

    ComputerTask computerTask = ComputerTask.building;
    ComputerTaskBuilding computerTaskBuilding = ComputerTaskBuilding.checkBuilding;
    ComputerTaskAttacking computerTaskAttacking = ComputerTaskAttacking.soldierSelection;

    #region Building Variables
    //checkBuilding
    GameObject buildingTarget;
    int whichBuilding;

    //getRawSource
    private bool ifCommandWood = false;
    private bool ifCommandGold = false;

    //searchBuildPlace
    private bool ifStartPos = false;
    private GameObject image;

    private enum Directors
    {
        right = 0,
        down = 1,
        left = 2,
        up = 3
    }

    Directors directors = Directors.right;
    float acutalSteps = 1f;
    int steps = 2;
    float sizeStep;

    //building

    #endregion

    #region Attacking Variables
    //soldierSelection
    GameObject soldierTarget;
    int whichSoldier;

    //creatingSoldiers
    private readonly int _computerSoldiersCount = 5;
    private readonly int _computerSoldiersMinCount = 4;
    int creatingSoldiers = 0;

    //attackingEnemies
    GameObject enemyTarget;
    bool commandsAttack = false;

    //returnSoldiers
    bool noHome = false;
    #endregion

    internal void UpdateComputer(List<GameObject> units)
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        if (!ifCommandGold)
        {
            List<GameObject> listOfWorkers = gameManager._playersGameObjects[whichPlayer].Where(g => g.GetComponent<Worker>() != null).ToList();

            listOfWorkers[1].SendMessage("SearchTree", null, SendMessageOptions.DontRequireReceiver);
            listOfWorkers[2].SendMessage("SearchTree", null, SendMessageOptions.DontRequireReceiver);
            listOfWorkers[3].SendMessage("SearchGoldmine", null, SendMessageOptions.DontRequireReceiver);
            listOfWorkers[4].SendMessage("SearchGoldmine", null, SendMessageOptions.DontRequireReceiver);

            ifCommandGold = true;
        }

        switch (computerTask)
        {
            case ComputerTask.building: Building(); break;
            case ComputerTask.attacking: Attacking(); break;
        }
    }

    #region Building
    private void Building()
    {
        switch (computerTaskBuilding)
        {
            case ComputerTaskBuilding.checkBuilding:
                CheckBuilding();
                break;
            case ComputerTaskBuilding.getRawSourceBuilding:
                GetRawSourceBuilding();
                break;
            case ComputerTaskBuilding.searchBuildPlace:
                SearchBuildPlace();
                break;
            case ComputerTaskBuilding.buildingBuilding:
                BuildingBuilding();
                break;
        }
    }

    private void CheckBuilding()
    {
        whichBuilding = CheckAllBuildings();
        if (whichBuilding == -1)
        {
            computerTask = ComputerTask.attacking;
            return;
        }

        buildingTarget = gameManager.BuildingsPrefabs[whichBuilding];
        computerTaskBuilding = ComputerTaskBuilding.getRawSourceBuilding;
    }

    private int CheckAllBuildings()
    {
        List<BuildingUnit> listOfBuildng = gameManager._playersGameObjects[whichPlayer].Where(g => g.GetComponent<BuildingUnit>() != null).Select(b => b.GetComponent<BuildingUnit>()).ToList();

        if (listOfBuildng.Where(b => b.Name == gameManager.BuildingsPrefabs[0].GetComponent<BuildingUnit>().name).ToList().Count != 0)
        {
            if (listOfBuildng.Where(b => b.Name == gameManager.BuildingsPrefabs[1].GetComponent<BuildingUnit>().name).ToList().Count != 0)
            {
                if (listOfBuildng.Where(b => b.Name == gameManager.BuildingsPrefabs[2].GetComponent<BuildingUnit>().name).ToList().Count != 0)
                {
                    if (listOfBuildng.Where(b => b.Name == gameManager.BuildingsPrefabs[3].GetComponent<BuildingUnit>().name).ToList().Count > 2)
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

    private void GetRawSourceBuilding()
    {
        List<GameObject> listOfWorkers = gameManager._playersGameObjects[whichPlayer].Where(g => g.GetComponent<Worker>() != null).ToList();
        if (actualWood < buildingTarget.GetComponent<BuildingUnit>().WoodCost)
        {
            return;
        }

        if (actualGold < buildingTarget.GetComponent<BuildingUnit>().GoldCost)
        {
            return;
        }
        computerTaskBuilding = ComputerTaskBuilding.searchBuildPlace;
    }

    int ii = 0;

    private void SearchBuildPlace()
    {
        if (!ifStartPos)
        {
            GameObject startBuilding = gameManager._playersGameObjects[whichPlayer].Where(g => g.GetComponent<BuildingUnit>() != null).FirstOrDefault();
            Vector3 startPos = startBuilding.transform.position;
            sizeStep = buildingTarget.GetComponent<ItemGame>().ItemPrefab.GetComponent<BuildingUnit>().SizeBuilding;
            startPos = new Vector3(startPos.x - startPos.x % 1, 1, startPos.z - startPos.z % 1 + sizeStep);

            sizeStep = sizeStep / 10f;

            image = Instantiate(buildingTarget.GetComponent<ItemGame>().ItemImageComputer, startPos, buildingTarget.transform.rotation);
            image.GetComponent<Renderer>().enabled = false;
            ifStartPos = true;
        }

        if (image == null)
        {
            computerTaskBuilding = ComputerTaskBuilding.checkBuilding;
            ifStartPos = false;
            return;
        }

        if (!image.GetComponent<FollowScriptComputer>().ifcollision)
        {
            switch (directors)
            {
                case Directors.right:
                    image.transform.position += new Vector3(sizeStep, 0, 0);
                    if (acutalSteps >= steps)
                    {
                        acutalSteps = 0f;
                        directors = Directors.down;
                    }
                    break;
                case Directors.down:
                    image.transform.position += new Vector3(0, 0, -sizeStep);
                    if (acutalSteps >= steps)
                    {
                        acutalSteps = 0f;
                        directors = Directors.left;
                    }
                    break;
                case Directors.left:
                    image.transform.position += new Vector3(-sizeStep, 0, 0);
                    if (acutalSteps >= steps)
                    {
                        acutalSteps = 0f;
                        directors = Directors.up;
                    }
                    break;
                case Directors.up:
                    image.transform.position += new Vector3(0, 0, sizeStep);
                    if (acutalSteps >= steps + 1)
                    {
                        acutalSteps = 0f;
                        directors = Directors.right;
                        steps += 2;
                    }
                    break;
            }
            acutalSteps += 0.1f;

            image.GetComponent<FollowScriptComputer>().ifcollision = true;
            return;
        }

        if (ii < 9)
        {
            image.transform.position += new Vector3(-1 + ii % 3, 0, -1 + ii / 3);
            ii++;
            return;
        }

        image.transform.position -= new Vector3(1, 0, 1);

        gameManager.UpdateWood(whichPlayer, -gameManager.BuildingsPrefabs[whichBuilding].GetComponent<Unit>().WoodCost);
        gameManager.UpdateGold(whichPlayer, -gameManager.BuildingsPrefabs[whichBuilding].GetComponent<Unit>().GoldCost);

        buildingTarget = (Instantiate(buildingTarget,
            new Vector3(image.transform.position.x, -(gameManager.ItemControllers[gameManager.CurrentButtonPressed].item as ItemGame).HeightBuilding, image.transform.position.z),
            gameManager.ItemControllers[gameManager.CurrentButtonPressed].item.ItemPrefab.transform.rotation));

        gameManager._playersGameObjects[whichPlayer].Add(buildingTarget);
        gameManager._playersGameObjects[whichPlayer][gameManager._playersGameObjects[whichPlayer].Count - 1].GetComponent<MeshRenderer>().materials[1].color = gameManager._playersMaterials[whichPlayer].color;
        gameManager._playersGameObjects[whichPlayer][gameManager._playersGameObjects[whichPlayer].Count - 1].GetComponent<Unit>().WhichPlayer = whichPlayer;
        gameManager._playersGameObjects[whichPlayer].Where(g => g.GetComponent<Worker>() != null).FirstOrDefault().GetComponent<Worker>().SendMessage("Command", buildingTarget, SendMessageOptions.DontRequireReceiver); ;

        Destroy(image);
        directors = Directors.right;
        acutalSteps = 1f;
        steps = 2;
        ii = 0;
        ifStartPos = false;
        computerTaskBuilding = ComputerTaskBuilding.buildingBuilding;
    }

    private void BuildingBuilding()
    {
        BuildingUnit bU = buildingTarget.GetComponent<BuildingUnit>();
        if (bU.BuildingPercent < bU.CreateTime)
        {
            return;
        }

        computerTaskBuilding = ComputerTaskBuilding.checkBuilding;
    }
    #endregion

    #region Attacking
    private void Attacking()
    {
        switch (computerTaskAttacking)
        {
            case ComputerTaskAttacking.soldierSelection:
                SoldierSelection();
                break;
            case ComputerTaskAttacking.getRawSourceSoldier:
                GetRawSourceSoldier();
                break;
            case ComputerTaskAttacking.creatingSoldiers:
                CreatingSoldiers();
                break;
            case ComputerTaskAttacking.searchingEnemy:
                SearchingEnemy();
                break;
            case ComputerTaskAttacking.attackingEnemies:
                AttackingEnemies();
                break;
            case ComputerTaskAttacking.returnSoldiers:
                ReturnSoldiers();
                break;
        }
    }

    private void SoldierSelection()
    {
        if (GetListUnits<Soldier>().Count() > _computerSoldiersCount)
        {
            computerTaskAttacking = ComputerTaskAttacking.attackingEnemies;
            return;
        }

        if (creatingSoldiers > _computerSoldiersCount)
        {
            //computerTaskAttacking = ComputerTaskAttacking.attackingEnemies;
            return;
        }

        whichSoldier = RandomizeNewSoldier();
        soldierTarget = gameManager.UnitsPrefabs[whichSoldier];
        creatingSoldiers++;

        computerTaskAttacking = ComputerTaskAttacking.getRawSourceSoldier;
    }

    private int RandomizeNewSoldier()
    {
        System.Random rnd = new System.Random();
        return rnd.Next(1, 4);
    }

    private void GetRawSourceSoldier()
    {
        if (actualWood < soldierTarget.GetComponent<Soldier>().WoodCost)
        {
            return;
        }

        if (actualGold < soldierTarget.GetComponent<Soldier>().GoldCost)
        {
            return;
        }

        computerTaskAttacking = ComputerTaskAttacking.creatingSoldiers;
    }

    private void CreatingSoldiers()
    {
        gameManager._playersGameObjects[whichPlayer]
            .Where(b => b.GetComponent<BuildingUnit>() != null && b.GetComponent<BuildingUnit>().Name == "Barracks")
            .Select(b => b.GetComponent<BuildingUnit>())
            .FirstOrDefault()
            .CreateUnit(soldierTarget, whichPlayer);

        computerTaskAttacking = ComputerTaskAttacking.soldierSelection;
    }

    private void SearchingEnemy()
    {
        enemyTarget = GetNearEnemy();

        commandsAttack = false;
        computerTaskAttacking = ComputerTaskAttacking.attackingEnemies;
    }

    private void AttackingEnemies()
    {
        var soldiers = GetListUnits<Soldier>();

        if (soldiers.Count == 0)
        {
            return;
        }

        if (enemyTarget == null || enemyTarget.GetComponent<Unit>().Hp < 0)
        {
            computerTaskAttacking = ComputerTaskAttacking.searchingEnemy;
            commandsAttack = false;
            return;
        }

        if (!commandsAttack)
        {
            foreach (Soldier soldier in soldiers)
            {
                soldier.SendMessage("CommandEnemy", enemyTarget, SendMessageOptions.DontRequireReceiver);
            }
            commandsAttack = true;
            return;
        }

        if (noHome)
        {
            return;
        }

        if (soldiers.Count < _computerSoldiersMinCount)
        {
            computerTaskAttacking = ComputerTaskAttacking.returnSoldiers;
            return;
        }
    }

    private void ReturnSoldiers()
    {
        var soldiers = GetListUnits<Soldier>();
        var buildings = GetListUnits<BuildingUnit>();

        if (buildings.Count == 0)
        {
            noHome = true;
            computerTaskAttacking = ComputerTaskAttacking.attackingEnemies;
            return;
        }

        foreach (Soldier soldier in soldiers)
        {
            soldier.SendMessage("Command", buildings[0].transform.position + new Vector3(0, 0, buildings[0].GetComponent<BuildingUnit>().Size), SendMessageOptions.DontRequireReceiver);
        }

        computerTaskAttacking = ComputerTaskAttacking.creatingSoldiers;
    }

    #endregion

    private GameObject GetNearEnemy()
    {
        var soldier = GetListUnits<Soldier>().FirstOrDefault();
        GameObject nearEnemy = null;
        float oldDistance = float.MaxValue;

        for (int i = 0; i < gameManager._playersGameObjects.Count; i++)
        {
            if (i == whichPlayer)
            {
                continue;
            }

            for (int j = 0; j < gameManager._playersGameObjects[i].Count; j++)
            {
                if (gameManager._playersGameObjects[i][j].GetComponent<Unit>().Hp < 0)
                {
                    continue;
                }

                if (nearEnemy == null)
                {
                    nearEnemy = gameManager._playersGameObjects[i][j];
                    oldDistance = Vector3.Magnitude(soldier.transform.position - nearEnemy.transform.position);
                    continue;
                }

                if (oldDistance > Vector3.Magnitude(nearEnemy.transform.position - gameManager._playersGameObjects[i][j].transform.position))
                {
                    nearEnemy = gameManager._playersGameObjects[i][j];
                    oldDistance = Vector3.Magnitude(soldier.transform.position - nearEnemy.transform.position);
                }
            }
        }

        return nearEnemy;
    }



    private List<T> GetListUnits<T>() where T : Unit
    {
        return gameManager._playersGameObjects[whichPlayer]
            .Where(u => u.GetComponent<T>() != null)
            .Select(u => u.GetComponent<T>())
            .ToList();
    }
}
