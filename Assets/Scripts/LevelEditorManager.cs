using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorManager : MonoBehaviour
{
    public ItemController[] ItemButtons;
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
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                 v  = hit.point;
            }          
            Instantiate(ItemButtons[CurrentButtonPressed].ItemPrefab, new Vector3(v.x - v.x % 1 + 0.5f, ItemButtons[CurrentButtonPressed].ItemHeightLevel, v.z - v.z % 1 + 0.5f), ItemButtons[CurrentButtonPressed].ItemPrefab.transform.rotation);
            ButtonOff();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ButtonOff();
        }

    }

    private void ButtonOff()
    {
        ItemButtons[CurrentButtonPressed].Clicked = false;
        Destroy(GameObject.FindGameObjectWithTag("ItemImage"));
    }
}
