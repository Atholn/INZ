using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Worker : HumanUnit
{
    private enum WorkerTask
    {
        idle, run, follow, build, repair, dig, chop, dead
    }

    private WorkerTask task = WorkerTask.idle;

    private const string ANIMATOR_DEAD = "Dead",
                 ANIMATOR_RUN = "Run",
                 ANIMATOR_BUILD = "Build",
                 ANIMATOR_CHOP = "Chop",
                 ANIMATOR_GOLD = "Gold",
                 ANIMATOR_WOOD = "Wood";

    private bool dead = false;
    private bool run = false;
    private bool build = false;
    private bool chop = false;
    private int wood = 0;
    private int gold = 0;

    private const float _stoppingDistance = 1,
                        buildingDistance = 0.5f,
                        choppingDistance = 1,
                        stopChoppingDistance = 0.5f,
                        stopDiggingDistance = 1f;

    int goToChopping = -1; // 1 go to 0 chopping -1 go to wood place 
    bool goToDigging = false;

    float choppingTime = 5f;
    float diggingTime = 3f;
    Transform goldMineTarget;

    GameObject GoldBag;
    GameObject Woods;
    GameObject Axe;
    GameObject Hammer;
    GameObject Pick;
    GameManager _gameManager;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "GoldBag")
            {
                GoldBag = transform.GetChild(i).gameObject;
            }
            if (transform.GetChild(i).name == "Woods")
            {

                Woods = transform.GetChild(i).gameObject;
            }
        }

        GoldBag.SetActive(false);
        Woods.SetActive(false);

        _gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (isDead && !dead)
        {
            task = WorkerTask.dead;
            nav.velocity = Vector3.zero;
            timmer = 0;
            isDead = false;
            dead = true;
        }

        switch (task)
        {
            case WorkerTask.idle: Idling(); break;
            case WorkerTask.run: Running(); break;
            case WorkerTask.follow: Following(); break;
            case WorkerTask.build: Building(); break;
            case WorkerTask.repair: Repairing(); break;
            case WorkerTask.dig: Digging(); break;
            case WorkerTask.chop: Chopping(); break;
            case WorkerTask.dead: Death(); break;
        }

        Animate();
    }

    #region Tasks
    private void Idling()
    {
        nav.velocity = Vector3.zero;

        run = false;
        build = false;
        chop = false;
    }

    private void Running()
    {
        UpdateDistance();

        if (distance > _stoppingDistance)
        {
            return;
        }

        run = false;
        nav.velocity = Vector3.zero;
        task = WorkerTask.idle;
    }

    private void Following()
    {
        UpdateDistance();

        if (distance > _stoppingDistance)
        {
            run = true;
            return;
        }

        run = false;
        nav.velocity = Vector3.zero;
    }

    private void Building()
    {
        UpdateDistance(true);

        if (distance > _stoppingDistance)
        {
            return;
        }

        nav.velocity = Vector3.zero;
        run = false;
        build = true;

        BuildingUnit bU = target.GetComponent<BuildingUnit>();
        ItemGame it = target.GetComponent<ItemGame>();

        if (bU.BuildingPercent < bU.CreateTime)
        {
            bU.BuildingPercent += Time.deltaTime;
            bU.DustOn();
            target.transform.position = new Vector3(target.transform.position.x, -it.HeightBuilding + (bU.BuildingPercent / bU.CreateTime) * (it.ItemHeightPosY + it.HeightBuilding), target.transform.position.z);

            return;
        }

        bU.DustOff();
        bU.PointerPosition = new Vector3(target.transform.position.x, 0.45f, target.transform.position.z - (bU.Size / 2) - 1);
        bU.UpdateUnitPoints(WhichPlayer);

        ifSearchNearBuildingPoint = false;
        build = false;
        target = null;
        task = WorkerTask.idle;
    }

    private void Repairing()
    {
        UpdateDistance(true);

        if (distance > _stoppingDistance)
        {
            return;
        }

        nav.velocity = Vector3.zero;
        run = false;
        build = true;

        BuildingUnit bU = target.GetComponent<BuildingUnit>();

        if (bU.Hp < bU.HpMax)
        {
            bU.Hp += 1;
            target.gameObject.GetComponent<BuildingUnit>().UpdateFire();
            return;
        }

        ifSearchNearBuildingPoint = false;
        build = false;
        target = null;
        task = WorkerTask.idle;
    }

    private void Digging()
    {
        if (animator.GetInteger(ANIMATOR_GOLD) == 0)
        {
            goToDigging = true;
        }
        else if (animator.GetInteger(ANIMATOR_GOLD) > 0)
        {
            goToDigging = false;
        }

        if (goToDigging && target == null)
        {
            nav.velocity = Vector3.zero;
            run = false;
            target = null;
            task = WorkerTask.idle;
        }

        UpdateDistance(true, true);

        if (goToDigging)
        {
            if (distance > choppingDistance)
            {
                gold = 0;
                run = true;
                return;
            }

            nav.velocity = Vector3.zero;
            timmer = 0;

            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }

            gold = 10;
        }
        else
        {
            if (timmer < diggingTime)
            {
                timmer += Time.deltaTime;
                if (timmer > diggingTime)
                {
                    foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
                    {
                        renderer.enabled = true;
                    }
                    goldMineTarget = target;
                    target.GetComponent<GoldMine>().DiggingGoldmine();
                    target = SearchNearGoldPlace();

                    run = true;
                    GoldBag.SetActive(true);
                }
                return;
            }

            if (distance > stopDiggingDistance)
            {
                return;
            }

            gold = 0;
            GoldBag.SetActive(false);
            _gameManager.UpdateGold(WhichPlayer, 10);

            if (goldMineTarget == null)
            {
                nav.velocity = Vector3.zero;
                run = false;
                target = null;
                task = WorkerTask.idle;
                return;
            }

            target = goldMineTarget;
        }
    }

    private void Chopping()
    {
        if (animator.GetInteger(ANIMATOR_WOOD) == 0 && !animator.GetBool(ANIMATOR_CHOP))
        {
            goToChopping = -1;
        }
        else if (animator.GetInteger(ANIMATOR_WOOD) == 0 && animator.GetBool(ANIMATOR_CHOP))
        {
            goToChopping = 0;
        }
        else if (animator.GetInteger(ANIMATOR_WOOD) > 0)
        {
            goToChopping = 1;
        }

        if (goToChopping == -1 && target == null)
        {
            target = SearchNearTreePlace();

            if (target == null)
            {
                target = SearchNearWoodPlace();

                if (target == null)
                {
                    task = WorkerTask.idle;
                    run = false;
                }
            }
        }

        if (goToChopping == -1)
            UpdateDistance();
        else if (goToChopping == 1)
            UpdateDistance(true);

        if (goToChopping == -1)
        {
            if (distance > choppingDistance)
            {
                run = true;
                return;
            }

            timmer = 0;
            run = false;
            chop = true;
            wood = 0;
            nav.velocity = Vector3.zero;

            return;
        }

        if (goToChopping == 0)
        {
            if (timmer < choppingTime)
            {
                timmer += Time.deltaTime;
                return;
            }

            if (target == null)
            {
                target = SearchNearTreePlace();
                chop = false;
                return;
            }

            target.GetComponent<Tree>().ChoppingProcess();
            run = true;
            chop = false;
            wood = 10;
            target = SearchNearWoodPlace();

            Woods.SetActive(true);

            if (target == null)
            {
                task = WorkerTask.idle;
                run = false;
            }

            return;
        }

        if (goToChopping == 1)
        {
            if (distance > stopChoppingDistance)
            {
                return;
            }
            _gameManager.UpdateWood(WhichPlayer, 10);

            run = true;
            chop = false;
            wood = 0;
            target = SearchNearTreePlace();
            Woods.SetActive(false);

            return;
        }
    }

    private void Death()
    {
        if (timmer > timeDeath)
        {
            Destroy(gameObject);
            return;
        }

        timmer += Time.deltaTime;
    }

    private void Animate()
    {
        animator.SetBool(ANIMATOR_DEAD, dead);
        animator.SetBool(ANIMATOR_RUN, run);
        animator.SetBool(ANIMATOR_BUILD, build);
        animator.SetBool(ANIMATOR_CHOP, chop);
        animator.SetInteger(ANIMATOR_GOLD, gold);
        animator.SetInteger(ANIMATOR_WOOD, wood);
    }
    #endregion

    #region Search
    private Transform SearchNearWoodPlace()
    {
        List<List<GameObject>> list = GameObject.FindObjectOfType<GameManager>()._playersGameObjects;

        int search = -1;
        for (int i = 0; i < list.Count(); i++)
        {
            for (int j = 0; j < list[i].Count(); j++)
            {
                if (list[i][j].transform == transform) search = i;
            }
        }

        List<GameObject> buildings = new List<GameObject>();
        if (search != -1)
        {
            for (int i = 0; i < list[search].Count(); i++)
            {
                BuildingUnit build = list[search][i].GetComponent<BuildingUnit>();

                if (build != null && build.PlaceWood)
                {
                    buildings.Add(list[search][i]);
                }
            }
        }

        return buildings.OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
            .Select(x => x.transform)
            .FirstOrDefault();
    }

    private Transform SearchNearGoldPlace()
    {
        List<List<GameObject>> list = GameObject.FindObjectOfType<GameManager>()._playersGameObjects;

        int search = -1;
        for (int i = 0; i < list.Count(); i++)
        {
            for (int j = 0; j < list[i].Count(); j++)
            {
                if (list[i][j].transform == transform) search = i;
            }
        }

        List<GameObject> buildings = new List<GameObject>();
        if (search != -1)
        {
            for (int i = 0; i < list[search].Count(); i++)
            {
                BuildingUnit build = list[search][i].GetComponent<BuildingUnit>();

                if (build != null && build.PlaceGold)
                {
                    buildings.Add(list[search][i]);
                }
            }
        }

        return buildings.OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
            .Select(x => x.transform)
            .FirstOrDefault();
    }

    private Transform SearchNearTreePlace()
    {
        return GameObject
            .FindGameObjectsWithTag("Tree")
            .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
            .Select(x => x.transform)
            .FirstOrDefault(); ;
    }

    private Transform SearchNearGoldminePlace()
    {
        return GameObject.FindGameObjectsWithTag("Goldmine")
            .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
            .Select(x => x.transform)
            .FirstOrDefault();
    }
    #endregion

    #region Commands
    void Command(Vector3 destination)
    {
        if (Hp < 0)
        {
            return;
        }

        run = true;
        build = false;
        chop = false;

        target = null;
        nav.SetDestination(destination);
        task = WorkerTask.run;
    }

    void Command(Tree tree)
    {
        if (Hp < 0)
        {
            return;
        }

        run = true;
        build = false;
        chop = false;

        target = tree.transform;
        task = WorkerTask.chop;
    }

    void Command(GoldMine goldMine)
    {
        if (Hp < 0)
        {
            return;
        }

        run = true;
        build = false;
        chop = false;

        target = goldMine.transform;
        task = WorkerTask.dig;
    }

    void Command(GameObject gameObject)
    {
        if (Hp < 0)
        {
            return;
        }

        if (gameObject.GetComponent<BuildingUnit>() != null)
        {
            BuildingUnit buildingUnit = gameObject.GetComponent<BuildingUnit>();
            target = gameObject.transform;

            run = true;

            if (buildingUnit.BuildingPercent < buildingUnit.CreateTime)
            {
                task = WorkerTask.build;
                ifSearchNearBuildingPoint = false;
                return;
            }

            if (buildingUnit.Hp < HpMax)
            {
                task = WorkerTask.repair;
                return;
            }

            task = WorkerTask.follow;
        }
    }
    #endregion

    #region AICommands
    void CommandStop()
    {
        if (Hp < 0)
        {
            return;
        }

        run = false;
        target = null;

        nav.velocity = Vector3.zero;
        task = WorkerTask.idle;
    }

    void SearchTree()
    {
        if (Hp < 0)
        {
            return;
        }

        target = SearchNearTreePlace();
        task = WorkerTask.chop;
    }
    void SearchGoldmine()
    {
        if (Hp < 0)
        {
            return;
        }

        target = SearchNearGoldminePlace();
        task = WorkerTask.dig;
    }
    #endregion
}
