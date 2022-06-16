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

    }
    internal bool ifcollision = false;
}

