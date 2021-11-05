using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    public Material material;
    public Material material1;
    public Material material2;
    
    void Start()
    {
        Material material = GetComponent<Material>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            material = material1;
           
        }
        if (Input.GetMouseButtonDown(1))
        {
            material = material2;
            return;
        }
    }
}
