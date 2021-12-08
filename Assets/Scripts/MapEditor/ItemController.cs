using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemController : MonoBehaviour
{
    internal bool Clicked = false;
    internal Vector3 firstScale = new Vector3();
    public Item item;
    public MapEditorManager editor;

    private void Awake()
    {
        var colors = gameObject.GetComponent<Button>().colors;
        colors.selectedColor = transform.parent.gameObject.GetComponentInParent<Image>().color;
        gameObject.GetComponent<Button>().colors = colors;

        firstScale = item.ItemPrefab.transform.localScale;
        item.ItemImage.transform.localScale = item.ItemPrefab.transform.localScale;
        editor = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<MapEditorManager>();        
    }

    public virtual void ButtonClicked()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, editor.ItemControllers[item.ID].item.ItemHeightLevel, Input.mousePosition.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Clicked = true;
        
        Instantiate(editor.ItemControllers[item.ID].item.ItemImage, new Vector3(worldPosition.x, editor.ItemControllers[item.ID].item.ItemHeightLevel, worldPosition.z), item.ItemPrefab.transform.rotation);

        editor.CurrentButtonPressed = item.ID;
    }
}
