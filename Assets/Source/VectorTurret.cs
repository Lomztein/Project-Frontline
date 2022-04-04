using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorTurret : MonoBehaviour, ITurret
{
    public Transform Base;
    public Transform VectoringPlatform;

    public Vector2 HorizontalRange;
    public Vector2 VerticalRange;
    public float VectoringSpeed;
    public bool Instant;

    public Vector3 _targetLocalPosition;

    public void AimTowards(Vector3 position)
    {
        if (Instant)
        {
            VectoringPlatform.LookAt(position);
        }
        else
        {
            _targetLocalPosition = Base.InverseTransformPoint(position);
        }
    }

    private void FixedUpdate()
    {
        if (!Instant)
        {
            Vector3 angles = Turret.Clamp(Turret.CalculateAngleTowards(_targetLocalPosition), HorizontalRange, VerticalRange);
            VectoringPlatform.localRotation = Quaternion.RotateTowards(VectoringPlatform.transform.localRotation, Quaternion.Euler(angles.x, angles.y, 0f), VectoringSpeed * Time.fixedDeltaTime);
        }
    }

    public bool CanHit(Vector3 target)
    {
        Vector3 localPosition = Base.InverseTransformPoint(target);
        Vector3 angles = Turret.CalculateAngleTowards(localPosition);

        return HorizontalRange.x < angles.y && angles.y < HorizontalRange.y
            && VerticalRange.x < angles.x && angles.x < VerticalRange.y;
    }

    public float DeltaAngle(Vector3 target)
    {
        Vector3 localPosition = VectoringPlatform.InverseTransformPoint(target);
        float angle = Vector3.Angle(Vector3.forward, localPosition);
        return angle;
    }
}
