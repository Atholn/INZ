using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HumanUnit : Unit
{
    internal bool IsDead = false;
    protected float timeDeath = 10f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        base.Update();



    }
}
