using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCursorMovement : MonoBehaviour
{
    public Camera MainCamera;
    public Vector3 ShiftVector;

    void Update()
    {
        gameObject.transform.localPosition = MainCamera.transform.localPosition + ShiftVector;
    }
}
