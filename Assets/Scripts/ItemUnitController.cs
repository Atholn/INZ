using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUnitController : ItemController
{
    UnitEditorPanel unitEditorPanel;

    private void Start()
    {
        unitEditorPanel = FindObjectOfType<UnitEditorPanel>();
    }

    public override void ButtonClicked()
    {
        item.ItemImage.GetComponent<MeshRenderer>().material = unitEditorPanel.actualMaterial;
        item.ItemPrefab.GetComponent<MeshRenderer>().material = unitEditorPanel.actualMaterial;
        base.ButtonClicked();
    }
}
