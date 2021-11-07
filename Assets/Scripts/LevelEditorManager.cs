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
    //private int[,] mapTerrain; // height level 0 
    //private GameObject[,] mapTerrainPrefabs;
    //private int[,] mapNatureUnit; // height level 1
    //private GameObject[,] mapNatureUnitPrefabs;

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
        int size;
        if (ItemButtons[CurrentButtonPressed] is ItemUnitController)
        {
            size = ItemButtons[CurrentButtonPressed].ItemPrefab.GetComponent<StartPointUnit>().buildSize / 2;

            if (vx < size || vx >= sizeMap - size || vz < size || vz >= sizeMap) return;
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

    private void CreateGameObject(int vx, int vy, int level)
    {
        if (replaceToggle.isOn && maps[level][vx, vy] != 0 && maps[level][vx, vy] != CurrentButtonPressed)
        {
            DeleteGameObject(vx, vy);
        }

        if (maps[level][vx, vy] == 0)
        {
            Instantiate(ItemButtons[CurrentButtonPressed].ItemPrefab,
                    new Vector3(vx, ItemButtons[CurrentButtonPressed].ItemHeightPosY, vy),
                    ItemButtons[CurrentButtonPressed].ItemPrefab.transform.rotation);

            maps[level][vx, vy] = CurrentButtonPressed;
            mapsPrefabs[level][vx, vy] = ItemButtons[CurrentButtonPressed].ItemPrefab;
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


        //mapTerrain = new int[size, size];
        //mapTerrainPrefabs = new GameObject[size, size];
        //mapNatureUnit = new int[size, size];
        //mapNatureUnitPrefabs = new GameObject[size, size];

        sizeMap = size;
        for (int i = 0; i < mapCount; i++)
        {
            maps[i] = new int[size, size];
            mapsPrefabs[i] = new GameObject[size, size];
        }
        //maps[0] = mapTerrain;
        //maps[1] = mapNatureUnit;
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
