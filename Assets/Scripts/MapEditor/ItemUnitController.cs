using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUnitController : ItemController
{
    UnitEditorPanel unitEditorPanel;

    private void Start()
    {
        unitEditorPanel = FindObjectOfType<UnitEditorPanel>();
    }

    public override void ButtonClicked()
    {
        item.ItemImage.GetComponent<MeshRenderer>().material = unitEditorPanel.ActualMaterial;
        item.ItemPrefab.GetComponent<MeshRenderer>().material = unitEditorPanel.ActualMaterial;

        var colors = gameObject.GetComponent<Button>().colors;
        colors.selectedColor = transform.parent.gameObject.GetComponentInParent<Image>().color;
        gameObject.GetComponent<Button>().colors = colors;

        base.ButtonClicked();
    }
}
