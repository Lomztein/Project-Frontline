using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public bool HighAngle;

    private Vector3 _targetLocalPos;
    private Vector3 _targetHorizontalLocalPos;
    private float _targetLocalAngle;

    public void AimTowards(Vector3 position)
    {
        _targetLocalPos = Base.InverseTransformPoint (position);
        _targetHorizontalLocalPos = HorizontalAxis.InverseTransformPoint(position) - HorizontalAxis.InverseTransformPoint(Muzzle.position);
        _targetLocalAngle = ComputeTrajectoryAngle(_targetHorizontalLocalPos.z, _targetHorizontalLocalPos.y, ProjectileSpeed, ProjectileGravity, HighAngle);
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
        return Vector3.Distance(transform.position, target) < GetApproxProjectileRange(45f, 0f, ProjectileSpeed, ProjectileGravity);
    }

    public float DeltaAngle(Vector3 target)
    {
        if (!float.IsNaN(_targetLocalAngle))
        {
            float horAngle = Mathf.Atan2(_targetLocalPos.x, _targetLocalPos.z) * Mathf.Rad2Deg;
            float verAngle = _targetLocalAngle;
            Vector3 dir = Quaternion.Euler(-verAngle, horAngle, 0f) * Vector3.forward;
            Matrix4x4 matrix = Matrix4x4.TRS(Muzzle.position, Base.rotation, Vector3.one);
            dir = matrix.MultiplyVector(dir);
            return Vector3.Angle(dir, Muzzle.forward);
        }
        else
        {
            return 180f;
        }
    }

    private float ComputeTrajectoryAngle(float distance, float height, float speed, float gravity, bool high)
    {
        float v2 = speed * speed;
        float v4 = speed * speed * speed * speed;

        float x2 = distance * distance;
        int highSign = high ? 1 : -1;

        float num = v2 + Mathf.Sqrt(v4 - gravity * (gravity * x2 + 2f * height * v2)) * highSign;
        float dom = gravity * distance;

        float res = Mathf.Rad2Deg * Mathf.Atan(num / dom);

        return res;
    }

    public float GetApproxProjectileRange(float angle, float height, float speed, float gravity)
    {
        float rads = Mathf.Deg2Rad * angle;
        return Mathf.Pow(speed, 2f) * Mathf.Sin(2f * rads) / gravity;
    }
}
