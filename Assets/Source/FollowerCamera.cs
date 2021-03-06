﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerCamera : MonoBehaviour
{
    public Transform FollowObject;

    public Vector3 PositionOffset;
    public Vector3 LookPositionOffset;

    public float PositionLerpSpeed;
    public float RotationLerpSpeed;

    protected Vector3 TargetPosition => FollowObject.TransformPoint(PositionOffset);
    protected Vector3 TargetLookPosition => FollowObject.TransformPoint(LookPositionOffset);
    protected Vector3 TargetUp => FollowObject.up;

    virtual protected void FixedUpdate()
    {
        if (FollowObject)
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, PositionLerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetLookPosition - transform.position, TargetUp), RotationLerpSpeed * Time.deltaTime);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (FollowObject)
        {
            Gizmos.DrawLine(TargetPosition, TargetLookPosition);
            Gizmos.DrawSphere(TargetPosition, 0.5f);
        }
    }
}
