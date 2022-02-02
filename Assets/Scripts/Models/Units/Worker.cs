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
    Vector3 nearPos;


    bool searchtarget = false;

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

        if (IsDead)
        {
            task = WorkerTask.dead;
            nav.velocity = Vector3.zero;
            timmer = 0;
            IsDead = false;
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
        BuildingUnit bU = target.GetComponent<BuildingUnit>();
        ItemGame it = target.GetComponent<ItemGame>();

        if (!searchtarget)
        {
            nearPos = SearchNearBuildingPoint(bU.Size);
            searchtarget = true;
        }

        nav.SetDestination(nearPos);
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (distance > bU.SizeBuilding / 2)
        {
            animator.SetBool(ANIMATOR_BUILD, false);
            run = true;
            return;
        }

        nav.velocity = Vector3.zero;
        run = false;
        animator.SetBool(ANIMATOR_BUILD, true);

        if (bU.BuildingPercent < bU.CreateTime)
        {
            bU.BuildingPercent += Time.deltaTime;
            target.transform.position = new Vector3(target.transform.position.x, -it.HeightBuilding + (bU.BuildingPercent / bU.CreateTime) * (it.ItemHeightPosY + it.HeightBuilding), target.transform.position.z);
            return;
        }

        BuildingUnit bu = target.GetComponent<BuildingUnit>();
        bu.PointerPosition = new Vector3(target.transform.position.x, 0.45f, target.transform.position.z - (bu.SizeBuilding / 2) - 1);
        bu.UpdateUnitPoints(whichPlayer);

        animator.SetBool(ANIMATOR_BUILD, false);
        task = WorkerTask.idle;
    }

    private void Repairing()
    {
        BuildingUnit bU = target.GetComponent<BuildingUnit>();

        nav.SetDestination(new Vector3(target.position.x, 0, target.position.z - bU.Size / 2));
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (distance > bU.SizeBuilding / 2)
        {

            animator.SetBool(ANIMATOR_BUILD, false);
            run = true;
            return;
        }

        if (distance <= bU.SizeBuilding / 2)
        {
            nav.velocity = Vector3.zero;
            run = false;
        }

        animator.SetBool(ANIMATOR_BUILD, true);
        if (bU.Hp < bU.HpMax)
        {
            bU.Hp += 1;

            // todo mniej ogni
            return;
        }

        animator.SetBool(ANIMATOR_BUILD, false);
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

        if (goToDigging)
            nav.SetDestination(new Vector3(target.position.x, target.position.y, target.position.z - 4f));
        else
        {
            if (target.GetComponent<BuildingUnit>() != null)
            {
                nav.SetDestination(target.position + new Vector3(0, 0, -target.GetComponent<BuildingUnit>().SizeBuilding / 2));
            }
            else
            {
                nav.SetDestination(new Vector3(target.position.x, target.position.y, target.position.z - 4f));
            }
        }

        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (goToDigging)
        {
            if (distance > choppingDistance)
            {
                animator.SetInteger(ANIMATOR_GOLD, 0);
                run = true;
                return;
            }

            nav.velocity = Vector3.zero;
            timmer = 0;

            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }

            animator.SetInteger(ANIMATOR_GOLD, 10);
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

            animator.SetInteger(ANIMATOR_GOLD, 0);
            GoldBag.SetActive(false);
            _gameManager.UpdateGold(whichPlayer, 10);

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
            nav.SetDestination(target.position);
        else if (goToChopping == 1)
            nav.SetDestination(target.position + new Vector3(0, 0, -target.GetComponent<BuildingUnit>().SizeBuilding / 2));
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (goToChopping == -1)
        {
            if (distance > choppingDistance)
            {
                run = true;
                return;
            }

            timmer = 0;
            animator.SetBool(ANIMATOR_CHOP, true);
            animator.SetInteger(ANIMATOR_WOOD, 0);
            run = false;
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

            target.GetComponent<Tree>().ChoppingProcess();
            animator.SetBool(ANIMATOR_CHOP, false);
            animator.SetInteger(ANIMATOR_WOOD, 10);
            run = true;
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
            _gameManager.UpdateWood(whichPlayer, 10);
            animator.SetBool(ANIMATOR_CHOP, false);
            animator.SetInteger(ANIMATOR_WOOD, 0);
            run = true;
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
        run = true;
        build = false;
        chop = false;

        nav.SetDestination(destination);
        task = WorkerTask.run;
    }

    void Command(Tree tree)
    {
        run = true;
        build = false;
        chop = false;

        target = tree.transform;
        task = WorkerTask.chop;
    }

    void Command(GoldMine goldMine)
    {
        run = true;
        build = false;
        chop = false;

        target = goldMine.transform;
        task = WorkerTask.dig;
    }

    void Command(GameObject gameObject)
    {
        if (gameObject.GetComponent<BuildingUnit>() != null)
        {
            BuildingUnit buildingUnit = gameObject.GetComponent<BuildingUnit>();
            target = gameObject.transform;

            if (buildingUnit.BuildingPercent < buildingUnit.CreateTime)
            {
                task = WorkerTask.build;
                searchtarget = false;
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
        run = false;
        target = null;

        nav.velocity = Vector3.zero;
        task = WorkerTask.idle;
    }

    void SearchTree()
    {
        target = SearchNearTreePlace();
        task = WorkerTask.chop;
    }
    void SearchGoldmine()
    {
        target = SearchNearGoldminePlace();
        task = WorkerTask.dig;
    }
    #endregion
}
