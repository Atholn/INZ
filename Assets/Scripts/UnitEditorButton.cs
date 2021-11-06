using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitEditorButton : MonoBehaviour
{
    public int ID;
    public Button button;
    public Color buttonColor;
    public Material unitMaterial;
    UnitEditorPanel unitEditorPanel;  

    private void Start()
    {
        unitEditorPanel = GetComponentInParent<UnitEditorPanel>();
    }

    public void ChangePanelColor() 
    {
        unitEditorPanel.GetComponent<Image>().color = buttonColor;
        unitEditorPanel.actualMaterial = unitMaterial;
    }
}
