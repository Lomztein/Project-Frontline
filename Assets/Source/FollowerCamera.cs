using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerCamera : MonoBehaviour
{
    public Transform FollowObject;

    public Vector3 PositionOffset;
    public Vector3 LookPositionOffset;

    public float PositionLerpSpeed;
    public float RotationLerpSpeed;

    private Vector3 TargetPosition => FollowObject.TransformPoint(PositionOffset);
    private Vector3 TargetLookPosition => FollowObject.TransformPoint(LookPositionOffset);
    private Vector3 TargetUp => FollowObject.up;

    void FixedUpdate()
    {
        if (FollowObject)
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, PositionLerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetLookPosition - transform.position, TargetUp), RotationLerpSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if (FollowObject)
        {
            Gizmos.DrawLine(TargetPosition, TargetLookPosition);
            Gizmos.DrawSphere(TargetPosition, 0.5f);
        }
    }
}
