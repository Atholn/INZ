using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowDestroyerScript : MonoBehaviour
{
    private MapEditorManager _mapEditorManager;

    private void Start()
    {
        _mapEditorManager = FindObjectOfType<MapEditorManager>();
    }

    private void Update()
    {
        int vx = (int)(_mapEditorManager.v.x - _mapEditorManager.v.x % 1);
        int vz = (int)(_mapEditorManager.v.z - _mapEditorManager.v.z % 1);

        transform.position = new Vector3(vx, _mapEditorManager.TypeTerrainToDestroy.value, vz);
    }
}
