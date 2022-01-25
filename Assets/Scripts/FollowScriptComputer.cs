using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class FollowScriptComputer : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        ifcollision = false;
    }
    private void OnTriggerStay(Collider other)
    {
        ifcollision = false;
    }

    private void OnTriggerExit(Collider other)
    {
        ifcollision = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ifcollision = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        ifcollision = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        ifcollision = false;
    }


    internal void Check()
    {
        
        //if(ifcollision)
        //{
        //    Debug.LogError("Trigger");
        //}
    }
    internal bool ifcollision = false;
}

