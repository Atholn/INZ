using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    static CameraControll cameraControl;

    public float cameraSpeed, zoomSpeed, groundHeight;
    public Vector2 cameraHeightMinMax;
    public Vector2 cameraRotationMinMax;
    
    new Camera camera;
    Vector2 mousePos, keyboardInput;

    private void Awake()
    {
        cameraControl = this;    
        camera = GetComponent<Camera>();
        
    }

    void Update()
    {
        keyboardInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Vector2 movementDirection = keyboardInput;

        var deltaPosition = new Vector3(movementDirection.x, 0, movementDirection.y);
        deltaPosition *= cameraSpeed * Time.deltaTime;
        transform.localPosition += deltaPosition;
    }
}
