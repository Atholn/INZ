using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private GameObject Terrain;
    public GameObject basicTerrain;

    RaycastHit hit;
    internal Vector3 v;

    public Slider sizeSlider;
    public Text valueSizeSliderText;
    public Toggle singleMultiToggle;
    public Text valueSingleMultiToggleText;
    public Toggle replaceToggle;
    public Text valueReplaceToggleText;

    public Vector3 orginalScale;

    private List<StartPoint> startPoints = new List<StartPoint>();
    public UnitEditorPanel unitEditorPanel;

    public GameObject[] panelsToActive;

    private void Start()
    {
        orginalScale = ItemButtons[0].item.ItemImage.transform.localScale;

        InitializeStartMaps();
        InitializeStartPointUnitsList();
    }

    private void InitializeStartPointUnitsList()
    {
        foreach (UnitEditorButton unit in unitEditorPanel.UnitButtons)
        {
            startPoints.Add(new StartPoint
            {
                unitMaterialName = unit.unitMaterial.name,
                unitStartLocation = Vector3.zero,
            });
        }
    }

    private void InitializeStartMaps()
    {
        maps = new int[mapCount][,];
        mapsPrefabs = new GameObject[mapCount][,];
    }

    private void Update()
    {
        UpdateCreateTerrain();
        UpdateButtonOff();
        UpdateLocation();
        UpdateSettingsPanel();

        //
        CheckMap();
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

        if (ItemButtons[CurrentButtonPressed].item.ItemHeightLevel == 0)
        {
            ItemButtons[CurrentButtonPressed].item.ItemImage.transform.localScale = new Vector3(orginalScale.x * sizeSlider.value, orginalScale.y * sizeSlider.value, orginalScale.z);
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

    private void UpdateCreateTerrain()
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
                if (ItemButtons[CurrentButtonPressed].item.ItemHeightLevel == 0)
                {
                    GenerateTerrain(vx, vz, ItemButtons[CurrentButtonPressed].item.ItemHeightLevel);
                }

                if (ItemButtons[CurrentButtonPressed].item.ItemHeightLevel == 1)
                {
                    GenerateNatureUnit(vx, vz, ItemButtons[CurrentButtonPressed].item.ItemHeightLevel);
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
            int fullSize = ItemButtons[CurrentButtonPressed].item.ItemPrefab.GetComponent<StartPointUnit>().buildSize;

            if (vx < fullSize / 2 || vx >= sizeMap - fullSize / 2 || vz < fullSize / 2 || vz >= sizeMap - fullSize / 2) return;

            int tempvx;
            int tempvz;

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = vx - fullSize / 2 + i;
                    tempvz = vz - fullSize / 2 + j;

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
        if (replaceToggle.isOn && maps[level][vx, vz] > 0 && maps[level][vx, vz] != CurrentButtonPressed)
        {
            if (mapsPrefabs[level][vx, vz].GetComponent<StartPointUnit>() != null)
            {
                StartPoint spu = startPoints.Where(u => u.unitStartLocation.x == vx && u.unitStartLocation.z == vz).First();

                int areaToReset = mapsPrefabs[level][vx, vz].GetComponent<StartPointUnit>().buildSize;
                int tempvx, tempvz;

                for (int i = 0; i < areaToReset; i++)
                {
                    for (int j = 0; j < areaToReset; j++)
                    {
                        tempvx = vx - areaToReset / 2 + i;
                        tempvz = vz - areaToReset / 2 + j;

                        maps[level][tempvx, tempvz] = 0;
                    }
                }

                spu.unitStartLocation = Vector3.zero;
            }

            DeleteGameObject(vx, vz, level);

        }

        if (maps[level][vx, vz] == 0)
        {
            if (ItemButtons[CurrentButtonPressed] is ItemUnitController)
            {
                UpdateStartUnitList(vx, vz, level);
            }

            maps[level][vx, vz] = CurrentButtonPressed;
            mapsPrefabs[level][vx, vz] = Instantiate(ItemButtons[CurrentButtonPressed].item.ItemPrefab,
                new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz),
                ItemButtons[CurrentButtonPressed].item.ItemPrefab.transform.rotation);
            //mapsPrefabs[level][vx, vz] = ItemButtons[CurrentButtonPressed].ItemPrefab;
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



    private void UpdateStartUnitList(int vx, int vz, int level)
    {
        StartPoint spu = startPoints.Where(u => u.unitMaterialName == unitEditorPanel.actualMaterial.name).First();

        if (spu.unitStartLocation == Vector3.zero)
        {
            spu.unitStartLocation = new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz);
            return;
        }

        if (maps[level][(int)spu.unitStartLocation.x, (int)spu.unitStartLocation.z] == CurrentButtonPressed)
        {
            int tempvx, tempvz;
            int fullSize = ItemButtons[CurrentButtonPressed].item.ItemPrefab.GetComponent<StartPointUnit>().buildSize;

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = (int)spu.unitStartLocation.x - fullSize / 2 + i;
                    tempvz = (int)spu.unitStartLocation.z - fullSize / 2 + j;

                    maps[level][tempvx, tempvz] = 0;
                }
            }
        }

        GameObjectToDelete(mapsPrefabs[level][(int)spu.unitStartLocation.x, (int)spu.unitStartLocation.z]);
        mapsPrefabs[level][(int)spu.unitStartLocation.x, (int)spu.unitStartLocation.z] = null;
        spu.unitStartLocation = new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz);
    }

    private void DeleteGameObject(int vx, int vz, int level)
    {
        GameObjectToDelete(mapsPrefabs[level][vx, vz]);

        maps[level][vx, vz] = 0;
        mapsPrefabs[level][vx, vz] = null;
    }

    private void GameObjectToDelete(GameObject gameObject)
    {
        Destroy(gameObject.transform.gameObject);
    }

    private void UpdateLocation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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

        if (Input.GetMouseButtonDown(3))
        {
            foreach (StartPoint sP in startPoints)
            {
                if (sP.unitStartLocation != Vector3.zero)
                {
                    Debug.Log(sP.unitMaterialName);
                }
            }
        }
    }

    private void UpdateButtonOff()
    {
        if (Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < ItemButtons.Length; i++)
            {
                ItemButtons[i].Clicked = false;
                if (ItemButtons[i].item.ItemHeightLevel == 0)
                {
                    ItemButtons[i].item.ItemImage.transform.localScale = orginalScale;
                }
            }

            GameObject[] itemImages = GameObject.FindGameObjectsWithTag("ItemImage");

            for (int i = 0; i < itemImages.Length; i++)
            {
                Destroy(itemImages[i]);
            }
        }
    }

    internal void CreateStartTerrain(int size)
    {
        if (Terrain != null)
        {
            GameObjectToDelete(Terrain);
        }
        else
        {
            foreach (GameObject panel in panelsToActive)
            {
                panel.SetActive(true);
            }
        }

        Ground ground = basicTerrain.gameObject.GetComponent<Ground>();
        basicTerrain.gameObject.transform.localScale = new Vector3(size * basicTerrain.gameObject.transform.localScale.x, basicTerrain.gameObject.transform.localScale.y, size * basicTerrain.gameObject.transform.localScale.z);
        Terrain = Instantiate(basicTerrain, new Vector3(size / 2 - 0.5f, 0, size / 2 - 0.5f), basicTerrain.transform.rotation);
        basicTerrain.gameObject.transform.localScale = ground.orginalScale;
        InitializeTerrainArrays(size);
    }

    private void InitializeTerrainArrays(int size)
    {
        sizeMap = size;

        for (int i = 0; i < mapCount; i++)
        {
            maps[i] = new int[size, size];
            mapsPrefabs[i] = new GameObject[size, size];
        }
    }

    public Map ExportInfo()
    {
        List<string> unitMaterials = new List<string>();
        List<float[]> unitStartLocations = new List<float[]>();
        foreach (StartPoint sp in startPoints)
        {
            unitStartLocations.Add(sp.uSL);
            unitMaterials.Add(sp.unitMaterialName);
        }


        return new Map()
        {
            SizeMap = this.sizeMap,
            Maps = this.maps,
            UnitMaterials = unitMaterials,
            UnitStartLocations = unitStartLocations,
        };
    }

    public void ImportInfo(Map map)
    {     
        DeleteArrayGameObjects(mapsPrefabs[0], 0);
        DeleteArrayGameObjects(mapsPrefabs[1], 1);

        InitializeTerrainArrays(map.SizeMap);
        CreateStartTerrain(sizeMap);
        InitializeNewMap(map);

    }

    private void InitializeNewMap(Map map)
    {
        maps = map.Maps;
        for (int i = 0; i < mapCount; i++)
        {
            for (int j = 0; j < sizeMap; j++)
            {
                for (int k = 0; k < sizeMap; k++)
                {
                    if (maps[i][j, k] > 0)
                    {
                        mapsPrefabs[i][j, k] = Instantiate(ItemButtons[maps[i][j, k]].item.ItemPrefab, new Vector3(j, ItemButtons[maps[i][j, k]].item.ItemHeightPosY, k), ItemButtons[maps[i][j, k]].item.ItemPrefab.transform.rotation);
                    }
                }
            }
        }
    }

    public void DeleteArrayGameObjects(GameObject[,] ArrayToDelete, int level)
    {
        for (int i = 0; i < sizeMap; i++)
        {
            for (int j = 0; j < sizeMap; j++)
            {
                if (ArrayToDelete[i, j] != null)
                {
                    DeleteGameObject(i, j, level);
                }
            }
        }
    }

}
