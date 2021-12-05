using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorManager : MonoBehaviour
{
    private Map _map = new Map();

    internal GameObject[][][] mapsPrefabs;
    internal GameObject Terrain;

    public ItemController[] ItemButtons;
    internal int CurrentButtonPressed;

    RaycastHit hit;
    internal Vector3 v;

    public Slider sizeSlider;
    public Text valueSizeSliderText;
    public Toggle singleMultiToggle;
    public Text valueSingleMultiToggleText;
    public Toggle replaceToggle;
    public Text valueReplaceToggleText;

    public UnitEditorPanel unitEditorPanel;
    public GameObject[] panelsToActive;
    public GameObject[] craftingPanels;
    public GameObject sizeMapPanel;

    private void Start()
    {
        InitializeOpenEditor();
        InitializeIDButtons();
        InitializeStartMaps();
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
        _map.Maps = new int[_map.MapsCount][][];
        mapsPrefabs = new GameObject[_map.MapsCount][][];
    }

    internal void InitializeStartTerrain(int sizeX, int sizeY, int mainGround = 0)
    {
        foreach (GameObject panel in panelsToActive)
        {
            panel.SetActive(true);
        }
        foreach (GameObject panel in craftingPanels)
        {
            panel.SetActive(false);
        }
        sizeMapPanel.SetActive(false);

        Vector3 firstScale = ItemButtons[mainGround].item.ItemPrefab.transform.localScale;

        GameObject basicTerrainPrefab = ItemButtons[mainGround].item.ItemPrefab;
        basicTerrainPrefab.gameObject.transform.localScale = new Vector3(sizeX * firstScale.x, firstScale.y, sizeY * firstScale.z);
        Terrain = Instantiate(basicTerrainPrefab, new Vector3(sizeX / 2 - 0.5f, 0, sizeY / 2 - 0.5f), basicTerrainPrefab.transform.rotation);

        basicTerrainPrefab.gameObject.transform.localScale = firstScale;

        InitializeTerrainArrays(sizeX, sizeY);
    }

    private void InitializeTerrainArrays(int sizeX, int sizeY)
    {
        _map.SizeMapX = sizeX;
        _map.SizeMapY = sizeY;

        for (int i = 0; i < _map.MapsCount; i++)
        {
            _map.Maps[i] = new int[sizeX][];
            mapsPrefabs[i] = new GameObject[sizeX][];

            for (int j = 0; j < sizeX; j++)
            {
                _map.Maps[i][j] = new int[sizeY];
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

            if ((vx < _map.SizeMapX && vx > -1) && (vz < _map.SizeMapY && vz > -1))
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

                if (vxSlider < 0 || vxSlider >= _map.SizeMapX || vySlider < 0 || vySlider >= _map.SizeMapY)
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
            InitializeEditorStartPoints();

            int fullSize = ItemButtons[CurrentButtonPressed].item.ItemPrefab.GetComponent<ItemStartPoint>().BuildSize;

            if (vx < fullSize / 2 || vx >= _map.SizeMapX - fullSize / 2 || vz < fullSize / 2 || vz >= _map.SizeMapY - fullSize / 2)
            {
                return;
            }

            int tempvx, tempvz;

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = vx - fullSize / 2 + i;
                    tempvz = vz - fullSize / 2 + j;

                    if (vx < 0 || vx >= _map.SizeMapX || vz < 0 || vz >= _map.SizeMapY || _map.Maps[level][tempvx][tempvz] != 0) return;
                }
            }

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = vx - fullSize / 2 + i;
                    tempvz = vz - fullSize / 2 + j;
                    if (tempvx == vx && tempvz == vz) continue;
                    _map.Maps[level][tempvx][tempvz] = -1;
                }
            }
        }

        CreateGameObject(vx, vz, level);
    }

    private void InitializeEditorStartPoints()
    {
        if (_map.EditorStartPoints == null)
        {
            _map.EditorStartPoints = new List<EditorStartPoint>();

            foreach (Button button in unitEditorPanel.ColorsUnitsButtons)
            {
                _map.EditorStartPoints.Add(new EditorStartPoint()
                {
                    UnitMaterialName = button.GetComponent<UnitColorButton>().unitMaterial.name,
                    UnitStartLocation = new float[3] { 0, 0, 0 },
                });
            }
        }
    }

    private void CreateGameObject(int vx, int vz, int level)
    {
        if (replaceToggle.isOn && _map.Maps[level][vx][vz] > 0 && _map.Maps[level][vx][vz] != CurrentButtonPressed)
        {
            if (mapsPrefabs[level][vx][vz].GetComponent<ItemStartPoint>() != null)
            {
                EditorStartPoint spu = _map.EditorStartPoints.Where(u => u.UnitStartLocation[0] == vx && u.UnitStartLocation[2] == vz).First();

                int areaToReset = mapsPrefabs[level][vx][vz].GetComponent<ItemStartPoint>().BuildSize;
                int tempvx, tempvz;

                for (int i = 0; i < areaToReset; i++)
                {
                    for (int j = 0; j < areaToReset; j++)
                    {
                        tempvx = vx - areaToReset / 2 + i;
                        tempvz = vz - areaToReset / 2 + j;

                        _map.Maps[level][tempvx][tempvz] = 0;
                    }
                }

                spu.UnitStartLocation[0] = 0;
                spu.UnitStartLocation[1] = 0;
                spu.UnitStartLocation[2] = 0;
            }


            DeleteGameObject(vx, vz, level);
        }

        if (_map.Maps[level][vx][vz] == 0)
        {
            if (ItemButtons[CurrentButtonPressed] is ItemUnitController)
            {
                UpdateStartUnitList(vx, vz, level);
            }

            _map.Maps[level][vx][vz] = CurrentButtonPressed;
            mapsPrefabs[level][vx][vz] = Instantiate(ItemButtons[CurrentButtonPressed].item.ItemPrefab,
                new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz),
                ItemButtons[CurrentButtonPressed].item.ItemPrefab.transform.rotation);
        }
    }

    private void UpdateStartUnitList(int vx, int vz, int level)
    {
        EditorStartPoint spu = _map.EditorStartPoints.Where(u => u.UnitMaterialName == unitEditorPanel.ActualMaterial.name).First();

        if (ArrayToVector3(spu.UnitStartLocation) == Vector3.zero)
        {
            //spu.UnitStartLocation = Vector3ToArray(new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz));
            spu.UnitStartLocation[0] = vx;
            spu.UnitStartLocation[1] = ItemButtons[CurrentButtonPressed].item.ItemHeightPosY;
            spu.UnitStartLocation[2] = vz;

            return;
        }

        if (_map.Maps[level][(int)spu.UnitStartLocation[0]][(int)spu.UnitStartLocation[2]] == CurrentButtonPressed)
        {
            int tempvx, tempvz;
            int fullSize = ItemButtons[CurrentButtonPressed].item.ItemPrefab.GetComponent<ItemStartPoint>().BuildSize;

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = (int)spu.UnitStartLocation[0] - fullSize / 2 + i;
                    tempvz = (int)spu.UnitStartLocation[2] - fullSize / 2 + j;

                    _map.Maps[level][tempvx][tempvz] = 0;
                }
            }
        }

        DeleteGameObject((int)spu.UnitStartLocation[0], (int)spu.UnitStartLocation[2], level);
        spu.UnitStartLocation[0] = vx;
        spu.UnitStartLocation[1] = ItemButtons[CurrentButtonPressed].item.ItemHeightPosY;
        spu.UnitStartLocation[2] = vz;
    }

    private void DeleteGameObject(int vx, int vz, int level)
    {
        GameObjectToDelete(mapsPrefabs[level][vx][vz]);

        _map.Maps[level][vx][vz] = 0;
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
            for (int k = 0; k < _map.MapsCount; k++)
            {
                string test;
                for (int i = 0; i < _map.SizeMapX; i++)
                {
                    test = "";
                    for (int j = 0; j < _map.SizeMapY; j++)
                    {
                        test += _map.Maps[k][i][j];
                    }
                    Debug.Log(test);
                }

                Debug.LogError("-------------");
            }
        }

        if (Input.GetMouseButtonDown(3))
        {
            foreach (EditorStartPoint editorStartPoint in _map.EditorStartPoints)
            {
                Debug.Log(editorStartPoint.UnitStartLocation + " " + editorStartPoint.UnitMaterialName);
            }

        }
    }

    #region Export Import
    public Map ExportMap()
    {
        _map.GameStartPoints = new List<float[]>();

        if (_map.EditorStartPoints != null)
        {
            foreach (EditorStartPoint eSP in _map.EditorStartPoints)
            {
                if (eSP.UnitStartLocation[0] != 0 && eSP.UnitStartLocation[1] != 0 && eSP.UnitStartLocation[2] != 0)
                {
                    _map.GameStartPoints.Add(new float[3] { eSP.UnitStartLocation[0], eSP.UnitStartLocation[1], eSP.UnitStartLocation[2] });
                }
            }
        }

        return _map;
    }

    public void ImportMap(Map map)
    {
        DeleteMapGameObjects(); // Delete objects
        InitializeStartTerrain(_map.SizeMapX, _map.SizeMapY); //  zero arrays 

        _map = map;
        InitializeNewMap();
    }

    public void DeleteMapGameObjects()
    {
        for (int k = 0; k < _map.MapsCount; k++)
        {
            for (int i = 0; i < _map.SizeMapX; i++)
            {
                for (int j = 0; j < _map.SizeMapY; j++)
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

    private void InitializeNewMap()
    {
        for (int i = 0; i < _map.MapsCount; i++)
        {
            for (int j = 0; j < _map.SizeMapX; j++)
            {
                for (int k = 0; k < _map.SizeMapY; k++)
                {
                    if (_map.Maps[i][j][k] > 0)
                    {
                        mapsPrefabs[i][j][k] = Instantiate(ItemButtons[_map.Maps[i][j][k]].item.ItemPrefab, new Vector3(j, ItemButtons[_map.Maps[i][j][k]].item.ItemHeightPosY, k), ItemButtons[_map.Maps[i][j][k]].item.ItemPrefab.transform.rotation);
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

    public int[] GetSizeMap()
    {
        return new int[3] { _map.SizeMapX, _map.SizeMapY, _map.MapsCount };
    }

    private Vector3 ArrayToVector3(float[] array)
    {
        return new Vector3(array[0], array[1], array[2]);
    }

    private float[] Vector3ToArray(Vector3 vec)
    {
        return new float[3] { vec.x, vec.y, vec.z };
    }


}


public class test
{

    // in generate unit nature
    //            if (ItemButtons[CurrentButtonPressed] is ItemUnitController)
    //        {
    //            int fullSize = ItemButtons[CurrentButtonPressed].item.ItemPrefab.GetComponent<StartPointUnit>().buildSize;

    //            if (vx<fullSize / 2 || vx >= _map.SizeMapX - fullSize / 2 || vz<fullSize / 2 || vz >= _map.SizeMapY - fullSize / 2)
    //            {
    //                return;
    //            }

    //int tempvx, tempvz;

    //for (int i = 0; i < fullSize; i++)
    //{
    //    for (int j = 0; j < fullSize; j++)
    //    {
    //        tempvx = vx - fullSize / 2 + i;
    //        tempvz = vz - fullSize / 2 + j;

    //        if (vx < 0 || vx >= _map.SizeMapX || vz < 0 || vz >= _map.SizeMapY || _map.Maps[level][tempvx][tempvz] != 0) return;
    //    }
    //}

    //for (int i = 0; i < fullSize; i++)
    //{
    //    for (int j = 0; j < fullSize; j++)
    //    {
    //        tempvx = vx - fullSize / 2 + i;
    //        tempvz = vz - fullSize / 2 + j;
    //        if (tempvx == vx && tempvz == vz) continue;
    //        _map.Maps[level][tempvx][tempvz] = -1;
    //    }
    //}
    //        }




    //InitializeStartPointUnitsList();


    //private void InitializeStartPointUnitsList()
    //{
    //    foreach (Button button in unitEditorPanel.ColorsUnitsButtons)
    //    {
    //        UnitEditorButton unitEditorButton = button.gameObject.GetComponent<UnitEditorButton>();

    //        startPoints.Add(new EditorStartPoint
    //        {
    //            unitMaterialName = unitEditorButton.unitMaterial.name,
    //            unitStartLocation = Vector3.zero,
    //        });
    //    }
    //}



    //// w create game object
    ///
    //        if (mapsPrefabs[level][vx][vz].GetComponent<StartPointUnit>() != null)
    //        {
    //            EditorStartPoint spu = startPoints.Where(u => u.unitStartLocation.x == vx && u.unitStartLocation.z == vz).First();

    //int areaToReset = mapsPrefabs[level][vx][vz].GetComponent<StartPointUnit>().buildSize;
    //int tempvx, tempvz;

    //            for (int i = 0; i<areaToReset; i++)
    //            {
    //                for (int j = 0; j<areaToReset; j++)
    //                {
    //                    tempvx = vx - areaToReset / 2 + i;
    //                    tempvz = vz - areaToReset / 2 + j;

    //                    _map.Maps[level][tempvx][tempvz] = 0;
    //                }
    //            }

    //            spu.unitStartLocation = Vector3.zero;
    //        }



    /* 
     *             if (ItemButtons[CurrentButtonPressed] is ItemUnitController)
            {
                UpdateStartUnitList(vx, vz, level);
            }


         private void UpdateStartUnitList(int vx, int vz, int level)
    {
        EditorStartPoint spu = startPoints.Where(u => u.unitMaterialName == unitEditorPanel.ActualMaterial.name).First();

        if (spu.unitStartLocation == Vector3.zero)
        {
            spu.unitStartLocation = new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz);
            return;
        }

        if (_map.Maps[level][(int)spu.unitStartLocation.x][(int)spu.unitStartLocation.z] == CurrentButtonPressed)
        {
            int tempvx, tempvz;
            int fullSize = ItemButtons[CurrentButtonPressed].item.ItemPrefab.GetComponent<StartPointUnit>().buildSize;

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = (int)spu.unitStartLocation.x - fullSize / 2 + i;
                    tempvz = (int)spu.unitStartLocation.z - fullSize / 2 + j;

                    _map.Maps[level][tempvx][tempvz] = 0;
                }
            }
        }

        DeleteGameObject((int)spu.unitStartLocation.x, (int)spu.unitStartLocation.z, level);
        spu.unitStartLocation = new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz);
    }
     */



}