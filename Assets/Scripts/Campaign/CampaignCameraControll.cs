using UnityEngine;

public class CampaignCameraControll : MonoBehaviour
{
    private Vector3 shiftVector = new Vector3(0, 0, -2);
    private Vector3 actualPosTarget;

    private void Awake()
    {
        actualPosTarget = transform.position;
    }

    private void Update()
    {
        if(transform.position != actualPosTarget)
        {
            transform.position = Vector3.Lerp(transform.position, actualPosTarget, 2.0f * Time.deltaTime);
        }
    }

    internal void GoToCampaignPoint(Vector2 newTargetPos)
    {
        actualPosTarget = new Vector3(newTargetPos.x, transform.position.y, newTargetPos.y) + shiftVector;
    }
}
