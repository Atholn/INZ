using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public Item item;

    internal bool Clicked = false;
    internal Vector3 firstScale = new Vector3();

    private MapEditorManager editor;

    private void Awake()
    {
        var colors = gameObject.GetComponent<Button>().colors;
        colors.selectedColor = transform.parent.gameObject.GetComponentInParent<Image>().color;
        gameObject.GetComponent<Button>().colors = colors;

        firstScale = item.ItemPrefab.transform.localScale;
        item.ItemImage.transform.localScale = item.ItemPrefab.transform.localScale;

        editor = GameObject.FindObjectOfType<MapEditorManager>();
    }

    public virtual void ButtonClicked()
    {
        editor.DestroyItemImages();

        Vector3 screenPosition = new Vector3(Input.mousePosition.x, editor.ItemControllers[item.ID].item.ItemHeightLevel, Input.mousePosition.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Clicked = true;
 
        editor.image = Instantiate(editor.ItemControllers[item.ID].item.ItemImage,
                                   new Vector3(worldPosition.x,
                                               editor.ItemControllers[item.ID].item.ItemHeightLevel,
                                               worldPosition.z),
                                   item.ItemPrefab.transform.rotation);

        editor.CurrentButtonPressed = item.ID;
    }
}
