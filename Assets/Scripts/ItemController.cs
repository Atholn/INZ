using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemController : MonoBehaviour
{
    public int ID;
    public bool Clicked = false;

    public LevelEditorManager editor;
    public GameObject ItemPrefab;
    public GameObject ItemImage;
    public float ItemHeightLevel;

    private void Start()
    {
        editor = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<LevelEditorManager>();
    }

    public void ButtonClicked()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, editor.ItemButtons[ID].ItemHeightLevel, Input.mousePosition.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Clicked = true;

        Instantiate(editor.ItemButtons[ID].ItemImage, new Vector3(worldPosition.x, editor.ItemButtons[ID].ItemHeightLevel, worldPosition.z), ItemPrefab.transform.rotation);

        editor.CurrentButtonPressed = ID;

    }
}
