using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : HumanUnit
{
    private enum SoldierTask
    {
        idle, run, follow, attack, dead
    }

    private SoldierTask task = SoldierTask.idle;

    private const string ANIMATOR_RUNNING = "Run",
                         ANIMATOR_DEAD = "Dead",
                         ANIMATOR_ATTACK = "Attack";

    private bool run = false;
    private bool attack = false;
    private bool deading = false;

    private const float _stoppingDistance = 1.5f,
                        _attackLenghtTime = 2f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);

        if (IsDead)
        {
            task = SoldierTask.dead;
            nav.velocity = Vector3.zero;
            timmer = 0;
            IsDead = false;
            deading = true;
        }

        switch (task)
        {
            case SoldierTask.idle: Idling(); break;
            case SoldierTask.run: Moving(); break;
            case SoldierTask.follow: Following(); break;
            case SoldierTask.attack: Attack(); break;
            case SoldierTask.dead: Death(); break;
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
            task = SoldierTask.idle;
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

        distance = Vector3.Magnitude(nav.destination - transform.position);

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
            task = SoldierTask.idle;
            /// to do zeby reagowal na inne jednostki
            /// near enemies
            return;
        }

        nav.velocity = Vector3.zero;
        attack = true;

        timmer += Time.deltaTime;
        if (timmer > _attackLenghtTime)
        {
            timmer = 0;
            target.gameObject.GetComponent<Unit>().Hp -= base.AttackPower;

            if (GetComponentInParent<BowShot>() != null)
            {
                BowShoting();
            }

            if (target.gameObject.GetComponent<Unit>().Hp <= 0)
            {

                //if (target.gameObject.GetComponent<HumanUnit>() != null)
                //    target.gameObject.GetComponent<HumanUnit>().IsDead = true;

                nav.velocity = Vector3.zero;
                target = null;
                task = SoldierTask.idle;
                /// to do zeby reagowal na inne jednostki
                /// near enemies

                return;
            }
        }
    }

    private void BowShoting()
    {
        //    BowShot bowShot = GetComponent<BowShot>();
        //    //GameObject arrow = (Instantiate(bowShot.Arrow, new Vector3 (0,0,0) , bowShot.Arrow.transform.rotation));
        //    GameObject Arrow = bowShot.Arrow;
        //    ////arrow.transform.SetParent(transform);
        //    //arrow.transform.localPosition = new Vector3(transform.position.x, 3, transform.position.z);
        //    //arrow.GetComponent<Arrow>().SetLandingPlace(target.transform.position);


        //    GameObject newArrow = Instantiate(Arrow, transform.position + new Vector3(0,2,0), Arrow.transform.rotation);
        //    //newArrow.transform.rotation = Arrow.transform.rotation;
        //    //newArrow.transform.position = transform.position;
        //    Rigidbody rb = newArrow.GetComponent<Rigidbody>();

        //    //newArrow.GetComponent<Arrow>().SetLandingPlace(target.transform.position);
        //    newArrow.GetComponent<Arrow>().target = target.position;
        //    //rb.velocity = transform.forward * 30;
        //    //rb.MovePosition
    }

    private void Following()
    {
        nav.SetDestination(target.position);
        distance = Vector3.Magnitude(nav.destination - transform.position);

        if (distance > _stoppingDistance)
        {
            run = true;
            attack = false;
        }

        if (distance <= _stoppingDistance)
        {
            nav.velocity = Vector3.zero;
            run = false;
            attack = false;
        }
    }

    private void Moving()
    {
        distance = Vector3.Magnitude(nav.destination - transform.position);

        if (distance > _stoppingDistance)
        {
            run = true;
            attack = false;
            return;
        }

        run = false;
        attack = false;
        nav.velocity = Vector3.zero;
        task = SoldierTask.idle;
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
        task = SoldierTask.run;
    }

    void CommandPlayer(GameObject gameObject)
    {
        target = gameObject.transform;
        task = SoldierTask.follow;
    }

    void CommandEnemy(GameObject gameObject)
    {
        target = gameObject.transform;
        task = SoldierTask.attack;
    }
}
