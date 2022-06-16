using UnityEngine;
using UnityEngine.UI;

public class UpgradeUnit : MonoBehaviour
{
    public int GoldCost;
    public int WoodCost;
    public GameObject Unit;

    internal int NumberOfUpgrade;

    private BuildingUnit buildingParent;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        GetComponent<RawImage>().texture = Unit.GetComponent<Unit>().Profile;
    }

    public void ClickUpdate()
    {
        if (gameManager._players[0].actualGold >= GoldCost &&
            gameManager._players[0].actualWood >= WoodCost)
        {
            //todo update player who play in play in network
            gameManager.UpdateWood(0, GoldCost);
            gameManager.UpdateGold(0, WoodCost);

            gameManager.UpgradeUnit(0, NumberOfUpgrade);
            gameObject.SetActive(false);
            return;
        }

        bool[] errors = new bool[2];
        errors[0] = gameManager._players[0].actualGold < GoldCost;
        errors[1] = gameManager._players[0].actualWood < WoodCost;

        gameManager.ShowErrors(errors);
    }

    public void ShowHints()
    {
        string[] texts = new string[2];
        texts[0] = GoldCost.ToString();
        texts[1] = WoodCost.ToString();

        gameManager.ShowHints(texts);
    }
}
