using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Worker : HumanUnit
{
    GameObject GoldBag;
    GameObject Woods;
    GameManager _gameManager;

    protected override void Start()
    {
        base.Start();
    }

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

    protected override void Update()
    {
        base.Update();

        if (IsDead)
        {
            task = Task.dead;
            nav.velocity = Vector3.zero;
            timmer = 0;
            IsDead = false;
            deading = true;
        }

        switch (task)
        {
            case Task.idle: Idling(); break;
            case Task.move: Moving(); break;
            case Task.follow: Following(); break;
            case Task.build: Building(); break;
            case Task.repair: Repairing(); break;
            case Task.chopping: Chopping(); break;
            case Task.digging: Digging(); break;
            case Task.dead: Death(); break;
        }

        Animate();
    }

    public enum Task
    {
        idle, move, follow, build, repair, chopping, digging, dead
    }

    const string ANIMATOR_RUNNING = "Run",
    ANIMATOR_DEAD = "Dead",
    ANIMATOR_ATTACK = "Attack",
    ANIMATOR_WOOD = "Wood",
    ANIMATOR_GOLD = "Gold",
    ANIMATOR_BUILD = "Building",
    ANIMATOR_CHOPPING = "Chopping";

    protected Task task = Task.idle;

    protected float attackDistance = 1,
                    attackCooldown = 1,
                    attackDamage = 0,
                    stoppingDistance = 1,
                    buildingDistance = 0.5f,
                    choppingDistance = 1,
                    stopChoppingDistance = 0.5f,
                    stopDiggingDistance = 1f;

    internal Tree choppingTree;
    bool running = false;
    bool deading = false;
    bool chopping = false;
    int woodInBack = 0;
    int goToChopping = -1; // 1 go to 0 chopping -1 go to wood place 
    bool goToDigging = false;


    float choppingTime = 5f;
    float diggingTime = 3f;
    Transform goldMineTarget;


    protected virtual void Animate()
    {
        //var speedVector = nav.velocity;
        //speedVector.y = 0;
        //float speed = speedVector.magnitude;
        if (animator == null) return;
        animator.SetBool(ANIMATOR_RUNNING, running);
        animator.SetBool(ANIMATOR_DEAD, deading);
    }

    protected virtual void Idling()
    {
        nav.velocity = Vector3.zero;
    }

    protected virtual void Moving()
    {
        animator.SetBool(ANIMATOR_BUILD, false);
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (distance > stoppingDistance)
        {
            running = true;
        }

        if (distance <= stoppingDistance)
        {
            running = false;
            task = Task.idle;
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

    protected virtual void Following()
    {
        nav.SetDestination(target.position);
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (distance <= stoppingDistance)
        {
            nav.velocity = Vector3.zero;
            running = false;
            task = Task.idle;
            target = null;
        }
    }

    private Vector3 SearchNearBuildingPoint(int size)
    {
        List<x> xs = new List<x>();
        Vector3 vector3 = new Vector3(target.position.x + (size / 2), 0, target.position.z);
        nav.SetDestination(vector3);
        xs.Add(new x() { pos = vector3 });

        vector3 = new Vector3(target.position.x + (size / 2), 0, target.position.z + (size / 2));
        nav.SetDestination(vector3);
        xs.Add(new x() { pos = vector3 });

        vector3 = new Vector3(target.position.x, 0, target.position.z + (size / 2));
        nav.SetDestination(vector3);
        xs.Add(new x() { pos = vector3 });

        vector3 = new Vector3(target.position.x - (size / 2), 0, target.position.z + (size / 2));
        nav.SetDestination(vector3);
        xs.Add(new x() { pos = vector3 });

        vector3 = new Vector3(target.position.x - (size / 2), 0, target.position.z);
        nav.SetDestination(vector3);
        xs.Add(new x() { pos = vector3 });

        vector3 = new Vector3(target.position.x - (size / 2), 0, target.position.z - (size / 2));
        nav.SetDestination(vector3);
        xs.Add(new x() { pos = vector3 });

        vector3 = new Vector3(target.position.x, 0, target.position.z - (size / 2));
        nav.SetDestination(vector3);
        xs.Add(new x() { pos = vector3 });

        vector3 = new Vector3(target.position.x + (size / 2), 0, target.position.z - (size / 2));
        nav.SetDestination(vector3);
        xs.Add(new x() { pos = vector3 });
        
        foreach(x tmpX in xs)
        {
            tmpX.distance = Vector3.Magnitude(tmpX.pos - transform.position);
        }

        return xs.OrderBy(x => x.distance).Select(x => x.pos).FirstOrDefault();
    }

    private class x
    {
        public Vector3 pos;
        public float distance;
    }

    bool searchtarget = false;
    Vector3 nearPos;

    private void Building()
    {
        BuildingUnit bU = target.GetComponent<BuildingUnit>();
        ItemGame it = target.GetComponent<ItemGame>();

        if(!searchtarget)
        {
            nearPos = SearchNearBuildingPoint(bU.Size);
            searchtarget = true;
        }

        nav.SetDestination(nearPos);
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        //Debug.LogError(distance);
        if (distance > bU.SizeBuilding / 2)
        {
            animator.SetBool(ANIMATOR_BUILD, false);
            running = true;
            return;
        }

        nav.velocity = Vector3.zero;
        running = false;
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
        task = Task.idle;
    }

    protected virtual void Repairing()
    {
        BuildingUnit bU = target.GetComponent<BuildingUnit>();

        nav.SetDestination(new Vector3(target.position.x, 0, target.position.z - bU.Size / 2));
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (distance > bU.SizeBuilding/2)
        {

            animator.SetBool(ANIMATOR_BUILD, false);
            running = true;
            return;
        }

        if (distance <= bU.SizeBuilding/2)
        {
            nav.velocity = Vector3.zero;
            running = false;
        }

        animator.SetBool(ANIMATOR_BUILD, true);
        if (bU.Hp < bU.HpMax)
        {
            bU.Hp += 1;

            // todo mniej ogni
            return;
        }

        animator.SetBool(ANIMATOR_BUILD, false);
        task = Task.idle;
    }

    protected virtual void Chopping()
    {
        if (animator.GetInteger(ANIMATOR_WOOD) == 0 && !animator.GetBool(ANIMATOR_CHOPPING))
        {
            goToChopping = -1;
        }
        else if (animator.GetInteger(ANIMATOR_WOOD) == 0 && animator.GetBool(ANIMATOR_CHOPPING))
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
                    task = Task.idle;
                    running = false;
                }
            }

        }

        if (goToChopping == -1)
            nav.SetDestination(target.position);
        else if (goToChopping == 1)
            nav.SetDestination(target.position + new Vector3(0,0, -target.GetComponent<BuildingUnit>().SizeBuilding/2));
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (goToChopping == -1)
        {
            if (distance > choppingDistance)
            {
                running = true;
                return;
            }

            timmer = 0;
            animator.SetBool(ANIMATOR_CHOPPING, true);
            animator.SetInteger(ANIMATOR_WOOD, 0);
            running = false;
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
            animator.SetBool(ANIMATOR_CHOPPING, false);
            animator.SetInteger(ANIMATOR_WOOD, 10);
            running = true;
            target = SearchNearWoodPlace();
            Woods.SetActive(true);

            if (target == null)
            {
                task = Task.idle;
                running = false;
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
            animator.SetBool(ANIMATOR_CHOPPING, false);
            animator.SetInteger(ANIMATOR_WOOD, 0);
            running = true;
            target = SearchNearTreePlace();
            Woods.SetActive(false);

            return;
        }
    }

    protected virtual void Digging()
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
            running = false;
            target = null;
            task = Task.idle;
        }

        if (goToDigging)
            nav.SetDestination(new Vector3(target.position.x, target.position.y, target.position.z - 4f));
        else
        {
            if(target.GetComponent<BuildingUnit>()!=null)
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
                running = true;
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

                    running = true;
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
                running = false;
                target = null;
                task = Task.idle;
                return;
            }

            target = goldMineTarget;
        }
    }

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
        return GameObject.FindGameObjectsWithTag("Tree").OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).Select(x => x.transform).FirstOrDefault(); ;
    }

    private Transform SearchNearGoldminePlace()
    {
        return GameObject.FindGameObjectsWithTag("Goldmine").OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).Select(x => x.transform).FirstOrDefault(); ;
    }
    #endregion

    #region Commands
    void Command(Vector3 destination)
    {
        nav.SetDestination(destination);
        task = Task.move;
    }

    void Command(Tree tree)
    {
        target = tree.transform;
        task = Task.chopping;
    }

    void Command(GoldMine goldMine)
    {
        target = goldMine.transform;
        task = Task.digging;
    }

    void Command(GameObject gameObject)
    {
        if (gameObject.GetComponent<BuildingUnit>() != null)
        {
            BuildingUnit buildingUnit = gameObject.GetComponent<BuildingUnit>();
            target = gameObject.transform;

            if (buildingUnit.BuildingPercent < buildingUnit.CreateTime)
            {
                task = Task.build;
                searchtarget = false;
                return;
            }

            if (buildingUnit.Hp < HpMax)
            {
                task = Task.repair;
                return;
            }

            task = Task.follow;
        }
    }

    #endregion

    #region AI
    #endregion

    #region AICommands
    void CommandStop()
    {
        target = null;
        nav.velocity = Vector3.zero;
        running = false;
        task = Task.idle;
    }

    void SearchTree()
    {
        target = SearchNearTreePlace();
        task = Task.chopping;
    }
    void SearchGoldmine()
    {
        target = SearchNearGoldminePlace();
        task = Task.digging;
    }
    #endregion
}
