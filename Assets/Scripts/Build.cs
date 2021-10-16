using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    RaycastHit hit;
    Vector3 movePoint;
    public GameObject prefab;

    void Start()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, 50000.0f, (1 << 0)))
        {
            transform.position = hit.point;
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 0)))
        {
            transform.position = hit.point;
            //Debug.Log(transform.position);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 v = new Vector3(transform.position.x - transform.position.x % 1 + 0.5f, 0.5f, transform.position.z - transform.position.z % 1 + 0.5f);
            
            Instantiate(prefab, v, transform.rotation);
            Destroy(gameObject);
        }
    }
}
