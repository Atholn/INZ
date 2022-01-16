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

    void Command(Tree tree)
    {
        target = tree.transform;
        task = Task.chopping;
    }

    void Command(GameObject gameObject)
    {
        if (gameObject.GetComponent<BuildingUnit>() != null)
        {
            target = gameObject.transform;
            task = Task.build;

            return;
        }
    }

    //void Command(BuildingUnit building)
    //{
    //    target = building.transform;
    //    task = Task.build;
    //}

    //void Command(BuildingUnit building)
    //{

    //}
}
