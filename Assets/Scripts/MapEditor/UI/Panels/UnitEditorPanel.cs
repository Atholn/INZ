using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitEditorPanel : MonoBehaviour
{
    internal Material ActualMaterial;
    internal List<Button> ColorsUnitsButtons = new List<Button>();
    private Vector3 shiftPlaceVector = new Vector3(15, 45, 0);

    private void Start()
    {
        InitializeColorsUnitsButtons();
    }

    private void InitializeColorsUnitsButtons()
    {
        Button ButtonToCopy = gameObject.GetComponentInChildren<Button>();
        RectTransform startPosition = ButtonToCopy.GetComponent<RectTransform>();
        List<Material> materialList = MapToPlayStorage.ImportResources<Material>("Materials/Units/", ".mat");

        for (int i = 0; i < materialList.Count; i++)
        {
            Vector3 buttonPlace =
                i % 2 == 0 ?
                new Vector3(startPosition.transform.position.x + i * (startPosition.rect.width + shiftPlaceVector.x), startPosition.transform.position.y, 0) :
                new Vector3(ColorsUnitsButtons[i - 1].transform.position.x, startPosition.transform.position.y - startPosition.rect.height - shiftPlaceVector.y, 0);
            ColorsUnitsButtons.Add(Instantiate(ButtonToCopy, buttonPlace, ButtonToCopy.transform.rotation));

            ColorsUnitsButtons[i].transform.SetParent(gameObject.transform);
            ColorsUnitsButtons[i].transform.localScale = ButtonToCopy.transform.localScale;

            ItemUnitColorController unitEditorButton = ColorsUnitsButtons[i].GetComponent<ItemUnitColorController>();
            unitEditorButton.ID = i;
            unitEditorButton.unitMaterial = materialList[i];

            if (i == 0)
            {
                ActualMaterial = unitEditorButton.unitMaterial;
                GetComponent<Image>().color = ActualMaterial.color;
            }

            ColorsUnitsButtons[i].image.color = materialList[i].color;
            ColorBlock cb = ColorsUnitsButtons[i].colors;
            cb.normalColor = materialList[i].color;
            ColorsUnitsButtons[i].colors = cb;
        }

        Destroy(ButtonToCopy.gameObject);
    }
}
