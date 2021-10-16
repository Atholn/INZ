using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorManager : MonoBehaviour
{
    public ItemController[] ItemButtons;
    public GameObject[] ItemPrefabs;
    public GameObject[] ItemImage;
    public int[] ItemHeightLevel;
    public int CurrentButtonPressed;

    RaycastHit hit;
   
    private void Start()
    {

    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && ItemButtons[CurrentButtonPressed].Clicked)
        {
            Vector3 v = hit.point;
            if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 0)))
            {
                 v  = hit.point;
            }
            ItemButtons[CurrentButtonPressed].Clicked = false;
            Instantiate(ItemPrefabs[CurrentButtonPressed], new Vector3(v.x - v.x % 1 + 0.5f, 0.5f, v.z - v.z % 1 + 0.5f), Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("ItemImage"));
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ItemButtons[CurrentButtonPressed].Clicked = false;
            Destroy(GameObject.FindGameObjectWithTag("ItemImage"));
        }

    }
}
