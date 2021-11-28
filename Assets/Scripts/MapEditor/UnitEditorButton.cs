using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitEditorButton : MonoBehaviour
{
    public Material unitMaterial;   
    internal int ID;
    private UnitEditorPanel _unitEditorPanel;  

    private void Start()
    {
        _unitEditorPanel = GetComponentInParent<UnitEditorPanel>();
    }

    public void ChangePanelColor() 
    {
        _unitEditorPanel.GetComponent<Image>().color = unitMaterial.color;
        _unitEditorPanel.ActualMaterial = unitMaterial;
    }
}
