using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemController : MonoBehaviour
{
    public int ID;
    public bool Clicked = false;
    private LevelEditorManager editor;

    private void Start()
    {
        editor = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<LevelEditorManager>();
    }

    public void ButtonClicked()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Clicked = true;

        Instantiate(editor.ItemImage[ID], new Vector3(worldPosition.x, editor.ItemHeightLevel[ID], worldPosition.y), Quaternion.identity);

        editor.CurrentButtonPressed = ID;

    }
}
