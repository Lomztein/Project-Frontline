using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : FollowerCamera
{
    public float Sensitivity;
    public Vector3 _lookEuler;

    private void Update()
    {
        Vector3 input = new Vector3(
            Input.GetAxis("Mouse Y"),
            Input.GetAxis("Mouse X")
        );

        _lookEuler += input * Sensitivity;
    }

    protected override void FixedUpdate()
    {
        if (FollowObject)
        {
            Quaternion rot = Quaternion.Euler(_lookEuler);

            transform.position = rot * Vector3.Lerp(transform.position, TargetPosition, PositionLerpSpeed * Time.deltaTime);
            transform.rotation = rot * Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetLookPosition - transform.position, TargetUp), RotationLerpSpeed * Time.deltaTime);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawRay(transform.position, Quaternion.Euler(_lookEuler) * transform.forward * 5);
    }
}
