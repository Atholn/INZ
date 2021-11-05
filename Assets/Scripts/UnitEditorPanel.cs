using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitEditorPanel : MonoBehaviour
{
    public UnitEditorButton[] UnitButtons;
    public int CurrentUnitButtonPressed = 0;

    private void Start()
    {
        GetComponent<Image>().color = UnitButtons[CurrentUnitButtonPressed].buttonColor;
    }

    private void Update()
    {
    }
}
