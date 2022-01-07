using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HumanUnit : Unit
{
    public enum Task
    {
        idle, move, follow, build, chopping
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
                    buildingDistance = 2,
                    choppingDistance = 2;

    protected Transform target;
    bool running = false;
    bool chopping = false;
    int woodInBack = 0;
    int woodMax = 1000;
    bool goToChopping = false;

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        switch (task)
        {
            case Task.idle: Idling(); break;
            case Task.move: Moving(); break;
            case Task.follow: Following(); break;
            case Task.build: Building(); break;
            case Task.chopping: Chopping(); break;
        }

        Animate();
    }

    protected virtual void Animate()
    {
        //var speedVector = nav.velocity;
        //speedVector.y = 0;
        //float speed = speedVector.magnitude;



        animator.SetBool(ANIMATOR_RUNNING, running);
        //animator.SetBool(ANIMATOR_DEAD, destroy);
    }

    protected virtual void Idling()
    {
        nav.velocity = Vector3.zero;
    }

    protected virtual void Moving()
    {
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

    }

    protected virtual void Building()
    {
        float distance = Vector3.Magnitude(nav.destination - transform.position);
        if (distance <= stoppingDistance)
        {
            task = Task.build;
        }
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

        nav.SetDestination(target.position);
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (goToChopping)
        {
            if (distance > choppingDistance)
            {
                running = true;
                return;
            }

            if (distance <= choppingDistance && animator.GetInteger(ANIMATOR_WOOD) < woodMax)
            {
                nav.velocity = Vector3.zero;
                running = false;
                animator.SetBool(ANIMATOR_CHOPPING, true);
                animator.SetInteger(ANIMATOR_WOOD, animator.GetInteger(ANIMATOR_WOOD) + 10);

                //Debug.Log(animator.GetInteger(ANIMATOR_WOOD));

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
            if (distance > choppingDistance)
            {
                running = true;
                return;
            }

            if (distance <= choppingDistance)
            {
                nav.velocity = Vector3.zero;
                //running = false;
                animator.SetInteger(ANIMATOR_WOOD, 0);
                goToChopping = false;

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
