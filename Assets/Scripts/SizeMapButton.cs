using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeMapButton : MonoBehaviour
{
    public int size;
    public GameObject basicTerrain;
    public LevelEditorManager editor;
    

    public void ChoiceMapSize()
    {
        GameObject panel = GameObject.FindGameObjectWithTag("SizePanel");
        panel.SetActive(false);
        editor.CreateStartTerrain(size, basicTerrain);
    }
}
