using System.Collections.Generic;
using System.Linq;
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

    protected void UpdateDistance(bool buildingTarget = false)
    {
        if (target != null)
        {
            if (buildingTarget && target.GetComponent<BuildingUnit>() != null)
            {
                nav.SetDestination(new Vector3(target.position.x + 3, 0, target.position.z + 3));
            }
            else
            {
                nav.SetDestination(target.position);
            }
        }

        distance = CalculateLenghtStraightLine(nav.destination, transform.position);
    }

    private float CalculateLenghtStraightLine(Vector3 firstVector, Vector3 secondVector)
    {
        return Vector3.Magnitude(new Vector3(firstVector.x, 0, firstVector.z) - new Vector3(secondVector.x, 0, secondVector.z));
    }

    protected Vector3 SearchNearBuildingPoint(int size)
    {
        List<VectorDistance> VectorsDistances = new List<VectorDistance>();
        Vector3 vector3;

        vector3 = new Vector3(target.position.x + (size / 2), 0, target.position.z);
        nav.SetDestination(vector3);
        VectorsDistances.Add(new VectorDistance() { pos = vector3 });

        vector3 = new Vector3(target.position.x + (size / 2), 0, target.position.z + (size / 2));
        nav.SetDestination(vector3);
        VectorsDistances.Add(new VectorDistance() { pos = vector3 });

        vector3 = new Vector3(target.position.x, 0, target.position.z + (size / 2));
        nav.SetDestination(vector3);
        VectorsDistances.Add(new VectorDistance() { pos = vector3 });

        vector3 = new Vector3(target.position.x - (size / 2), 0, target.position.z + (size / 2));
        nav.SetDestination(vector3);
        VectorsDistances.Add(new VectorDistance() { pos = vector3 });

        vector3 = new Vector3(target.position.x - (size / 2), 0, target.position.z);
        nav.SetDestination(vector3);
        VectorsDistances.Add(new VectorDistance() { pos = vector3 });

        vector3 = new Vector3(target.position.x - (size / 2), 0, target.position.z - (size / 2));
        nav.SetDestination(vector3);
        VectorsDistances.Add(new VectorDistance() { pos = vector3 });

        vector3 = new Vector3(target.position.x, 0, target.position.z - (size / 2));
        nav.SetDestination(vector3);
        VectorsDistances.Add(new VectorDistance() { pos = vector3 });

        vector3 = new Vector3(target.position.x + (size / 2), 0, target.position.z - (size / 2));
        nav.SetDestination(vector3);
        VectorsDistances.Add(new VectorDistance() { pos = vector3 });

        foreach (VectorDistance vectorDistance in VectorsDistances)
        {
            vectorDistance.distance = Vector3.Magnitude(vectorDistance.pos - transform.position);
        }

        return VectorsDistances.OrderBy(x => x.distance).Select(x => x.pos).FirstOrDefault();
    }

    private class VectorDistance
    {
        public Vector3 pos;
        public float distance;
    }

}
