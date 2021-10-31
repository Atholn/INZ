using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorManager : MonoBehaviour
{
    public ItemController[] ItemButtons;
    public int CurrentButtonPressed;
     
    private int[,] map;
    private int sizeMap;
    RaycastHit hit;
    internal Vector3 v;

    //public Text locationText;

    private void Start()
    {
        
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0) && ItemButtons[CurrentButtonPressed].Clicked)
        {
            float vx = v.x - v.x % 1;
            float vz = v.z - v.z % 1;


            if ((vx< sizeMap && vx>-1) && (vz < sizeMap && vz > -1) && map[(int)(vx - vx % 1), (int)(vz - vz % 1)] == 0)
            {
                if(map[(int)(v.x), (int)(v.z)] != 0)
                {
                    
                }

                Instantiate(ItemButtons[CurrentButtonPressed].ItemPrefab, 
                    new Vector3(vx - vx % 1, ItemButtons[CurrentButtonPressed].ItemHeightLevel, vz - vz % 1), 
                    ItemButtons[CurrentButtonPressed].ItemPrefab.transform.rotation);

                map[(int)(vx - vx % 1), (int)(vz - vz % 1)] = CurrentButtonPressed;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ButtonOff();
        }

        UpdateLocation(ray);
        CheckMap();
    }

    private void UpdateLocation(Ray ray)
    {
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            v = hit.point;
        }
    }

    private void CheckMap()
    {
        if (Input.GetMouseButtonDown(2))
        {
            //Debug.Log(map.Length);
            //string test = "";
            //for (int i = 0; i < map.Length; i++)
            //{
            //    if (i % sizeMap == 0)
            //    {
            //        Debug.Log(test);
            //        test = "";
            //    }
            //    test += i % sizeMap + " ";
            //}


            //for(int i=0; i<sizeMap; i++)
            //{
            //    for (int j = 0; j < sizeMap; j++)
            //    {
            //        map[i, j] = 0;
            //    }
            //}
            string test;
            for (int i = 0; i < sizeMap; i++)
            {
                test = "";
                for (int j = 0; j < sizeMap; j++)
                {
                    test += map[i, j];
                }
                Debug.Log(test);
            }
        }

    }

    private void ButtonOff()
    {
        for (int i = 0; i < ItemButtons.Length; i++)
        {
            ItemButtons[i].Clicked = false;
        }

        GameObject[] itemImages = GameObject.FindGameObjectsWithTag("ItemImage");
        for (int i = 0; i < itemImages.Length; i++)
        {
            Destroy(itemImages[i]);
        }

    }

    internal void CreateStartTerrain(int size, GameObject basicTerrain)
    {
        Ground ground = basicTerrain.gameObject.GetComponent<Ground>();
        basicTerrain.gameObject.transform.localScale = new Vector3(size * basicTerrain.gameObject.transform.localScale.x, basicTerrain.gameObject.transform.localScale.y, size * basicTerrain.gameObject.transform.localScale.z);

        Instantiate(basicTerrain, new Vector3(size / 2  -0.5f, 0, size / 2 - 0.5f), basicTerrain.transform.rotation);
        basicTerrain.gameObject.transform.localScale = ground.orginalScale;

        //size = this.size;
        map = new int[size, size];
        sizeMap = size;
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
