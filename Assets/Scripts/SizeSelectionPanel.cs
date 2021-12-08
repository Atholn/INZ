using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SizeSelectionPanel : MonoBehaviour
{
    public Text SizeMapText;
    public Slider[] Sliders;

    private int[] _sizes = new int[2];
    private int _groundId = 0;
    private void Start()
    {
        _sizes[0] = (int)Sliders[0].minValue;
        _sizes[1] = (int)Sliders[1].minValue;
    }

    private void Update()
    {
        SizeMapText.text = $"{_sizes[0]} x {_sizes[1]}";
    }

    public void SetSizeButtons(int size)
    {
        for (int i = 0; i < Sliders.Length; i++)
        {
            _sizes[i] = size;
        }
    }

    public void SetSizeSliders()
    {
        for (int i = 0; i < Sliders.Length; i++)
        {
            _sizes[i] = (int)Sliders[i].value;
        }
    }

    public void SetMainGround(int id)
    {
        _groundId = id;
    }
}
