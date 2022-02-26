using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtillaryTurret : MonoBehaviour, ITurret
{
    public Transform Base;
    public Transform HorizontalAxis;
    public Transform VerticalAxis;
    public Transform Muzzle;

    public float HorRotationSpeed;
    public float VerRotationSpeed;

    public float ProjectileSpeed;
    public float ProjectileGravity;

    private Vector3 _targetLocalPos;
    private Vector3 _targetHorizontalLocalPos;
    private float _targetLocalAngle;

    public void AimTowards(Vector3 position)
    {
        _targetLocalPos = Base.InverseTransformPoint (position);
        _targetHorizontalLocalPos = HorizontalAxis.InverseTransformPoint (position) - VerticalAxis.localPosition;
        _targetLocalAngle = ComputeTrajectoryAngle(_targetHorizontalLocalPos.z, _targetHorizontalLocalPos.y, ProjectileSpeed, ProjectileGravity);
    }

    private void FixedUpdate()
    {
        if (!float.IsNaN(_targetLocalAngle))
        {
            float horAngle = Mathf.Atan2(_targetLocalPos.x, _targetLocalPos.z) * Mathf.Rad2Deg;
            HorizontalAxis.localRotation = Quaternion.RotateTowards(HorizontalAxis.localRotation, Quaternion.Euler(0f, horAngle, 0f), HorRotationSpeed * Time.fixedDeltaTime);
            VerticalAxis.localRotation = Quaternion.RotateTowards(VerticalAxis.localRotation, Quaternion.Euler(-_targetLocalAngle, 0f, 0f), VerRotationSpeed * Time.fixedDeltaTime);
        }
    }

    public bool CanHit(Vector3 target)
    {
        return true;
    }

    public float DeltaAngle(Vector3 target)
    {
        return 0f;
    }

    private float ComputeTrajectoryAngle(float distance, float height, float speed, float gravity)
    {
        float v2 = speed * speed;
        float v4 = speed * speed * speed * speed;

        float x2 = distance * distance;

        float num = v2 - Mathf.Sqrt(v4 - gravity * (gravity * x2 + 2f * height * v2));
        float dom = gravity * distance;

        float res = Mathf.Rad2Deg * Mathf.Atan(num / dom);

        return res;
    }
}
