using UnityEngine;

public class FollowScript : MonoBehaviour
{
    public Material MaterialAllowsBuild;
    public Material MaterialNotAllowsBuild;
    public Material MaterialRebuild;

    private float height;
    private int limit;
    private MapEditorManager mapEditorManager;

    private void Start()
    {
        mapEditorManager = FindObjectOfType<MapEditorManager>();
        height = mapEditorManager.ItemControllers[mapEditorManager.CurrentButtonPressed].item.ItemHeightPosY;

        ItemStartPoint startPointUnit =
            mapEditorManager.ItemControllers[mapEditorManager.CurrentButtonPressed].item.ItemPrefab.GetComponent<ItemStartPoint>();

        limit = startPointUnit != null ? startPointUnit.BuildSize / 2 : 0;
    }

    private void Update()
    {
        CheckIfCan((int)(mapEditorManager.v.x - mapEditorManager.v.x % 1), (int)(mapEditorManager.v.z - mapEditorManager.v.z % 1));

        int vx = (int)(mapEditorManager.v.x - (mapEditorManager.v.x % 1));
        int vz = (int)(mapEditorManager.v.z - (mapEditorManager.v.z % 1));

        if (vx >= limit && vx <= mapEditorManager.GetSizeMap()[0] - limit - 1 &&
            vz >= limit && vz <= mapEditorManager.GetSizeMap()[1] - limit - 1)
        {
            transform.position = new Vector3(vx, height + 0.2f, vz);
        }
    }

    private void CheckIfCan(int x, int z)
    {
        if (x < limit || x > mapEditorManager.GetSizeMap()[0] - limit -1 || z < limit || z > mapEditorManager.GetSizeMap()[1] - limit-1)
        {
            gameObject.GetComponent<MeshRenderer>().material = MaterialNotAllowsBuild; 
            return;
        }

        switch (mapEditorManager.CanCreate(x, z))
        {
            case -1: gameObject.GetComponent<MeshRenderer>().material = MaterialRebuild; break;
            case 0: gameObject.GetComponent<MeshRenderer>().material = MaterialNotAllowsBuild; break;
            case 1: gameObject.GetComponent<MeshRenderer>().material = MaterialAllowsBuild; break;
        }
    }
}
