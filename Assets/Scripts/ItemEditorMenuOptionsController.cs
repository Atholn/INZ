using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEditorMenuOptionsController : MonoBehaviour
{
    private GameObject panel;

    private void Start()
    {
        panel = GameObject.FindGameObjectWithTag("OptionsEditorPanel");
        panel.SetActive(false);
    }

    public void ClickOptionButton()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
