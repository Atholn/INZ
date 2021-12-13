using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorManager : MonoBehaviour
{
    enum TypeOfDestroy
    {
        Terrain = 0,
        Nature = 1,
        All = 2,
    }

    private MapWorld _mapWorldInfo = new MapWorld();

    internal GameObject[][][] mapsPrefabs;
    internal GameObject Terrain;
    internal int CurrentButtonPressed;

    RaycastHit hit;
    internal Vector3 v;

    public ItemController[] ItemControllers;
    public ItemDeleteController ItemDelete;
    public Slider sizeSlider;
    public Text valueSizeSliderText;
    public Toggle singleMultiToggle;
    public Text valueSingleMultiToggleText;
    public Toggle replaceToggle;
    public Text valueReplaceToggleText;
    public Dropdown TypeTerrainToDestroy;

    public UnitEditorPanel unitEditorPanel;
    public GameObject[] panelsToActive;
    public GameObject[] craftingPanels;
    public GameObject sizeMapPanel;

    private void Start()
    {
        InitializeOpenEditor();
        InitializeIDButtons();
        InitializeHeightMainTerrain();
        InitializeDestroyDropdown();
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

    private void InitializeDestroyDropdown()
    {
        List<string> typeOfDestroy = Enum.GetValues(typeof(TypeOfDestroy)).Cast<TypeOfDestroy>().Select(s => s.ToString()).ToList();
        TypeTerrainToDestroy.AddOptions(typeOfDestroy);
        TypeTerrainToDestroy.value = TypeTerrainToDestroy.options.Count - 1;
    }
    private void Update()
    {
        UpdateSettingsPanel();
        UpdateLocation();
        UpdateCreate();
        UpdateDestroy();
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

            if ((vx < _mapWorldInfo.SizeMapX && vx > -1) && (vz < _mapWorldInfo.SizeMapY && vz > -1))
            {
                if (CanCreate(vx, vz) == 0)
                {
                    return;
                }

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

                if (vxSlider < 0 || vxSlider >= _mapWorldInfo.SizeMapX || vySlider < 0 || vySlider >= _mapWorldInfo.SizeMapY)
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
            if (_mapWorldInfo.StartPoints == null)
            {
                InitializeEditorStartPoints();
            }

            int fullSize = (ItemControllers[CurrentButtonPressed].item as ItemStartPoint).BuildSize;

            if (vx < fullSize / 2 || vx >= _mapWorldInfo.SizeMapX - fullSize / 2 || vz < fullSize / 2 || vz >= _mapWorldInfo.SizeMapY - fullSize / 2)
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

                    if (vx < 0 || vx >= _mapWorldInfo.SizeMapX || vz < 0 || vz >= _mapWorldInfo.SizeMapY || _mapWorldInfo.Maps[level][tempvx][tempvz] != 0)
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

                    _mapWorldInfo.Maps[level][tempvx][tempvz] = -1;
                }
            }
        }

        CreateGameObject(vx, vz, level);
    }

    private void InitializeEditorStartPoints()
    {
        _mapWorldInfo.StartPoints = new List<EditorStartPoint>();

        foreach (Button button in unitEditorPanel.ColorsUnitsButtons)
        {
            _mapWorldInfo.StartPoints.Add(new EditorStartPoint()
            {
                UnitMaterialName = button.GetComponent<UnitColorButton>().unitMaterial.name,
                UnitStartLocation = new float[3] { 0, 0, 0 },
            });
        }
    }

    private void CreateGameObject(int vx, int vz, int level)
    {
        if (replaceToggle.isOn && _mapWorldInfo.Maps[level][vx][vz] != 0 && _mapWorldInfo.Maps[level][vx][vz] != CurrentButtonPressed)
        {
            if (_mapWorldInfo.Maps[level][vx][vz] == -1 && mapsPrefabs[level][vx][vz] == null)
            {
                float tmpMin, min = -1;
                int j = -1;
                for (int i = 0; i < _mapWorldInfo.StartPoints.Count; i++)
                {
                    if (_mapWorldInfo.StartPoints[i].UnitStartLocation[0] == 0 && _mapWorldInfo.StartPoints[i].UnitStartLocation[1] == 0 && _mapWorldInfo.StartPoints[i].UnitStartLocation[2] == 0)
                    {
                        continue;
                    }

                    tmpMin = (vx - _mapWorldInfo.StartPoints[i].UnitStartLocation[0]) * (vx - _mapWorldInfo.StartPoints[i].UnitStartLocation[0]) - (vz - _mapWorldInfo.StartPoints[i].UnitStartLocation[2]) * (vx - _mapWorldInfo.StartPoints[i].UnitStartLocation[2]);
                    if (min == -1 || tmpMin < min)
                    {
                        min = tmpMin;
                        j = i;
                    }
                }

                ResetAreaAfterCreate((int)_mapWorldInfo.StartPoints[j].UnitStartLocation[0], (int)_mapWorldInfo.StartPoints[j].UnitStartLocation[2], level);
            }
            else
            {
                if (mapsPrefabs[level][vx][vz].GetComponent<ItemStartPoint>() != null)
                {
                    ResetAreaAfterCreate(vx, vz, level);
                }

                DeleteGameObject(vx, vz, level);
            }
        }

        if (_mapWorldInfo.Maps[level][vx][vz] == 0)
        {
            if (ItemControllers[CurrentButtonPressed] is ItemUnitController)
            {
                UpdateStartUnitList(vx, vz, level);
                _mapWorldInfo.Maps[level][vx][vz] = -1;
            }
            else
            {
                _mapWorldInfo.Maps[level][vx][vz] = CurrentButtonPressed;
            }

            mapsPrefabs[level][vx][vz] = Instantiate(ItemControllers[CurrentButtonPressed].item.ItemPrefab,
                new Vector3(vx, ItemControllers[CurrentButtonPressed].item.ItemHeightPosY, vz),
                ItemControllers[CurrentButtonPressed].item.ItemPrefab.transform.rotation);
        }
    }

    private void ResetAreaAfterCreate(int vx, int vz, int level)
    {
        EditorStartPoint spu = _mapWorldInfo.StartPoints.Where(u => u.UnitStartLocation[0] == vx && u.UnitStartLocation[2] == vz).First();

        int areaToReset = mapsPrefabs[level][vx][vz].GetComponent<ItemStartPoint>().BuildSize;
        int tempvx, tempvz;

        for (int i = 0; i < areaToReset; i++)
        {
            for (int j = 0; j < areaToReset; j++)
            {
                tempvx = vx - areaToReset / 2 + i;
                tempvz = vz - areaToReset / 2 + j;

                _mapWorldInfo.Maps[level][tempvx][tempvz] = 0;
            }
        }

        DeleteGameObject(vx, vz, level);

        spu.UnitStartLocation[0] = 0;
        spu.UnitStartLocation[1] = 0;
        spu.UnitStartLocation[2] = 0;
    }

    private void UpdateStartUnitList(int vx, int vz, int level)
    {
        EditorStartPoint spu = _mapWorldInfo.StartPoints.Where(u => u.UnitMaterialName == unitEditorPanel.ActualMaterial.name).First();

        if (ArrayToVector3(spu.UnitStartLocation) == Vector3.zero)
        {
            //spu.UnitStartLocation = Vector3ToArray(new Vector3(vx, ItemButtons[CurrentButtonPressed].item.ItemHeightPosY, vz));
            spu.UnitStartLocation[0] = vx;
            spu.UnitStartLocation[1] = ItemControllers[CurrentButtonPressed].item.ItemHeightPosY;
            spu.UnitStartLocation[2] = vz;

            return;
        }

        if (_mapWorldInfo.Maps[level][(int)spu.UnitStartLocation[0]][(int)spu.UnitStartLocation[2]] == -1)
        {
            int tempvx, tempvz;
            int fullSize = ItemControllers[CurrentButtonPressed].item.ItemPrefab.GetComponent<ItemStartPoint>().BuildSize;

            for (int i = 0; i < fullSize; i++)
            {
                for (int j = 0; j < fullSize; j++)
                {
                    tempvx = (int)spu.UnitStartLocation[0] - fullSize / 2 + i;
                    tempvz = (int)spu.UnitStartLocation[2] - fullSize / 2 + j;

                    _mapWorldInfo.Maps[level][tempvx][tempvz] = 0;
                }
            }
        }

        DeleteGameObject((int)spu.UnitStartLocation[0], (int)spu.UnitStartLocation[2], level);
        spu.UnitStartLocation[0] = vx;
        spu.UnitStartLocation[1] = ItemControllers[CurrentButtonPressed].item.ItemHeightPosY;
        spu.UnitStartLocation[2] = vz;
    }

    private void UpdateDestroy()
    {
        if (ItemDelete.Pressed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (TypeTerrainToDestroy.value)
                {
                    case 0:
                        if (mapsPrefabs[0][(int)v.x][(int)v.z] != null)
                        {
                            DeleteGameObject((int)v.x, (int)v.z, 0);
                        }
                        break;
                    case 1:
                        if (mapsPrefabs[1][(int)v.x][(int)v.z] != null)
                        {
                            DeleteGameObject((int)v.x, (int)v.z, 1);
                        }
                        break;
                    case 2:
                        if (mapsPrefabs[0][(int)v.x][(int)v.z] != null)
                        {
                            DeleteGameObject((int)v.x, (int)v.z, 0);
                        }
                        if (mapsPrefabs[1][(int)v.x][(int)v.z] != null)
                        {
                            DeleteGameObject((int)v.x, (int)v.z, 1);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void DeleteGameObject(int vx, int vz, int level)
    {
        GameObjectToDelete(mapsPrefabs[level][vx][vz]);

        _mapWorldInfo.Maps[level][vx][vz] = 0;
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

            ItemDelete.Pressed = false;

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
            for (int k = 0; k < _mapWorldInfo.MapsCount; k++)
            {
                string test;
                for (int i = 0; i < _mapWorldInfo.SizeMapX; i++)
                {
                    test = "";
                    for (int j = 0; j < _mapWorldInfo.SizeMapY; j++)
                    {
                        test += _mapWorldInfo.Maps[k][i][j];
                    }
                    Debug.Log(test);
                }

                Debug.LogError("-------------");
            }
        }
    }

    #region Export Import
    public MapWorld ExportMap()
    {
        return _mapWorldInfo;
    }

    public void ImportMap(MapWorld newMap)
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

        MapLoader.ResetAndLoad(ref _mapWorldInfo, ref newMap, ref mapsPrefabs, ref Terrain, GetPrefabsOnly());

        ItemController startPointPrefab = ItemControllers.Where(i => i is ItemUnitController).FirstOrDefault();
        foreach (EditorStartPoint eSP in _mapWorldInfo.StartPoints)
        {
            if (ArrayToVector3(eSP.UnitStartLocation) == Vector3.zero)
            {
                continue;
            }

            //startPointPrefab.item.ItemPrefab.GetComponent<MeshRenderer>().material = unitEditorPanel.ColorsUnitsButtons
            //               .Where(m => m.GetComponent<UnitColorButton>().unitMaterial.name == eSP.UnitMaterialName)
            //               .Select(m => m.GetComponent<UnitColorButton>().unitMaterial).FirstOrDefault(); 
            // sprawdza sie tylko wtedy kiedy wejdziesz w panel z kolorami i startpointemm

            List<Material> materialList = MapToPlayStorage.ImportResources<Material>("Materials/Units/", ".mat");
            startPointPrefab.item.ItemPrefab.GetComponent<MeshRenderer>().material = materialList.Where(m => m.name == eSP.UnitMaterialName).FirstOrDefault();

            mapsPrefabs[startPointPrefab.item.ItemHeightLevel][(int)eSP.UnitStartLocation[0]][(int)eSP.UnitStartLocation[2]] = Instantiate(startPointPrefab.item.ItemPrefab, new Vector3(eSP.UnitStartLocation[0], startPointPrefab.item.ItemHeightPosY, eSP.UnitStartLocation[2]), startPointPrefab.item.ItemPrefab.transform.rotation);
        }
    }

    internal void InitializeStartTerrain(int sizeX, int sizeY, int mainGroundID)
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

        _mapWorldInfo.MainGroundID = mainGroundID;
        
        _mapWorldInfo.SizeMapX = sizeX;
        _mapWorldInfo.SizeMapY = sizeY;

        MapLoader.InitializeNewMap(ref _mapWorldInfo, ref mapsPrefabs, ref Terrain, GetPrefabsOnly(), _mapWorldInfo.MainGroundID);
    }
    #endregion

    private List<Item> GetPrefabsOnly()
    {
        return ItemControllers.Select(t => t.item).ToList();
    }

    public int[] GetSizeMap()
    {
        return new int[3] { _mapWorldInfo.SizeMapX, _mapWorldInfo.SizeMapY, _mapWorldInfo.MapsCount };
    }

    private Vector3 ArrayToVector3(float[] array)
    {
        return new Vector3(array[0], array[1], array[2]);
    }

    private float[] Vector3ToArray(Vector3 vec)
    {
        return new float[3] { vec.x, vec.y, vec.z };
    }

    public int CanCreate(int x, int z)
    {
        if (ItemControllers[CurrentButtonPressed].item.ItemHeightLevel == 1)
        {
            if (ItemControllers[CurrentButtonPressed].item is ItemStartPoint)
            {
                int area = (ItemControllers[CurrentButtonPressed].GetComponent<ItemUnitController>().item as ItemStartPoint).BuildSize;
                int tempvx, tempvz;

                for (int i = 0; i < area; i++)
                {
                    for (int j = 0; j < area; j++)
                    {
                        tempvx = x - area / 2 + i;
                        tempvz = z - area / 2 + j;

                        if (_mapWorldInfo.Maps[1][tempvx][tempvz] != 0 || !(ItemControllers[_mapWorldInfo.Maps[0][tempvx][tempvz]].item as ItemTerrain).AllowsBuild)
                        {
                            return 0;
                        }
                    }
                }

                return 1;
            }

            if (_mapWorldInfo.Maps[1][x][z] == 0 && mapsPrefabs[1][x][z] == null && (ItemControllers[_mapWorldInfo.Maps[0][x][z]].item as ItemTerrain).AllowsBuild) return 1;

            if (!replaceToggle.isOn && (_mapWorldInfo.Maps[1][x][z] != 0 || mapsPrefabs[1][x][z] != null || !((ItemControllers[_mapWorldInfo.Maps[0][x][z]].item as ItemTerrain).AllowsBuild))) return 0;

            if (replaceToggle.isOn && !((ItemControllers[_mapWorldInfo.Maps[0][x][z]].item as ItemTerrain).AllowsBuild)) return 0;

            return -1;
        }

        if (mapsPrefabs[0][x][z] == null && ((_mapWorldInfo.Maps[1][x][z] == 0) || ((ItemControllers[CurrentButtonPressed].item as ItemTerrain).AllowsBuild && _mapWorldInfo.Maps[1][x][z] != 0))) return 1;

        if ((!replaceToggle.isOn && mapsPrefabs[0][x][z] != null) || (!(ItemControllers[CurrentButtonPressed].item as ItemTerrain).AllowsBuild) && _mapWorldInfo.Maps[1][x][z] != 0) return 0;

        return -1;
    }

    internal float[][][] GeneratePixelsColors()
    {
        float[][][] newPixelsColors = new float[GetSizeMap()[0]][][];

        for (int i = 0; i < newPixelsColors.Length; i++)
        {
            newPixelsColors[i] = new float[GetSizeMap()[1]][];

            for (int j = 0; j < newPixelsColors[i].Length; j++)
            {
                newPixelsColors[i][j] = new float[3];

                GameObject prefab = mapsPrefabs[1][i][j] != null ? mapsPrefabs[1][i][j] :
                    mapsPrefabs[0][i][j] != null ? mapsPrefabs[0][i][j] : Terrain;

                MeshRenderer mesh = prefab.gameObject.GetComponent<MeshRenderer>();

                newPixelsColors[i][j][0] = mesh.material.color.r;
                newPixelsColors[i][j][1] = mesh.material.color.g;
                newPixelsColors[i][j][2] = mesh.material.color.b;
            }
        }

        return newPixelsColors;
    }
}
