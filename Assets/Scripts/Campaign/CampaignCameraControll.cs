using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignCameraControll : MonoBehaviour
{
    private Vector3 shiftVector = new Vector3(0, 0, -4);

    internal void GoToCampaignPoint(Vector2 newTargetPos)
    {
        transform.position = new Vector3(newTargetPos.x, transform.position.y, newTargetPos.y) + shiftVector;
    }
}
