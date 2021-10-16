using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawItem : MonoBehaviour
{

    public int ID;
    private LevelEditorManager editor;

    void Start()
    {
        editor = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<LevelEditorManager>();
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Destroy(this.gameObject);
        }
    }
}
