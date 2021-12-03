using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorManager : MonoBehaviour
{
    public ItemController[] ItemButtons;
    internal int CurrentButtonPressed;

    internal int sizeMapX;
    internal int sizeMapY;
    internal int mapCount = 2; //Level 0 - terrain; Level 1 - Nature/Unit
    internal int[][][] maps;
    internal GameObject[][][] mapsPrefabs;
    internal GameObject Terrain;
    internal int BasicTerrainID = 0;

    RaycastHit hit;
    internal Vector3 v;

    public Slider sizeSlider;
    public Text valueSizeSliderText;
    public Toggle singleMultiToggle;
    public Text valueSingleMultiToggleText;
    public Toggle replaceToggle;
    public Text valueReplaceToggleText;

    private List<EditorStartPoint> startPoints = new List<EditorStartPoint>();
    public UnitEditorPanel unitEditorPanel;

    public GameObject[] panelsToActive;
    public GameObject[] craftingPanels;
    public GameObject sizeMapPanel;

    private void Start()
    {
        InitializeBasicTerrain();
        InitializeOpenEditor();
        InitializeIDButtons();
        InitializeStartMaps();

        InitializeStartPointUnitsList();
    }

    private void InitializeBasicTerrain()
    {
        ItemButtons[BasicTerrainID].firstScale = ItemButtons[BasicTerrainID].item.ItemPrefab.transform.localScale;
    }

    private void InitializeOpenEditor()
    {
        foreach (GameObject panel in panelsToActive)
        {
            panel.SetActive(false);
        }
        foreach (GameObject panel in craftingPanels)
        {
            panel.SetActive(false);
        }
        sizeMapPanel.SetActive(true);
    }

    private void InitializeIDButtons()
    {
        for (int i = 0; i < ItemButtons.Length; i++)
        {
            ItemButtons[i].item.ID = i;
        }
    }

    private void InitializeStartMaps()
    {
        maps = new int[mapCount][][];
        mapsPrefabs = new GameObject[mapCount][][];
    }

    private void InitializeStartPointUnitsList()
    {
        foreach (Button button in unitEditorPanel.ColorsUnitsButtons)
        {
            UnitEditorButton unitEditorButton = button.gameObject.GetComponent<UnitEditorButton>();

            startPoints.Add(new EditorStartPoint
            {
                unitMaterialName = unitEditorButton.unitMaterial.name,
                unitStartLocation = Vector3.zero,
            });
        }
    }

    internal void InitializeStartTerrain(int sizeX, int sizeY)
    {
        foreach (GameObject panel in panelsToActive)
        {
            panel.SetActive(true);
        }
        sizeMapPanel.SetActive(false);

        GameObject basicTerrainPrefab = ItemButtons[BasicTerrainID].item.ItemPrefab;
        basicTerrainPrefab.gameObject.transform.localScale = new Vector3(sizeX * ItemButtons[BasicTerrainID].firstScale.x, ItemButtons[BasicTerrainID].firstScale.y, sizeY * ItemButtons[BasicTerrainID].firstScale.z);
        Terrain = Instantiate(basicTerrainPrefab, new Vector3(sizeX / 2 - 0.5f, 0, sizeY / 2 - 0.5f), basicTerrainPrefab.transform.rotation);
        basicTerrainPrefab.gameObject.transform.localScale = ItemButtons[BasicTerrainID].firstScale;

        InitializeTerrainArrays(sizeX, sizeY);
    }

    private void InitializeTerrainArrays(int sizeX, int sizeY)
    {
        sizeMapX = sizeX;
        sizeMapY = sizeY;

        for (int i = 0; i < mapCount; i++)
        {
            maps[i] = new int[sizeX][];
            mapsPrefabs[i] = new GameObject[sizeX][];

            for (int j = 0; j < sizeX; j++)
            {
                maps[i][j] = new int[sizeY];
                mapsPrefabs[i][j] = new GameObject[sizeY];
            }
        }
    }

    private void Update()
    {
        UpdateSettingsPanel();
        UpdateLocation();
        UpdateCreate();
        UpdateButtonOff();

        //--
        UpdateCheckMap();
    }

    private void UpdateLocation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            v = hit.point;
        }
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
            ItemButtons[CurrentButtonPressed].item.ItemImage.transform.localScale = new Vector3(ItemButtons[CurrentButtonPressed].firstScale.x * sizeSlider.value, ItemButtons[CurrentButtonPressed].firstScale.y * sizeSlider.value, ItemButtons[CurrentButtonPressed].firstScale.z);
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

    #region CreateEditor
    private void UpdateCreate()
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

            if ((vx < sizeMapX && vx > -1) && (vz < sizeMapY && vz > -1))
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
    }

    private void GenerateTerrain(int vx, int vz, int level)
    {
        for (int i = 0; i < sizeSlider.value; i++)
        {
            for (int j = 0; j < sizeSlider.value; j++)
            {
                int vxSlider = vx - (int)sizeSlider.value / 2 + i;
                int vySlider = vz - (int)sizeSlider.value / 2 + j;

                if (vxSlider < 0 || vxSlider >= sizeMapX || vySlider < 0 || vySlider >= sizeMapY)
                {
                    continue;
                }

                CreateGameObject(vxSlider, vySlider, level);
            }
        }
    }

    private void GenerateNatureUnit(int vx, int vz, int level)
    {
        if (ItemButtons[CurrentButtonPressed] is ItemUnitController)
        {
            int fullSize = ItemButtons[CurrentButtonPressed].item.ItemPrefab.GetComponent<StartPointUnit>().buildSize;

            if (vx < fullSize / 2 || vx >= sizeMapX - fullSize / 2 || vz < fullSize / 2 || vz >= sizeMapY - fullSize / 2) return;

            int tempvx;
            int tempvz;

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = vx - fullSize / 2 + i;
                    tempvz = vz - fullSize / 2 + j;

                    if (vx < 0 || vx >= sizeMapX || vz < 0 || vz >= sizeMapY) return;
                    if (maps[level][tempvx][tempvz] != 0) return;
                }
            }

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = vx - fullSize / 2 + i;
                    tempvz = vz - fullSize / 2 + j;
                    if (tempvx == vx && tempvz == vz) continue;
                    maps[level][tempvx][tempvz] = -1;
                }
            }
        }

        CreateGameObject(vx, vz, level);
    }

    private void CreateGameObject(int vx, int vz, int level)
    {
        if (replaceToggle.isOn && maps[level][vx][vz] > 0 && maps[level][vx][vz] != CurrentButtonPressed)
        {
            if (mapsPrefabs[level][vx][vz].GetComponent<StartPointUnit>() != null)
            {
                EditorStartPoint spu = startPoints.Where(u => u.unitStartLocation.x == vx && u.unitStartLocation.z == vz).First();

                int areaToReset = mapsPrefabs[level][vx][vz].GetComponent<StartPointUnit>().buildSize;
                int tempvx, tempvz;

                for (int i = 0; i < areaToReset; i++)
                {
                    for (int j = 0; j < areaToReset; j++)
                    {
                        tempvx = vx - areaToReset / 2 + i;
                        tempvz = vz - areaToReset / 2 + j;

                        maps[level][tempvx][tempvz] = 0;
                    }
                }

                spu.unitStartLocation = Vector3.zero;
            }

            DeleteGameObject(vx, vz, level);
        }

        if (maps[level][vx][vz] == 0)
        {
            if (ItemButtons[CurrentButtonPressed] is ItemUnitController)
            {
                UpdateStartUnitList(vx, vz, level);
            }

            maps[level][vx][vz] = CurrentButtonPressed;
            mapsPrefabs[level][vx][vz] = Instantiate(ItemButtons[CurrentButtonPressed].item.ItemPrefab,
                new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz),
                ItemButtons[CurrentButtonPressed].item.ItemPrefab.transform.rotation);
        }
    }

    private void UpdateStartUnitList(int vx, int vz, int level)
    {
        EditorStartPoint spu = startPoints.Where(u => u.unitMaterialName == unitEditorPanel.ActualMaterial.name).First();

        if (spu.unitStartLocation == Vector3.zero)
        {
            spu.unitStartLocation = new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz);
            return;
        }

        if (maps[level][(int)spu.unitStartLocation.x][(int)spu.unitStartLocation.z] == CurrentButtonPressed)
        {
            int tempvx, tempvz;
            int fullSize = ItemButtons[CurrentButtonPressed].item.ItemPrefab.GetComponent<StartPointUnit>().buildSize;

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = (int)spu.unitStartLocation.x - fullSize / 2 + i;
                    tempvz = (int)spu.unitStartLocation.z - fullSize / 2 + j;

                    maps[level][tempvx][tempvz] = 0;
                }
            }
        }

        DeleteGameObject((int)spu.unitStartLocation.x, (int)spu.unitStartLocation.z, level);
        spu.unitStartLocation = new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz);
    }

    private void DeleteGameObject(int vx, int vz, int level)
    {
        GameObjectToDelete(mapsPrefabs[level][vx][vz]);

        maps[level][vx][vz] = 0;
        mapsPrefabs[level][vx][vz] = null;
    }

    private void GameObjectToDelete(GameObject gameObject)
    {
        Destroy(gameObject.transform.gameObject);
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
                    ItemButtons[i].item.ItemImage.transform.localScale = ItemButtons[i].firstScale;
                }
            }

            GameObject[] itemImages = GameObject.FindGameObjectsWithTag("ItemImage");

            for (int i = 0; i < itemImages.Length; i++)
            {
                Destroy(itemImages[i]);
            }
        }
    }
    #endregion

    private void UpdateCheckMap()
    {
        if (Input.GetMouseButtonDown(2))
        {
            for (int k = 0; k < mapCount; k++)
            {
                string test;
                for (int i = 0; i < sizeMapX; i++)
                {
                    test = "";
                    for (int j = 0; j < sizeMapY; j++)
                    {
                        test += maps[k][i][j];
                    }
                    Debug.Log(test);
                }

                Debug.LogError("-------------");
            }
        }

        if (Input.GetMouseButtonDown(3))
        {
            foreach (EditorStartPoint sP in startPoints)
            {
                if (sP.unitStartLocation != Vector3.zero)
                {
                    Debug.Log(sP.unitMaterialName);
                }
            }
        }
    }

    #region Export Import
    public Map ExportMap()
    {
        List<string> unitMaterials = new List<string>();
        List<float[]> unitStartLocations = new List<float[]>();

        foreach (EditorStartPoint sp in startPoints)
        {
            if (sp.unitStartLocation != Vector3.zero)
            {
                unitStartLocations.Add(sp.uSL);
                unitMaterials.Add(sp.unitMaterialName);
            }
        }

        return new Map()
        {
            SizeMapX = this.sizeMapX,
            SizeMapY = this.sizeMapY,
            Maps = this.maps,
            UnitMaterials = unitMaterials,
            UnitStartLocations = unitStartLocations,
            CreateTime = System.DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy HH-mm-ss"),
            UpdateTime = System.DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy HH-mm-ss"),
        };
    }

    public void ImportMap(Map map)
    {
        DeleteMapGameObjects();
        InitializeStartTerrain(sizeMapX, sizeMapY);
        InitializeNewMap(map);
    }

    public void DeleteMapGameObjects()
    {
        for (int k = 0; k < mapCount; k++)
        {
            for (int i = 0; i < sizeMapX; i++)
            {
                for (int j = 0; j < sizeMapY; j++)
                {
                    if (mapsPrefabs[k][i][j] != null)
                    {
                        DeleteGameObject(i, j, k);
                    }
                }
            }
        }

        GameObjectToDelete(Terrain);
    }

    private void InitializeNewMap(Map map)
    {
        maps = map.Maps;
        for (int i = 0; i < mapCount; i++)
        {
            for (int j = 0; j < sizeMapX; j++)
            {
                for (int k = 0; k < sizeMapY; k++)
                {
                    if (maps[i][j][k] > 0)
                    {
                        mapsPrefabs[i][j][k] = Instantiate(ItemButtons[maps[i][j][k]].item.ItemPrefab, new Vector3(j, ItemButtons[maps[i][j][k]].item.ItemHeightPosY, k), ItemButtons[maps[i][j][k]].item.ItemPrefab.transform.rotation);
                    }
                }
            }
        }
    }
    #endregion

    //public void NewTerrain()
    //{
    //    DeleteMapGameObjects();
    //    InitializeOpenEditor();
    //}
}
