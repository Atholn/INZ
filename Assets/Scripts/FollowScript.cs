using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    private RaycastHit hit;
    private float shift = 0.5f;
    private float height;
    private int limit;

    LevelEditorManager levelEditorManager;

    private void Start()
    {
        levelEditorManager = FindObjectOfType<LevelEditorManager>();
        height = levelEditorManager.ItemButtons[levelEditorManager.CurrentButtonPressed].ItemHeightPosY;

        StartPointUnit startPointUnit =  levelEditorManager.ItemButtons[levelEditorManager.CurrentButtonPressed].ItemPrefab.GetComponent<StartPointUnit>();
        limit = startPointUnit != null ? startPointUnit.buildSize / 2 : 0;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 500.0f) &&
            hit.point.x > limit && hit.point.x < levelEditorManager.sizeMap - (limit + 1) &&
            hit.point.z > limit + shift && hit.point.z < levelEditorManager.sizeMap - limit - shift)
        {
            transform.position = new Vector3(hit.point.x, height, hit.point.z - shift);
        }
    }
}
