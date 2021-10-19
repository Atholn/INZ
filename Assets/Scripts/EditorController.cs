using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorController : MonoBehaviour
{
    [SerializeField]
    GameObject firstPanel, secondPanel;

    public void SetActivePanel()
    {
        firstPanel.SetActive(false);
        secondPanel.SetActive(true);
    }
}
