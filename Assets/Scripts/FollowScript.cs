using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    public Material MaterialAllowsBuild;
    public Material MaterialNotAllowsBuild;
    public Material MaterialRebuild;

    private RaycastHit hit;
    private float height;
    private int limit;
    private MapEditorManager _mapEditorManager;

    private void Start()
    {
        _mapEditorManager = FindObjectOfType<MapEditorManager>();
        height = _mapEditorManager.ItemControllers[_mapEditorManager.CurrentButtonPressed].item.ItemHeightPosY;

        ItemStartPoint startPointUnit = _mapEditorManager.ItemControllers[_mapEditorManager.CurrentButtonPressed].item.ItemPrefab.GetComponent<ItemStartPoint>();
        limit = startPointUnit != null ? startPointUnit.BuildSize / 2 : 0;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        CheckIfCan((int)(_mapEditorManager.v.x - _mapEditorManager.v.x % 1), (int)(_mapEditorManager.v.z - _mapEditorManager.v.z % 1));

        //if (Physics.Raycast(ray, out hit, 500.0f) &&
        //    hit.point.x > limit && hit.point.x <= _mapEditorManager.GetSizeMap()[0] - limit &&
        //    hit.point.z > limit && hit.point.z <= _mapEditorManager.GetSizeMap()[1] - limit)
        //{
        //transform.position = new Vector3(hit.point.x - hit.point.x % 1, height + 0.2f, hit.point.z - hit.point.z % 1);
        //}
        int vx = (int)(_mapEditorManager.v.x - _mapEditorManager.v.x % 1);
        int vz = (int)(_mapEditorManager.v.z - _mapEditorManager.v.z % 1);
        if (vx > limit && vx <= _mapEditorManager.GetSizeMap()[0] - limit &&
            vz > limit && vz <= _mapEditorManager.GetSizeMap()[1] - limit)
        {
            transform.position = new Vector3(vx, height + 0.2f, vz);
        }
    }

    private void CheckIfCan(int x, int z)
    {
        if (_mapEditorManager.ItemControllers[_mapEditorManager.CurrentButtonPressed].item.ItemHeightLevel == 1)
        {
            if (_mapEditorManager.mapsPrefabs[1][x][z] == null && _mapEditorManager.CanCreate(x, z) )
            {
                gameObject.GetComponent<MeshRenderer>().material = MaterialAllowsBuild;
                return;
            }

            if (_mapEditorManager.mapsPrefabs[1][x][z] != null && _mapEditorManager.replaceToggle.isOn)
            {
                gameObject.GetComponent<MeshRenderer>().material = MaterialRebuild;
                return;
            }

            gameObject.GetComponent<MeshRenderer>().material = MaterialNotAllowsBuild;
        }
    }
}
