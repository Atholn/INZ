using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPointUnit:MonoBehaviour
{
    internal Material unitMaterial;
    public int buildScale;
    public int buildSize;

    private void Start()
    {
        gameObject.transform.localScale = new Vector3(buildScale, buildScale, buildScale);
    }
}
