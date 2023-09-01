using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignPlaneFlightHeightToTarget : MonoBehaviour
{
    public AIController Controller;
    public PlaneBody Body;
    public float Offset;

    private void FixedUpdate()
    {
        ITarget target = Controller.GetTarget();
        if (target.ExistsAndValid())
        {
            Body.FlightHeight = target.GetCenter().y - Offset;
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + Vector3.up * Offset, 0.5f);
    }
}
