using UnityEngine;
using UnityEngine.AI;

public class HumanUnit : Unit
{
    public int UnitPoint = 1;
    
    protected bool IsDead = false;
    protected float timeDeath = 10f;
    protected float timmer = 0;
    protected float distance;
    protected Transform target;
    protected NavMeshAgent nav;
    protected Animator animator;

    protected virtual void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (Hp <= 0)
        {
            IsDead = true;
        }
    }
}
