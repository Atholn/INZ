using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    public bool toDelete = false;
    void Update()
    {
        if(toDelete)
        {
            Destroy(gameObject);
        }
    }
}
