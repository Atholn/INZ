using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SizeSelectionPanel : MonoBehaviour
{
    public Text SizeMapText;
    public Slider[] Sliders;
    public Image TypeOfMapImage;
    public MapEditorManager MapEditorManager;
    public Button TypeTerrainButton;
    public Scrollbar typeTerrainScrollbar;

    private int[] _sizes = new int[2];
    private int _groundId = 0;
    private List<Button> TypeTerrainButtons = new List<Button>();
    private List<ItemController> TypeTerrainLevel0 = new List<ItemController>();
    private Vector3 shiftPlaceVector = new Vector3(45, 15, 0);

    private void Start()
    {
        _sizes[0] = (int)Sliders[0].minValue;
        _sizes[1] = (int)Sliders[1].minValue;


        TypeTerrainLevel0 = MapEditorManager.ItemControllers.Where(t => t.item.ItemHeightLevel == 0).ToList();

        for (int i = 0; i < TypeTerrainLevel0.Count; i++)
        {
            TypeTerrainButtons.Add(Instantiate(TypeTerrainButton,
                new Vector3(
                     TypeTerrainButton.transform.position.x,  
                     0,
                    0),
                TypeTerrainButton.transform.rotation));

            TypeTerrainButtons[i].transform.SetParent(TypeTerrainButton.GetComponentInParent<Transform>().parent.GetComponentInParent<Transform>());
            TypeTerrainButtons[i].transform.localScale = new Vector3(1, 1, 1);

            TypeTerrainButtons[i].GetComponent<Image>().sprite = TypeTerrainLevel0[i].GetComponent<Image>().sprite;
        }

        typeTerrainScrollbar.value = 1;
        TypeTerrainButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        SizeMapText.text = $"{_sizes[0]} x {_sizes[1]}";
    }

    public void SetSizeButtons(int size)
    {
        for (int i = 0; i < Sliders.Length; i++)
        {
            _sizes[i] = size;
            Sliders[i].value = size;
        }
    }

    public void SetSizeSliders()
    {
        for (int i = 0; i < Sliders.Length; i++)
        {
            _sizes[i] = (int)Sliders[i].value;
        }
    }

    public void SetMainGround()
    {
        Button actualClickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        _groundId =  TypeTerrainButtons.IndexOf(actualClickedButton);
    }

    public void AcceptParametersMap()
    {
        MapEditorManager.InitializeStartTerrain(_sizes[0], _sizes[1], TypeTerrainLevel0[_groundId].item.ID);
    }

    public void CancelParametersMap()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
