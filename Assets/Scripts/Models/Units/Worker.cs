using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Worker : HumanUnit
{
    protected override void Start()
    {
        base.Start();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);

        switch (task)
        {
            case Task.idle: Idling(); break;
            case Task.move: Moving(); break;
            case Task.follow: Following(); break;
            case Task.build: Building(); break;
            case Task.repair: Repairing(); break;
            case Task.chopping: Chopping(); break;
        }



        Animate();

    }

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

    void Command(GameObject gameObject)
    {
        if (gameObject.GetComponent<BuildingUnit>() != null)
        {
            BuildingUnit buildingUnit = gameObject.GetComponent<BuildingUnit>();
            target = gameObject.transform;

            if (buildingUnit.BuildingPercent < buildingUnit.CreateTime)
            {
                task = Task.build;
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

    //void Command(BuildingUnit building)
    //{
    //    target = building.transform;
    //    task = Task.build;
    //}

    //void Command(BuildingUnit building)
    //{

    //}

    public enum Task
    {
        idle, move, follow, build, repair, chopping
    }

    const string ANIMATOR_RUNNING = "Run",
    ANIMATOR_DEAD = "Dead",
    ANIMATOR_ATTACK = "Attack",
    ANIMATOR_WOOD = "Wood",
    ANIMATOR_GOLD = "Gold",
    ANIMATOR_BUILD = "Building",
    ANIMATOR_CHOPPING = "Chopping";

    protected Task task = Task.idle;
    protected NavMeshAgent nav;
    protected Animator animator;
    protected float attackDistance = 1,
                    attackCooldown = 1,
                    attackDamage = 0,
                    stoppingDistance = 1,
                    buildingDistance = 0.5f,
                    choppingDistance = 2,
                    stopChoppingDistance = 7;

    internal Tree choppingTree;
    protected Transform target;
    bool running = false;
    bool chopping = false;
    int woodInBack = 0;
    int woodMax = 100;
    bool goToChopping = false;

    protected virtual void Animate()
    {
        //var speedVector = nav.velocity;
        //speedVector.y = 0;
        //float speed = speedVector.magnitude;
        if (animator == null) return;
        animator.SetBool(ANIMATOR_RUNNING, running);
        //animator.SetBool(ANIMATOR_DEAD, destroy);
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

    protected virtual void Following()
    {
        nav.SetDestination(new Vector3(target.position.x, 0, target.position.z));
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (distance <= stoppingDistance)
        {
            nav.velocity = Vector3.zero;
            running = false;
            task = Task.idle;
            target = null;
        }
    }

    protected virtual void Building()
    {
        nav.SetDestination(new Vector3(target.position.x, 0, target.position.z));
        //float distance = Vector3.Magnitude(nav.destination - new Vector3(transform.position.x, 0, transform.position.z));
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        BuildingUnit bU = target.GetComponent<BuildingUnit>();
        ItemGame it = target.GetComponent<ItemGame>();

        Debug.LogError(distance);
        if (distance > bU.SizeBuilding)
        {

            animator.SetBool(ANIMATOR_BUILD, false);
            running = true;
            return;
        }

        if (distance <= bU.SizeBuilding)
        {
            nav.velocity = Vector3.zero;
            running = false;
        }

        animator.SetBool(ANIMATOR_BUILD, true);
        if (bU.BuildingPercent < bU.CreateTime)
        {
            bU.BuildingPercent += Time.deltaTime;

            target.transform.position = new Vector3(target.transform.position.x, -it.HeightBuilding + (bU.BuildingPercent / bU.CreateTime) * (it.ItemHeightPosY + it.HeightBuilding), target.transform.position.z);
            return;
        }


        animator.SetBool(ANIMATOR_BUILD, false);
        task = Task.idle;
    }

    protected virtual void Repairing()
    {
        nav.SetDestination(new Vector3(target.position.x, 0, target.position.z));
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        BuildingUnit bU = target.GetComponent<BuildingUnit>();
        ItemGame it = target.GetComponent<ItemGame>();

        if (distance > bU.SizeBuilding)
        {

            animator.SetBool(ANIMATOR_BUILD, false);
            running = true;
            return;
        }

        if (distance <= bU.SizeBuilding)
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
        if (animator.GetInteger(ANIMATOR_WOOD) < woodMax)
        {
            goToChopping = true;
        }
        else if (animator.GetInteger(ANIMATOR_WOOD) >= woodMax)
        {
            goToChopping = false;
        }
        else
        {
            return;
        }

        if (goToChopping && target == null)
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

        nav.SetDestination(target.position);
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (goToChopping)
        {
            if (distance > choppingDistance)
            {
                animator.SetBool(ANIMATOR_CHOPPING, false);
                running = true;
                return;
            }

            if (distance <= choppingDistance && animator.GetInteger(ANIMATOR_WOOD) < woodMax)
            {
                if (target == null)
                {
                    target = SearchNearTreePlace();
                    return;
                }

                nav.velocity = Vector3.zero;
                running = false;
                animator.SetBool(ANIMATOR_CHOPPING, true);
                animator.SetInteger(ANIMATOR_WOOD, animator.GetInteger(ANIMATOR_WOOD) + 1);

                target.transform.GetComponent<Tree>().ChoppingProcess(1);

                if (animator.GetInteger(ANIMATOR_WOOD) >= woodMax)
                {
                    goToChopping = false;
                    running = true;

                    target = SearchNearWoodPlace();
                    if (target == null)
                    {
                        task = Task.idle;
                        running = false;
                    }
                    animator.SetBool(ANIMATOR_CHOPPING, false);
                    return;
                }
                return;
            }
        }
        else
        {
            animator.SetBool(ANIMATOR_CHOPPING, false);
            if (distance > stopChoppingDistance)
            {
                running = true;
                return;
            }

            if (distance <= stopChoppingDistance)
            {
                //nav.velocity = Vector3.zero;

                animator.SetInteger(ANIMATOR_WOOD, 0);
                goToChopping = true;

                target = SearchNearTreePlace();

                if (target == null)
                {
                    task = Task.idle;
                    running = false;
                }

                return;
            }

        }

    }

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

    private Transform SearchNearTreePlace()
    {
        return GameObject.FindGameObjectsWithTag("Tree").OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).Select(x => x.transform).FirstOrDefault(); ;
    }

}
