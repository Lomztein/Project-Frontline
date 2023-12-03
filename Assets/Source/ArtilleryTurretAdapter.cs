using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryTurretAdapter : MonoBehaviour, ITurret
{
    public GameObject Turret;
    private ITurret _turret;
    public float ProjectileSpeed;
    public float ProjectileGravity;
    public bool HighAngle;

    public Transform Base;
    public Transform Muzzle;

    private Vector3 _targetLocalPos;
    private float _targetLocalAngle;

    private void Awake()
    {
        _turret = Turret.GetComponent<ITurret>();
    }

    public void AimTowards(Vector3 position)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(Muzzle.position, Base.rotation, Vector3.one).inverse;

        _targetLocalPos = matrix.MultiplyPoint(position);
        _targetLocalAngle = ComputeTrajectoryAngle(_targetLocalPos.z, _targetLocalPos.y, ProjectileSpeed, ProjectileGravity, HighAngle);
    }

    private void FixedUpdate()
    {
        if (!float.IsNaN(_targetLocalAngle))
        {
            float horAngle = Mathf.Atan2(_targetLocalPos.x, _targetLocalPos.z) * Mathf.Rad2Deg;
            float verAngle = _targetLocalAngle;

            Vector3 dir = Quaternion.Euler(-verAngle, horAngle, 0f) * Vector3.forward;

            Matrix4x4 matrix = Matrix4x4.TRS(Muzzle.position, Base.rotation, Vector3.one);
            Vector3 point = matrix.MultiplyPoint(dir * 10);

            _turret.AimTowards(point);

            DebugDrawTrajectory(Muzzle.position, matrix.MultiplyVector(dir), ProjectileSpeed, ProjectileGravity);
        }
    }

    private void DebugDrawTrajectory(Vector3 start, Vector3 dir, float speed, float gravity)
    {
        int iters = 50000;
        float distPerIter = 0.1f;
        float timePerIter = distPerIter / speed;

        Vector3 pos = start;
        Vector3 vel = dir * speed;
        Vector3 prevPos = pos;

        while(true && iters-- > 0)
        {
            vel += gravity * timePerIter * Vector3.down;
            pos += vel * timePerIter;
            Debug.DrawLine(pos, prevPos);
            if (pos.y < 0f)
                break;
            prevPos = pos;
        }
    }

    public bool CanHit(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) < GetApproxProjectileRange(45f, 0f, ProjectileSpeed, ProjectileGravity);
    }

    public float DeltaAngle(Vector3 target)
    {
        float horAngle = Mathf.Atan2(_targetLocalPos.x, _targetLocalPos.z) * Mathf.Rad2Deg;
        float verAngle = _targetLocalAngle;
        if (!float.IsNaN(verAngle))
        {
            Vector3 dir = Quaternion.Euler(-verAngle, horAngle, 0f) * Vector3.forward;
            Matrix4x4 matrix = Matrix4x4.TRS(Muzzle.position, Base.rotation, Vector3.one);
            dir = matrix.MultiplyVector(dir);
            return Vector3.Angle(dir, Muzzle.forward);
        }
        return 180;
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
