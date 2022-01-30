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

        transform.position = new Vector3(transform.position.x, HeightPosY, transform.position.z);

        if (Hp <= 0)
        {
            IsDead = true;
        }
    }

    protected void UpdateDistance()
    {
        if (target != null)
        {
            nav.SetDestination(target.position);
        }
        distance = CalculateLenghtStraightLine(nav.destination, transform.position);
    }

    private float CalculateLenghtStraightLine(Vector3 firstVector, Vector3 secondVector)
    {
        return Vector3.Magnitude(new Vector3(firstVector.x, 0, firstVector.z) - new Vector3(secondVector.x, 0, secondVector.z));
    }
}
