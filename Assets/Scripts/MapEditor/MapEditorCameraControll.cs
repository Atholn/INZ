using UnityEngine;

public class MapEditorCameraControll : MonoBehaviour
{
    public float Speed;

    internal float mapSizeX, mapSizeY;

    private float minPosX, minPosY;

    private void Awake()
    {
        minPosX = 9.5f;
        minPosY = -9.5f;
    }

    internal void SetMapLimits(float x, float y)
    {
        mapSizeX = x;
        mapSizeY = y;
    }

    void Update()
    {
        //movement direction
        Vector2 md = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // delta movement
        var dm = new Vector3(md.x, 0, md.y);
        dm *= Speed * Time.deltaTime;

        if (transform.localPosition.x + dm.x >= minPosX &&
            transform.position.x + dm.x <= mapSizeX &&
            transform.position.z + dm.z >= minPosY &&
            transform.position.z + dm.z <= mapSizeY)
            transform.localPosition += dm;
    }
}
