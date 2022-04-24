using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorCameraControll : MonoBehaviour
{
    public float Speed;

    void Update()
    {
        //movement direction
        Vector2 md = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // delta movement
        var dm = new Vector3(md.x, 0, md.y); 
        dm *= Speed * Time.deltaTime;
        transform.localPosition += dm;
    }
}
