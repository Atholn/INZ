using UnityEngine;

public class ItemDeleteController : MonoBehaviour
{
    public GameObject ItemImage;

    internal bool Pressed = false;

    public virtual void ButtonClicked()
    {
        Pressed = true;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 1f, Input.mousePosition.z));

        Instantiate(ItemImage, new Vector3(worldPosition.x, 1f, worldPosition.z), ItemImage.transform.rotation);
    }
}
