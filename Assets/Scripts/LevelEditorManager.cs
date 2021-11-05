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
    private GameObject[,] mapGameObjects;
    private int sizeMap;

    RaycastHit hit;
    internal Vector3 v;

    public Slider sizeSlider;
    public Text valueSizeSliderText;
    public Toggle singleMultiToggle;
    public Text valueSingleMultiToggleText;
    public Toggle replaceToggle;
    public Text valueReplaceToggleText;

    public Vector3 orginalScale;

    private void Start()
    {
        orginalScale = ItemButtons[0].ItemImage.transform.localScale;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        CreateTerrain();
        ButtonOff();

        UpdateLocation(ray);
        CheckMap();
        UpdateSettingsPanel();
    }


    private void UpdateSettingsPanel()
    {
        valueSizeSliderText.text = sizeSlider.value.ToString() + "x" + sizeSlider.value.ToString();


        if (singleMultiToggle.isOn)
        {
            valueSingleMultiToggleText.text = "Single";
        }

        if (!singleMultiToggle.isOn)
        {
            valueSingleMultiToggleText.text = "Multi";
        }

        if (ItemButtons[CurrentButtonPressed].ItemHeightLevel == 0)
        {
            ItemButtons[CurrentButtonPressed].ItemImage.transform.localScale = new Vector3(orginalScale.x * sizeSlider.value, orginalScale.y * sizeSlider.value, orginalScale.z);
        }

        if (replaceToggle.isOn)
        {
            valueReplaceToggleText.text = "Replace";
        }

        if (!replaceToggle.isOn)
        {
            valueReplaceToggleText.text = "No replace";
        }

    }

    private void CreateTerrain()
    {
        if (singleMultiToggle.isOn && Input.GetMouseButtonDown(0))
        {
            CreateTerrainCube();
            return;
        }

        if (!singleMultiToggle.isOn && Input.GetMouseButton(0))
        {
            CreateTerrainCube();
        }
    }

    private void CreateTerrainCube()
    {

        if (ItemButtons[CurrentButtonPressed].Clicked)
        {
            int vx = (int)(v.x - v.x % 1);
            int vz = (int)(v.z - v.z % 1);

            if ((vx < sizeMap && vx > -1) && (vz < sizeMap && vz > -1))
            {
                for (int i = 0; i < sizeSlider.value; i++)
                {
                    for (int j = 0; j < sizeSlider.value; j++)
                    {
                        int vxSlider = vx - (int)sizeSlider.value / 2 + i;
                        int vySlider = vz - (int)sizeSlider.value / 2 + j;

                        if (vxSlider < 0 || vxSlider >= sizeMap || vySlider < 0 || vySlider >= sizeMap)
                        {
                            continue;
                        }

                        if (replaceToggle.isOn && map[vxSlider, vySlider] != 0 && map[vxSlider, vySlider] != CurrentButtonPressed)
                        {
                            DeleteCube(vxSlider, vySlider);
                        }

                        if (map[vxSlider, vySlider] == 0)
                        {
                            Instantiate(ItemButtons[CurrentButtonPressed].ItemPrefab,
                                    new Vector3(vxSlider, ItemButtons[CurrentButtonPressed].ItemHeightPosY, vySlider),
                                    ItemButtons[CurrentButtonPressed].ItemPrefab.transform.rotation);

                            map[vxSlider, vySlider] = CurrentButtonPressed;
                            mapGameObjects[vxSlider, vySlider] = ItemButtons[CurrentButtonPressed].ItemPrefab;
                        }
                    }
                }
            }
        }




        //if (map[vx, vz] == 0)
        //{
        //    Instantiate(ItemButtons[CurrentButtonPressed].ItemPrefab,
        //            new Vector3(vx, ItemButtons[CurrentButtonPressed].ItemHeightLevel, vz),
        //            ItemButtons[CurrentButtonPressed].ItemPrefab.transform.rotation);

        //    map[vx, vz] = CurrentButtonPressed;
        //    mapGameObjects[vx, vz] = ItemButtons[CurrentButtonPressed].ItemPrefab;
        //}
    }

    private void DeleteCube(int vxSlider, int vySlider)
    {
        //todo
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
        if (Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < ItemButtons.Length; i++)
            {
                ItemButtons[i].Clicked = false;
                ItemButtons[i].ItemImage.transform.localScale = orginalScale;
            }

            GameObject[] itemImages = GameObject.FindGameObjectsWithTag("ItemImage");
            for (int i = 0; i < itemImages.Length; i++)
            {
                Destroy(itemImages[i]);
            }
        }
    }

    internal void CreateStartTerrain(int size, GameObject basicTerrain)
    {
        Ground ground = basicTerrain.gameObject.GetComponent<Ground>();
        basicTerrain.gameObject.transform.localScale = new Vector3(size * basicTerrain.gameObject.transform.localScale.x, basicTerrain.gameObject.transform.localScale.y, size * basicTerrain.gameObject.transform.localScale.z);

        Instantiate(basicTerrain, new Vector3(size / 2 - 0.5f, 0, size / 2 - 0.5f), basicTerrain.transform.rotation);
        basicTerrain.gameObject.transform.localScale = ground.orginalScale;

        //size = this.size;
        map = new int[size, size];
        mapGameObjects = new GameObject[size, size];
        sizeMap = size;
        //todo

        //for (int i = 0; i < size; i++)
        //{
        //    for (int j = 0; j < size; j++)
        //    {
        //        Debug.Log(mapGameObjects[i, j]);
        //    }
        //}

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
