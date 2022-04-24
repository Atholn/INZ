using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorCameraControll : MonoBehaviour
{
    public float Speed;
    static MapEditorCameraControll cameraControl;

    new Camera camera;
    Vector2 keyboardInput;

    private void Awake()
    {
        cameraControl = this;
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        keyboardInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Vector2 movementDirection = keyboardInput;

        var delta = new Vector3(movementDirection.x, 0, movementDirection.y);
        delta *= Speed * Time.deltaTime;
        transform.localPosition += delta;
    }
}
