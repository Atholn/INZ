using UnityEngine;

public class Soldier : HumanUnit
{
    private enum SoldierTask
    {
        idle, run, follow, attack, dead
    }

    private SoldierTask task = SoldierTask.idle;
    
    private const string ANIMATOR_DEAD = "Dead",
                         ANIMATOR_RUN = "Run",
                         ANIMATOR_ATTACK = "Attack";

    private bool dead = false;
    private bool run = false;
    private bool attack = false;

    private const float _stoppingDistance = 1.0f,
                        _attackLenghtTime = 2.0f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (isDead && !dead)
        {
            task = SoldierTask.dead;
            timmer = 0;
            dead = true;
            isDead = false;
        }

        switch (task)
        {
            case SoldierTask.idle: Idling(); break;
            case SoldierTask.run: Running(); break;
            case SoldierTask.follow: Following(); break;
            case SoldierTask.attack: Attacking(); break;
            case SoldierTask.dead: Death(); break;
        }

        Animate();
    }

    #region Tasks
    private void Idling()
    {
        nav.velocity = Vector3.zero;

        run = false;
        attack = false;
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
        task = SoldierTask.idle;
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

    private void Attacking()
    {
        if (target == null)
        {
            nav.velocity = Vector3.zero;
            task = SoldierTask.idle;

            return;
        }

        UpdateDistance(target.gameObject.GetComponent<BuildingUnit>() != null ? true : false);

        if (distance > AttackDistance)
        {
            run = true;
            attack = false;
            timmer = 0;
            return;
        }
        transform.LookAt(target);
        nav.velocity = Vector3.zero;
        attack = true;
        run = false;
        timmer += Time.deltaTime;

        if (timmer > _attackLenghtTime)
        {
            timmer = 0;
            target.gameObject.GetComponent<Unit>().Hp -= AttackPower;

            if (target.gameObject.GetComponent<BuildingUnit>() != null)
            {
                target.gameObject.GetComponent<BuildingUnit>().UpdateFire();
            }

            if (GetComponentInParent<BowShot>() != null)
            {
                transform.LookAt(target);
                BowShoting();
            }

            if (target.gameObject.GetComponent<Unit>().Hp <= 0)
            {
                nav.velocity = Vector3.zero;
                target = null;
                task = SoldierTask.idle;

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

    private void Death()
    {
        if (timmer > timeDeath)
        {
            Destroy(gameObject);
            return;
        }
        nav.velocity = Vector3.zero;
        timmer += Time.deltaTime;
    }

    private void Animate()
    {
        animator.SetBool(ANIMATOR_RUN, run);
        animator.SetBool(ANIMATOR_DEAD, dead);
        animator.SetBool(ANIMATOR_ATTACK, attack);
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
        attack = false;
        target = null;

        nav.SetDestination(destination);
        task = SoldierTask.run;
    }

    void CommandPlayer(GameObject gameObject)
    {
        if (Hp < 0)
        {
            return;
        }
        run = true;

        target = gameObject.transform;
        task = SoldierTask.follow;
    }

    void CommandEnemy(GameObject gameObject)
    {
        if (Hp < 0)
        {
            return;
        }
        run = true;

        target = gameObject.transform;
        task = SoldierTask.attack;
    }
    #endregion
}
