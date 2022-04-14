using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsThruster : MonoBehaviour
{
    public Vector3 Force;
    public Vector3 LocalPosition;
    public Rigidbody Body;

    private void FixedUpdate()
    {
        Body.AddForceAtPosition(transform.rotation * Force * Time.fixedDeltaTime, LocalPosition, ForceMode.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + LocalPosition, 0.25f);
        Gizmos.DrawLine(transform.position + LocalPosition, transform.position + Force);
    }

}
