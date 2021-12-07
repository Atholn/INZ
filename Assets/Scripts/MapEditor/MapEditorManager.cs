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

    public ItemController[] ItemControllers;
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
        InitializeHeightMainTerrain();
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
        for (int i = 0; i < ItemControllers.Length; i++)
        {
            ItemControllers[i].item.ID = i;
        }
    }
    private void InitializeHeightMainTerrain()
    {
        foreach (ItemController itemController in ItemControllers)
        {
            if (itemController.item is ItemTerrain)
            {
                (itemController.item as ItemTerrain).Initialize();
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

        if (ItemControllers[CurrentButtonPressed].item.ItemHeightLevel == 0)
        {
            ItemControllers[CurrentButtonPressed].item.ItemImage.transform.localScale = new Vector3(ItemControllers[CurrentButtonPressed].firstScale.x * sizeSlider.value, ItemControllers[CurrentButtonPressed].firstScale.y * sizeSlider.value, ItemControllers[CurrentButtonPressed].firstScale.z);
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
        if (ItemControllers[CurrentButtonPressed].Clicked)
        {
            int vx = (int)(v.x - v.x % 1);
            int vz = (int)(v.z - v.z % 1);

            if ((vx < _map.SizeMapX && vx > -1) && (vz < _map.SizeMapY && vz > -1))
            {
                if (ItemControllers[CurrentButtonPressed].item.ItemHeightLevel == 0)
                {
                    GenerateTerrain(vx, vz, ItemControllers[CurrentButtonPressed].item.ItemHeightLevel);
                }

                if (ItemControllers[CurrentButtonPressed].item.ItemHeightLevel == 1)
                {
                    GenerateNatureUnit(vx, vz, ItemControllers[CurrentButtonPressed].item.ItemHeightLevel);
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
        if (ItemControllers[CurrentButtonPressed] is ItemUnitController)
        {
            if (_map.EditorStartPoints == null)
            {
                InitializeEditorStartPoints();
            }

            int fullSize = (ItemControllers[CurrentButtonPressed].item as ItemStartPoint).BuildSize;

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

                    if (vx < 0 || vx >= _map.SizeMapX || vz < 0 || vz >= _map.SizeMapY || _map.Maps[level][tempvx][tempvz] != 0)
                    {
                        return;
                    }
                }
            }

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = vx - fullSize / 2 + i;
                    tempvz = vz - fullSize / 2 + j;

                    if (tempvx == vx && tempvz == vz)
                    {
                        continue;
                    }

                    _map.Maps[level][tempvx][tempvz] = -1;
                }
            }


        }

        CreateGameObject(vx, vz, level);
    }

    private void InitializeEditorStartPoints()
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

        if (_map.Maps[level][vx][vz] == 0 && CanCreate(vx, vz))
        {
            if (ItemControllers[CurrentButtonPressed] is ItemUnitController)
            {
                UpdateStartUnitList(vx, vz, level);
                _map.Maps[level][vx][vz] = -1;
            }
            else
            {
                _map.Maps[level][vx][vz] = CurrentButtonPressed;
            }

            mapsPrefabs[level][vx][vz] = Instantiate(ItemControllers[CurrentButtonPressed].item.ItemPrefab,
                new Vector3(vx, ItemControllers[CurrentButtonPressed].item.ItemHeightPosY, vz),
                ItemControllers[CurrentButtonPressed].item.ItemPrefab.transform.rotation);
        }
    }

    private void UpdateStartUnitList(int vx, int vz, int level)
    {
        EditorStartPoint spu = _map.EditorStartPoints.Where(u => u.UnitMaterialName == unitEditorPanel.ActualMaterial.name).First();

        if (ArrayToVector3(spu.UnitStartLocation) == Vector3.zero)
        {
            //spu.UnitStartLocation = Vector3ToArray(new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz));
            spu.UnitStartLocation[0] = vx;
            spu.UnitStartLocation[1] = ItemControllers[CurrentButtonPressed].item.ItemHeightPosY;
            spu.UnitStartLocation[2] = vz;

            return;
        }

        if (_map.Maps[level][(int)spu.UnitStartLocation[0]][(int)spu.UnitStartLocation[2]] == -1)
        {
            int tempvx, tempvz;
            int fullSize = ItemControllers[CurrentButtonPressed].item.ItemPrefab.GetComponent<ItemStartPoint>().BuildSize;

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
        spu.UnitStartLocation[1] = ItemControllers[CurrentButtonPressed].item.ItemHeightPosY;
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
            for (int i = 0; i < ItemControllers.Length; i++)
            {
                ItemControllers[i].Clicked = false;
                if (ItemControllers[i].item.ItemHeightLevel == 0)
                {
                    ItemControllers[i].item.ItemImage.transform.localScale = ItemControllers[i].firstScale;
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
            foreach (float[] editorStartPoint in _map.GameStartPoints)
            {
                Debug.Log(editorStartPoint[0] + editorStartPoint[1] + editorStartPoint[2]);
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

    public void ImportMap(Map newMap)
    {
        MapLoader.ResetAndLoad(ref _map, ref newMap, ref mapsPrefabs, ref Terrain, ItemControllers, newMap.MainGroundID);

        //InitializeStartTerrain(_map.SizeMapX, _map.SizeMapY); //  zero arrays 

        //_map = newMap;
        //InitializeNewMap();

        foreach (GameObject panel in panelsToActive)
        {
            panel.SetActive(true);
        }
        foreach (GameObject panel in craftingPanels)
        {
            panel.SetActive(false);
        }
        sizeMapPanel.SetActive(false);

        ItemController startPointPrefab = ItemControllers.Where(i => i is ItemUnitController).FirstOrDefault();
        foreach (EditorStartPoint eSP in _map.EditorStartPoints)
        {
            if (ArrayToVector3(eSP.UnitStartLocation) == Vector3.zero)
            {
                continue;
            }

            startPointPrefab.item.ItemPrefab.GetComponent<MeshRenderer>().material = unitEditorPanel.ColorsUnitsButtons
                           .Where(m => m.GetComponent<UnitColorButton>().unitMaterial.name == eSP.UnitMaterialName)
                           .Select(m => m.GetComponent<UnitColorButton>().unitMaterial).FirstOrDefault();

            mapsPrefabs[startPointPrefab.item.ItemHeightLevel][(int)eSP.UnitStartLocation[0]][(int)eSP.UnitStartLocation[2]] = Instantiate(startPointPrefab.item.ItemPrefab, new Vector3(eSP.UnitStartLocation[0], startPointPrefab.item.ItemHeightPosY, eSP.UnitStartLocation[2]), startPointPrefab.item.ItemPrefab.transform.rotation);
        }
    }

    internal void InitializeStartTerrain(int sizeX, int sizeY, int mainGroundID = 0)
    {
        _map.MainGroundID = mainGroundID;
        foreach (GameObject panel in panelsToActive)
        {
            panel.SetActive(true);
        }
        foreach (GameObject panel in craftingPanels)
        {
            panel.SetActive(false);
        }
        sizeMapPanel.SetActive(false);

        _map.SizeMapX = sizeX;
        _map.SizeMapY = sizeY;

        MapLoader.InitializeNewMap(ref _map, ref mapsPrefabs, ref Terrain, ItemControllers, mainGroundID);
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

    public bool CanCreate(int x, int z)
    {
        if (_map.Maps[1][x][z] != 0) return false;

        if (!(ItemControllers[_map.Maps[0][x][z]].item as ItemTerrain).AllowsBuild)
        {
            return false;
        }
        return true;
    }
}
