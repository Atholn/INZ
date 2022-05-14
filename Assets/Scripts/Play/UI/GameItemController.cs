using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameItemController : MonoBehaviour
{
    public Item item;

    internal bool Clicked = false;
    internal Vector3 firstScale = new Vector3();

    private GameManager gameManager;

    private void Awake()
    {
        var colors = gameObject.GetComponent<Button>().colors;
        colors.selectedColor = transform.parent.gameObject.GetComponentInParent<Image>().color;
        gameObject.GetComponent<Button>().colors = colors;

        firstScale = item.ItemPrefab.transform.localScale;
        item.ItemImage.transform.localScale = item.ItemPrefab.transform.localScale;

        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public virtual void ButtonClicked()
    {
        gameManager.DestroyItemImages();

        Vector3 screenPosition = new Vector3(Input.mousePosition.x, gameManager.ItemControllers[item.ID].item.ItemHeightLevel, Input.mousePosition.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Clicked = true;

        GameObject image = Instantiate(gameManager.ItemControllers[item.ID].item.ItemImage,
            new Vector3(worldPosition.x, gameManager.ItemControllers[item.ID].item.ItemHeightLevel, worldPosition.z),
            item.ItemPrefab.transform.rotation);

        gameManager.CurrentButtonPressed = item.ID;
        image.transform.GetComponent<MeshRenderer>().materials[1].color = gameManager._playersMaterials[0].color;
       //_playersGameObjects[i][j + 1].transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = _playersMaterials[i].color;
    }
}
