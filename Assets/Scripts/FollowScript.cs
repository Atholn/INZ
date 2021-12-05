using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    private RaycastHit hit;
    private float height;
    private int limit;
    private MapEditorManager levelEditorManager;

    private void Start()
    {
        levelEditorManager = FindObjectOfType<MapEditorManager>();
        height = levelEditorManager.ItemControllers[levelEditorManager.CurrentButtonPressed].item.ItemHeightPosY;

        ItemStartPoint startPointUnit = levelEditorManager.ItemControllers[levelEditorManager.CurrentButtonPressed].item.ItemPrefab.GetComponent<ItemStartPoint>();
        limit = startPointUnit != null ? startPointUnit.BuildSize / 2 : 0;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 500.0f) &&
            hit.point.x > limit && hit.point.x <= levelEditorManager.GetSizeMap()[0] - limit &&
            hit.point.z > limit && hit.point.z <= levelEditorManager.GetSizeMap()[1] - limit)
        {

            transform.position = new Vector3(hit.point.x - hit.point.x % 1, height+0.2f, hit.point.z - hit.point.z % 1);
        }
    }
}
