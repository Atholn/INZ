using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    RaycastHit hit;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Input.GetKeyDown("space"))
        //{
        //    transform.localScale *= 2;
        //}
        if (Physics.Raycast(ray, out hit, 50000.0f))
        {
            transform.position = new Vector3(hit.point.x, 0, hit.point.z);

        }
    }
}
