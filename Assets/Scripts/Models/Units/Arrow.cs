using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Arrow : MonoBehaviour
{
    //private float timmer = 0f;
    //private float timmerMax = 10f;
    //private bool ifLanding = false;
    //private NavMeshAgent nav;
    internal Vector3 target;

    //private void Update()
    //{
    //    //float distance = Vector3.Magnitude(nav.destination - transform.position);
    //    //if (distance < 1f && !ifLanding)
    //    //{
    //    //    nav.velocity = Vector3.zero;
    //    //    ifLanding = true;
    //    //}
        
    //    //if(ifLanding)
    //    //{
    //    //    nav.velocity = Vector3.zero;
    //    //    timmer += Time.deltaTime;

    //    //    if(timmer > timmerMax)
    //    //    {
    //    //        Destroy(gameObject);
    //    //    }
    //    //}
    //}

    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(target);//rb.velocity);
        //rb.MovePosition(new Vector3(0.01f, 0,0.01f));
    }

    //internal void SetLandingPlace(Vector3 vector3)
    //{
    //    nav = GetComponent<NavMeshAgent>();
    //    nav.SetDestination(vector3);
    //}
}
