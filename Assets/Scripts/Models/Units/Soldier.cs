using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : HumanUnit
{
    public enum Task
    {
        idle, run, follow, attack, dead
    }

    Task task = Task.idle;
    bool run = false;
    bool attack = false;
    bool deading = false;

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

    private float timmer = 0;

    protected override void Start()
    {
        base.Start();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    internal void GetComponents()
    {
        base.Start();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {

        base.Update();
        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);

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
            case Task.run: Moving(); break;
            case Task.follow: Following(); break;
            case Task.attack: Attack(); break;
            case Task.dead: Death(); break;
        }

        Animate();
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
        animator.SetBool(ANIMATOR_RUNNING, run);
        animator.SetBool(ANIMATOR_DEAD, IsDead);
        animator.SetBool(ANIMATOR_ATTACK, attack);
    }

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

        if (distance > AttackDistance)
        {
            run = true;
            timmer = 0;
            return;
        }


        if (target == null)
        {
            nav.velocity = Vector3.zero;
            target = null;
            task = Task.idle;
            /// to do zeby reagowal na inne jednostki
            /// near enemies
            return;
        }

        nav.velocity = Vector3.zero;
        attack = true;

        timmer += Time.deltaTime;
        if (timmer > attacklenght)
        {
            timmer = 0;
            target.gameObject.GetComponent<Unit>().Hp -= base.AttackPower;

            if (GetComponentInParent<BowShot>() != null)
            {
                BowShoting();
            }

            if (target.gameObject.GetComponent<Unit>().Hp <= 0)
            {

                if (target.gameObject.GetComponent<HumanUnit>() != null)
                    target.gameObject.GetComponent<HumanUnit>().IsDead = true;

                nav.velocity = Vector3.zero;
                target = null;
                task = Task.idle;
                /// to do zeby reagowal na inne jednostki
                /// near enemies

                return;
            }
        }
    }

    private void BowShoting()
    {
        BowShot bowShot = GetComponent<BowShot>();
        //GameObject arrow = (Instantiate(bowShot.Arrow, new Vector3 (0,0,0) , bowShot.Arrow.transform.rotation));
        GameObject Arrow = bowShot.Arrow;
        ////arrow.transform.SetParent(transform);
        //arrow.transform.localPosition = new Vector3(transform.position.x, 3, transform.position.z);
        //arrow.GetComponent<Arrow>().SetLandingPlace(target.transform.position);


        GameObject newArrow = Instantiate(Arrow, transform.position + new Vector3(0,2,0), Arrow.transform.rotation);
        //newArrow.transform.rotation = Arrow.transform.rotation;
        //newArrow.transform.position = transform.position;
        Rigidbody rb = newArrow.GetComponent<Rigidbody>();

        //newArrow.GetComponent<Arrow>().SetLandingPlace(target.transform.position);
        newArrow.GetComponent<Arrow>().target = target.position;
        //rb.velocity = transform.forward * 30;
        //rb.MovePosition
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
