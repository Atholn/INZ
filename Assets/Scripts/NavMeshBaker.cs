using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    [SerializeField]
    internal List<NavMeshSurface> navMeshSurfaces;

    private void Awake()
    {
    }

    internal void Bake()
    {
        for(int i= 0; i< navMeshSurfaces.Count; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
    }
}
