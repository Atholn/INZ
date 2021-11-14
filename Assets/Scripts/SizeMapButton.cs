using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeMapButton : MonoBehaviour
{
    public int size;
    public LevelEditorManager editor;

    public void ChoiceMapSize()
    {
        editor.InitializeStartTerrain(size);
    }
}
