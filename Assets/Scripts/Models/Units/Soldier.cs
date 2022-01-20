using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : HumanUnit
{
    public enum Task
    {
        idle, run, follow, attack
    }

    Task task = Task.idle;
    bool dead = false;
    bool run = false;
    bool attack = false;

    const string ANIMATOR_RUNNING = "Run",
                    ANIMATOR_DEAD = "Dead",
                    ANIMATOR_ATTACK = "Attack";

    protected Transform target;
    protected NavMeshAgent nav;
    protected Animator animator;


    protected float attackDistance = 1,
                    attackCooldown = 1,
                    attackDamage = 0,
                    stoppingDistance = 2,
                    buildingDistance = 0.5f,
                    choppingDistance = 1,
                    stopChoppingDistance = 1f,
                    attacklenght = 2f;



    protected override void Start()
    {
        base.Start();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);

        switch (task)
        {
            case Task.idle: Idling(); break;
            case Task.run: Moving(); break;
            case Task.follow: Following(); break;
            case Task.attack: Attack(); break;
        }

        Animate();
    }

    private void Animate()
    {
        animator.SetBool(ANIMATOR_RUNNING, run);
        animator.SetBool(ANIMATOR_DEAD, dead);
        animator.SetBool(ANIMATOR_ATTACK, attack);
    }


    private float timeAttack = 0;
    private void Attack()
    {
        if (target == null)
        {
            nav.velocity = Vector3.zero;
            target = null;
            task = Task.idle;
            /// to do zeby reagowal na inne jednostki
            /// near enemies
            return;
        }

        if (target.GetComponent<BuildingUnit>() != null)
        {
            nav.SetDestination(new Vector3(target.position.x + 3, 0, target.position.z + 3));
        }
        else
        {
            nav.SetDestination(target.position);
        }

        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (distance > stoppingDistance)
        {
            run = true;
            timeAttack = 0;
            return;
        }

        if (distance <= stoppingDistance)
        {
            nav.velocity = Vector3.zero;
            attack = true;

            timeAttack += Time.deltaTime;
            if (timeAttack > attacklenght)
            {
                target.gameObject.GetComponent<Unit>().Hp -= base.AttackPower;
                timeAttack = 0;
            }
        }
    }

    private void Following()
    {
        nav.SetDestination(target.position);
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (distance > stoppingDistance)
        {
            run = true;
            attack = false;
        }

        if (distance <= stoppingDistance)
        {
            nav.velocity = Vector3.zero;
            run = false;
            attack = false;
        }
    }

    private void Moving()
    {
        float distance = Vector3.Magnitude(nav.destination - transform.position);

        if (distance > stoppingDistance)
        {
            run = true;
            attack = false;
        }

        if (distance <= stoppingDistance)
        {
            run = false;
            attack = false;
            nav.velocity = Vector3.zero;
            task = Task.idle;
        }
    }

    private void Idling()
    {
        nav.velocity = Vector3.zero;
        run = false;
        attack = false;
    }

    void Command(Vector3 destination)
    {
        nav.SetDestination(destination);
        task = Task.run;
    }

    void CommandPlayer(GameObject gameObject)
    {
        target = gameObject.transform;
        task = Task.follow;
    }

    void CommandEnemy(GameObject gameObject)
    {
        target = gameObject.transform;
        task = Task.attack;
    }
}
