using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemController : MonoBehaviour
{
    internal bool Clicked = false;
    internal Vector3 firstScale = new Vector3();
    public Item item;
    public MapEditorManager editor;

    private void Awake()
    {
        firstScale = item.ItemPrefab.transform.localScale;
        item.ItemImage.transform.localScale = item.ItemPrefab.transform.localScale;
        editor = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<MapEditorManager>();        
    }

    public virtual void ButtonClicked()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, editor.ItemButtons[item.ID].item.ItemHeightLevel, Input.mousePosition.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Clicked = true;
        
        Instantiate(editor.ItemButtons[item.ID].item.ItemImage, new Vector3(worldPosition.x, editor.ItemButtons[item.ID].item.ItemHeightLevel, worldPosition.z), item.ItemPrefab.transform.rotation);

        editor.CurrentButtonPressed = item.ID;
    }
}
