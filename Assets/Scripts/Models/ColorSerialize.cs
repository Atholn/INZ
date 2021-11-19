using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorSerialize
{
    private Color color;
    public float[] colorSerialized
    {
        get
        {
            return new float[3] { color.r, color.g, color.b };
        }
        set
        {
            color.r = colorSerialized[0];
            color.g = colorSerialized[1];
            color.b = colorSerialized[2];
        }
    }
}
