using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitEditorPanel : MonoBehaviour
{
    public UnitEditorButton[] UnitButtons;
    public Material actualMaterial;

    private void Start()
    {
        GetComponent<Image>().color = UnitButtons[0].buttonColor;
        actualMaterial = UnitButtons[0].unitMaterial;

        for (int i = 0; i < UnitButtons.Length; i++)
        {
            UnitButtons[i].ID = i;
        }
    }
}
