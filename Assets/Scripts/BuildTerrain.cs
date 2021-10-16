using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTerrain : MonoBehaviour
{
    public GameObject terrain;
    public void spawn()
    {
        Instantiate(terrain);
    }
}
