using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private float torque;

    private Rigidbody rigidbody;
    private float timeStopMax = 3f;
    private float timeStop = 0f;
    private bool ifStop = false;
    private int attackPower = 0;
    private int player;

    private void Update()
    {
        if (ifStop)
        {
            timeStop += Time.deltaTime;
            if (timeStop >= timeStopMax)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(ifStop)
        {
            return;
        }

        rigidbody.velocity = Vector3.zero;
        rigidbody.freezeRotation = true;
        transform.SetParent(collision.transform);
        transform.position += new Vector3(0, -1, 0);
        ifStop = true;

        Unit unit = collision.transform.GetComponent<Unit>();
        if(unit != null && unit.WhichPlayer != player)
        {
            unit.Hp -= attackPower;
            if(unit is BuildingUnit)
            {
                (unit as BuildingUnit).UpdateFire();
            }
        }
    }

    internal void SetLandingPlace(Vector3 vector3, int attackPowerUnit, int playerNumber)
    {
        attackPower = attackPowerUnit;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce((vector3 - transform.position) * 15);
        rigidbody.AddTorque(transform.right * torque);
        player = playerNumber;
    }
}
