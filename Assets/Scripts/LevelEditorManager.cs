using System;
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
                v = hit.point;
            }
            Instantiate(ItemButtons[CurrentButtonPressed].ItemPrefab, new Vector3(v.x - v.x % 1 + 0.5f, ItemButtons[CurrentButtonPressed].ItemHeightLevel, v.z - v.z % 1 + 0.5f), ItemButtons[CurrentButtonPressed].ItemPrefab.transform.rotation);
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

    internal void CreateStartTerrain(int size, GameObject basicTerrain)
    {
        Ground ground = basicTerrain.gameObject.GetComponent<Ground>();
        basicTerrain.gameObject.transform.localScale = new Vector3(size * basicTerrain.gameObject.transform.localScale.x, basicTerrain.gameObject.transform.localScale.y, size * basicTerrain.gameObject.transform.localScale.z);

        Instantiate(basicTerrain, new Vector3(size / 2, 0, size / 2), basicTerrain.transform.rotation);
        basicTerrain.gameObject.transform.localScale = ground.orginalScale;
        //todo


        //bT.gameObject.transform *= new Vector3(1, 0, 1);
        //for (int i=0; i<size;i++)
        //{
        //    for (int j = 0; j < size; j++)
        //    {

        //        Instantiate(basicTerrain, new Vector3(i + 0.5f, 0, j + 0.5f), basicTerrain.transform.rotation);
        //    }
        //}
    }
}
