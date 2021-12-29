using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : HumanUnit
{
    protected override void Update()
    {
        base.Update();
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
    }

    void Command(Vector3 destination)
    {
        nav.SetDestination(destination);
        task = Task.move;
    }

    //void Command(Worker workerToFollow)
    //{

    //    task = Task.follow;
    //}

    //void Command(BuildingUnit building)
    //{

    //}
}
