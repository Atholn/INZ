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

        Unit unit = item.GetComponent<Unit>();
        if (gameManager._players[0].actualGold < unit.GoldCost ||
            gameManager._players[0].actualWood < unit.WoodCost)
        {
            bool[] errors = new bool[2];
            errors[0] = gameManager._players[0].actualGold < unit.GoldCost;
            errors[1] = gameManager._players[0].actualWood < unit.WoodCost;

            gameManager.ShowErrors(errors);
            return;
        }

        gameManager.building = true;

        Vector3 screenPosition = new Vector3(Input.mousePosition.x, gameManager.GameItemControllers[item.ID].item.ItemHeightLevel, Input.mousePosition.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Clicked = true;

        GameObject image = Instantiate(gameManager.GameItemControllers[item.ID].item.ItemImage,
            new Vector3(worldPosition.x, gameManager.GameItemControllers[item.ID].item.ItemHeightLevel, worldPosition.z),
            item.ItemPrefab.transform.rotation);

        gameManager.CurrentButtonPressed = item.ID;
        image.transform.GetComponent<MeshRenderer>().materials[1].color = gameManager._playersMaterials[0].color;
    }

    public void ShowHints()
    {
        string[] texts = new string[2];
        texts[0] = item.GetComponent<BuildingUnit>().GoldCost.ToString();
        texts[1] = item.GetComponent<BuildingUnit>().WoodCost.ToString();

        gameManager.ShowHints(texts);
    }
}
