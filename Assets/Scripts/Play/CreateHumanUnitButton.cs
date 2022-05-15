using UnityEngine;
using UnityEngine.UI;

public class CreateHumanUnitButton : MonoBehaviour
{
    public GameObject Unit;

    private BuildingUnit buildingParent;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        GetComponent<RawImage>().texture = Unit.GetComponent<Unit>().Profile;
    }

    public void Create()
    {
        if(gameManager._players[0].actualUnitsPoint + Unit.GetComponent<HumanUnit>().UnitPoint <= gameManager._players[0].actualMaxUnitsPoint &&
            gameManager._players[0].actualGold >= Unit.GetComponent<Unit>().GoldCost &&
            gameManager._players[0].actualWood >= Unit.GetComponent<Unit>().WoodCost)
        {
            gameManager.UpdateWood(0, -Unit.GetComponent<Unit>().WoodCost);
            gameManager.UpdateGold(0, -Unit.GetComponent<Unit>().GoldCost);

            buildingParent = gameManager.actualClickBuild;

            buildingParent.CreateUnit(Unit, 0);
        }
    }

    public void ShowHints()
    {
        string[] texts = new string[3];
        texts[0] = Unit.GetComponent<HumanUnit>().GoldCost.ToString();
        texts[1] = Unit.GetComponent<HumanUnit>().WoodCost.ToString();
        texts[2] = Unit.GetComponent<HumanUnit>().UnitPoint.ToString();

        gameManager.ShowHints(texts);
    }
}
