using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUnitController : ItemController
{
    UnitEditorPanel unitEditorPanel;
    //public StartPointUnit startPointUnit;

    private void Start()
    {
        unitEditorPanel = FindObjectOfType<UnitEditorPanel>();
    }

    public override void ButtonClicked()
    {
        ItemImage.GetComponent<MeshRenderer>().material = unitEditorPanel.actualMaterial;
        ItemPrefab.GetComponent<MeshRenderer>().material = unitEditorPanel.actualMaterial;
        base.ButtonClicked();
    }
}
