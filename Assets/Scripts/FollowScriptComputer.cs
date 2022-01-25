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


    

    internal void Check()
    {
        
        //if(ifcollision)
        //{
        //    Debug.LogError("Trigger");
        //}
    }
    internal bool ifcollision = false;
}

