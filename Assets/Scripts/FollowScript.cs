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
        height = levelEditorManager.ItemButtons[levelEditorManager.CurrentButtonPressed].item.ItemHeightPosY;

        StartPointUnit startPointUnit = levelEditorManager.ItemButtons[levelEditorManager.CurrentButtonPressed].item.ItemPrefab.GetComponent<StartPointUnit>();
        limit = startPointUnit != null ? startPointUnit.buildSize / 2 : 0;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 500.0f) &&
            hit.point.x > limit && hit.point.x <= levelEditorManager.sizeMapX - limit &&
            hit.point.z > limit && hit.point.z <= levelEditorManager.sizeMapY - limit)
        {

            transform.position = new Vector3(hit.point.x - hit.point.x % 1, height, hit.point.z - hit.point.z % 1);
        }
    }
}
