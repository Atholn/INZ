using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationTextScript : MonoBehaviour
{
    public LevelEditorManager levelEditorManager;
    public Text locationText;

    private void Update()
    {
        locationText.text = (levelEditorManager.v.x - levelEditorManager.v.x % 1).ToString() + " x " + (levelEditorManager.v.z - levelEditorManager.v.z % 1).ToString() + " y";
    }
}
