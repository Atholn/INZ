using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class EditorStartPoint
{
    internal Vector3 unitStartLocation;
    internal string unitMaterialName;

    public float[] uSL 
    {
        get 
        {
            return new float[3] { unitStartLocation.x, unitStartLocation.y, unitStartLocation.z }; 
        }
        set
        {
            unitStartLocation.x = uSL[0];
            unitStartLocation.y = uSL[1];
            unitStartLocation.z = uSL[2];
        }
    }
}
