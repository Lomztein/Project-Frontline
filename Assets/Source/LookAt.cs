using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;

    private void FixedUpdate()
    {
        transform.LookAt(Target.TransformPoint(Offset));
    }

    private void OnDrawGizmos()
    {
        if (Target != null)
        {
            Gizmos.DrawWireSphere(Target.TransformPoint(Offset), 0.15f);
        }
    }
}
