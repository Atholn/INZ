using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemController : MonoBehaviour
{
    public Item item;

    internal bool Clicked = false;
    internal Vector3 firstScale = new Vector3();

    private MapEditorManager _editor;
    private GameManager _gameManager;

    private void Awake()
    {
        var colors = gameObject.GetComponent<Button>().colors;
        colors.selectedColor = transform.parent.gameObject.GetComponentInParent<Image>().color;
        gameObject.GetComponent<Button>().colors = colors;

        firstScale = item.ItemPrefab.transform.localScale;
        item.ItemImage.transform.localScale = item.ItemPrefab.transform.localScale;

        if (GameObject.FindGameObjectWithTag("LevelEditorManager") != null)
        {
            _editor = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<MapEditorManager>();
        }
        else
        {
            _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
    }

    public virtual void ButtonClicked()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, _editor != null ? _editor.ItemControllers[item.ID].item.ItemHeightLevel : _gameManager.ItemControllers[item.ID].item.ItemHeightLevel, Input.mousePosition.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Clicked = true;

        Instantiate(_editor != null ? _editor.ItemControllers[item.ID].item.ItemImage : _gameManager.ItemControllers[item.ID].item.ItemImage,
            new Vector3(worldPosition.x, _editor != null ? _editor.ItemControllers[item.ID].item.ItemHeightLevel : _gameManager.ItemControllers[item.ID].item.ItemHeightLevel, worldPosition.z),
            item.ItemPrefab.transform.rotation);

        if (_editor != null)
        {
            _editor.CurrentButtonPressed = item.ID;
        }
        else
        {
            _gameManager.CurrentButtonPressed = item.ID;
        }
    }
}
