using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanUnit : Unit
{
    public enum Task
    {
        idle, move, follow, build
    }

    const string ANIMATOR_RUNNING = "Run",
    ANIMATOR_DEAD = "Dead",
    ANIMATOR_ATTACK = "Attack",
    ANIMATOR_WOOD = "Wood",
    ANIMATOR_GOLD = "Gold",
    ANIMATOR_BUILD = "Building";


    protected Task task = Task.idle;
    protected NavMeshAgent nav;
    protected Animator animator;
    protected float attackDistance = 1,
    attackCooldown = 1,
    attackDamage = 0,
    stoppingDistance = 1;

    bool running = true;
    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        transform.position =  new Vector3(transform.position.x, 0, transform.position.z);

        switch (task)
        {
            case Task.idle: running = false; Idling(); break;
            case Task.move: running = true; Moving(); break;
            case Task.follow: Following(); break;
            case Task.build: Building(); break;
        }

        Animate();
    }

    protected virtual void Animate()
    {
        var speedVector = nav.velocity;
        speedVector.y = 0;
        float speed = speedVector.magnitude;
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
        if (distance <= stoppingDistance)
        {
            task = Task.idle;
        }
    }
    protected virtual void Following()
    {

    }
    protected virtual void Building()
    {
        nav.velocity = Vector3.zero;
    }
}
