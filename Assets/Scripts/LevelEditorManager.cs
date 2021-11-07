using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorManager : MonoBehaviour
{
    public ItemController[] ItemButtons;
    public int CurrentButtonPressed;

    internal int sizeMap;
    private int mapCount = 2; //Level 0 - terrain; Level 1 - Nature/Unit
    private int[][,] maps;
    private GameObject[][,] mapsPrefabs;

    RaycastHit hit;
    internal Vector3 v;

    public Slider sizeSlider;
    public Text valueSizeSliderText;
    public Toggle singleMultiToggle;
    public Text valueSingleMultiToggleText;
    public Toggle replaceToggle;
    public Text valueReplaceToggleText;

    public Vector3 orginalScale;

    private List<StartPointUnit> startPointUnits = new List<StartPointUnit>();
    UnitEditorPanel unitEditorPanel;

    private void Start()
    {
        orginalScale = ItemButtons[0].ItemImage.transform.localScale;

        InitializeStartMaps();

        CreateStartPointUnits();
    }

    private void InitializeStartMaps()
    {
        maps = new int[mapCount][,];
        mapsPrefabs = new GameObject[mapCount][,];
    }

    private void CreateStartPointUnits()
    {
        unitEditorPanel = FindObjectOfType<UnitEditorPanel>();
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
            return;
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
                if (ItemButtons[CurrentButtonPressed].ItemHeightLevel == 0)
                {
                    GenerateTerrain(vx, vz, ItemButtons[CurrentButtonPressed].ItemHeightLevel);
                }

                if (ItemButtons[CurrentButtonPressed].ItemHeightLevel == 1)
                {
                    GenerateNatureUnit(vx, vz, ItemButtons[CurrentButtonPressed].ItemHeightLevel);
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

    private void GenerateNatureUnit(int vx, int vz, int level)
    {       
        if (ItemButtons[CurrentButtonPressed] is ItemUnitController)
        {
            int size;
            int fullSize = ItemButtons[CurrentButtonPressed].ItemPrefab.GetComponent<StartPointUnit>().buildSize;
            size =fullSize / 2;

            if (vx < size || vx >= sizeMap - size || vz < size || vz >= sizeMap - size) return;

            int tempvx;
            int tempvz;

            for (int i=0; i< fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = vx - fullSize/2 + i;
                    tempvz = vz - fullSize/2 + j;                   

                    if (vx < 0 || vx >= sizeMap || vz < 0 || vz >= sizeMap) return;
                    if (maps[level][tempvx, tempvz] != 0) return;
                }
            }

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = vx - fullSize / 2 + i;
                    tempvz = vz - fullSize / 2 + j;
                    if (tempvx == vx && tempvz == vz) continue;
                    maps[level][tempvx, tempvz] = -1;
                }
            }                   
        }

        CreateGameObject(vx, vz, level);
    }

    private void GenerateTerrain(int vx, int vz, int level)
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

                CreateGameObject(vxSlider, vySlider, level);
            }
        }
    }

    private void CreateGameObject(int vx, int vz, int level)
    {
        if (replaceToggle.isOn && maps[level][vx, vz] != 0 && maps[level][vx, vz] != CurrentButtonPressed)
        {
            DeleteGameObject(vx, vz);
        }

        if (maps[level][vx, vz] == 0)
        {
            Instantiate(ItemButtons[CurrentButtonPressed].ItemPrefab,
                    new Vector3(vx, ItemButtons[CurrentButtonPressed].ItemHeightPosY, vz),
                    ItemButtons[CurrentButtonPressed].ItemPrefab.transform.rotation);

            maps[level][vx, vz] = CurrentButtonPressed;
            mapsPrefabs[level][vx, vz] = ItemButtons[CurrentButtonPressed].ItemPrefab;
        }
    }

    //private void CreateGameObject(int vx, int vy)
    //{
    //    if (replaceToggle.isOn && mapTerrain[vx, vy] != 0 && mapTerrain[vx, vy] != CurrentButtonPressed)
    //    {
    //        DeleteGameObject(vx, vy);
    //    }

    //    if (mapTerrain[vx, vy] == 0)
    //    {
    //        Instantiate(ItemButtons[CurrentButtonPressed].ItemPrefab,
    //                new Vector3(vx, ItemButtons[CurrentButtonPressed].ItemHeightPosY, vy),
    //                ItemButtons[CurrentButtonPressed].ItemPrefab.transform.rotation);

    //        mapTerrain[vx, vy] = CurrentButtonPressed;
    //        mapTerrainPrefabs[vx, vy] = ItemButtons[CurrentButtonPressed].ItemPrefab;
    //    }
    //}

    private void DeleteGameObject(int vxSlider, int vySlider)
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
            for (int k = 0; k < mapCount; k++)
            {
                string test;
                for (int i = 0; i < sizeMap; i++)
                {
                    test = "";
                    for (int j = 0; j < sizeMap; j++)
                    {
                        test += maps[k][i, j];
                    }
                    Debug.Log(test);
                }

                Debug.LogError("-------------");
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
                if (ItemButtons[i].ItemHeightLevel == 0)
                {
                    ItemButtons[i].ItemImage.transform.localScale = orginalScale;
                }
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

        sizeMap = size;
        for (int i = 0; i < mapCount; i++)
        {
            maps[i] = new int[size, size];
            mapsPrefabs[i] = new GameObject[size, size];
        }
    }
}
