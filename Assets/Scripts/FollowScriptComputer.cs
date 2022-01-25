using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class FollowScriptComputer : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        ifcollision = false;
        Debug.LogWarning("Trigger");
    }
    private void OnCollisionStay(Collision collision)
    {
        ifcollision = false;
        Debug.LogWarning("Trigger");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        ifcollision = false;
    }

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
    private void OnCollisionExit(Collision collision)
    {
        ifcollision = false;
        Debug.LogWarning("Trigger");
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

