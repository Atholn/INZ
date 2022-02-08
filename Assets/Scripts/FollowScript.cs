using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    public Material MaterialAllowsBuild;
    public Material MaterialNotAllowsBuild;
    public Material MaterialRebuild;

    private float height;
    private int limit;
    private MapEditorManager _mapEditorManager;
    private GameManager _gameManager;

    private void Start()
    {
        _mapEditorManager = FindObjectOfType<MapEditorManager>();
        _gameManager = FindObjectOfType<GameManager>();
        height = _mapEditorManager != null ? 
            _mapEditorManager.ItemControllers[_mapEditorManager!=null? _mapEditorManager.CurrentButtonPressed : _gameManager.CurrentButtonPressed].item.ItemHeightPosY:
            _gameManager.ItemControllers[_mapEditorManager!=null? _mapEditorManager.CurrentButtonPressed : _gameManager.CurrentButtonPressed].item.ItemHeightPosY;

        ItemStartPoint startPointUnit = _mapEditorManager != null ?
            _mapEditorManager.ItemControllers[_mapEditorManager != null ? _mapEditorManager.CurrentButtonPressed : _gameManager.CurrentButtonPressed].item.ItemPrefab.GetComponent<ItemStartPoint>():
            _gameManager.ItemControllers[_mapEditorManager != null ? _mapEditorManager.CurrentButtonPressed : _gameManager.CurrentButtonPressed].item.ItemPrefab.GetComponent<ItemStartPoint>();

        limit = startPointUnit != null ? startPointUnit.BuildSize / 2 : 0;
    }

    private void Update()
    {
        if(_mapEditorManager != null)
        CheckIfCan((int)(_mapEditorManager.v.x - _mapEditorManager.v.x % 1), (int)(_mapEditorManager.v.z - _mapEditorManager.v.z % 1));

        int vx = (int)((_mapEditorManager != null ? _mapEditorManager.v.x : _gameManager.v.x) - ((_mapEditorManager != null ? _mapEditorManager.v.x : _gameManager.v.x) % 1));
        int vz = (int)((_mapEditorManager != null ? _mapEditorManager.v.z : _gameManager.v.z) - ((_mapEditorManager != null ? _mapEditorManager.v.z : _gameManager.v.z) % 1));

        if(_mapEditorManager == null)
        {
            transform.position = new Vector3(vx, height + 0.2f, vz);
            return;
        }

        if (vx >= limit && vx <= _mapEditorManager.GetSizeMap()[0] - limit - 1 &&
            vz >= limit && vz <= _mapEditorManager.GetSizeMap()[1] - limit -1)
        {
            transform.position = new Vector3(vx, height + 0.2f, vz);
        }
    }

    private void CheckIfCan(int x, int z)
    {
        if (x < limit || x > _mapEditorManager.GetSizeMap()[0] - limit -1 || z < limit || z > _mapEditorManager.GetSizeMap()[1] - limit-1)
        {
            gameObject.GetComponent<MeshRenderer>().material = MaterialNotAllowsBuild; 
            return;
        }

        switch (_mapEditorManager.CanCreate(x, z))
        {
            case -1: gameObject.GetComponent<MeshRenderer>().material = MaterialRebuild; break;
            case 0: gameObject.GetComponent<MeshRenderer>().material = MaterialNotAllowsBuild; break;
            case 1: gameObject.GetComponent<MeshRenderer>().material = MaterialAllowsBuild; break;
        }
    }
}
