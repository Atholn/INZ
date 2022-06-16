using UnityEngine;
using UnityEngine.UI;

public class ItemUnitColorController : MonoBehaviour
{
    internal Material unitMaterial;   
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
