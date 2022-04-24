using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowDestroyerScript : MonoBehaviour
{
    private MapEditorManager mapEditorManager;

    private void Start()
    {
        mapEditorManager = FindObjectOfType<MapEditorManager>();
    }

    private void Update()
    {
        int vx = (int)(mapEditorManager.v.x - mapEditorManager.v.x % 1);
        int vz = (int)(mapEditorManager.v.z - mapEditorManager.v.z % 1);

        transform.position = new Vector3(vx, mapEditorManager.TypeTerrainToDestroy.value, vz);
    }
}
