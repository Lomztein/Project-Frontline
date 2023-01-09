using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour, ITurret
{
    public Transform Base;
    public Transform HorizontalAxis;
    public Transform VerticalAxis;

    public Vector2 HorizontalRange = new Vector2(-180f, 180f);
    public Vector2 VerticalRange = new Vector2(-180f, 180f);

    public float HorizontalSpeed;
    public float VerticalSpeed;

    private Vector3 _baseTargetLocalPosition;
    private Vector3 _horizontalTargetLocalPosition;
    public bool Instant;

    public void AimTowards(Vector3 position)
    {
        if (HorizontalAxis)
        {
            _baseTargetLocalPosition = Base.InverseTransformPoint(position - HorizontalAxis.localPosition);
            _horizontalTargetLocalPosition = HorizontalAxis.InverseTransformPoint(position) - VerticalAxis.localPosition;
        }
        else
        {
            _horizontalTargetLocalPosition = Base.InverseTransformPoint(position) - VerticalAxis.localPosition;
        }
    }

    private void FixedUpdate()
    {
        if (HorizontalAxis)
        {
            RotateHorizontal(_baseTargetLocalPosition, Time.fixedDeltaTime);
        }
        if (VerticalAxis)
        {
            RotateVertical(_horizontalTargetLocalPosition, Time.fixedDeltaTime);
        }
    }

    private void RotateHorizontal(Vector3 localPosition, float deltaTime)
    {
        Vector3 baseAngles = Clamp(CalculateAngleTowards(localPosition), HorizontalRange, VerticalRange);
        float angle = baseAngles.y;

        if (Instant)
        {
            HorizontalAxis.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            HorizontalAxis.localRotation = Quaternion.RotateTowards(HorizontalAxis.localRotation, Quaternion.Euler(0f, angle, 0f), HorizontalSpeed * deltaTime);
        }
    }

    private void RotateVertical(Vector3 localPosition, float deltaTime)
    {
        Vector3 baseAngles = Clamp(CalculateAngleTowards(localPosition), HorizontalRange, VerticalRange);
        float angle = baseAngles.x;

        if (Instant)
        {
            VerticalAxis.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }
        else
        {
            VerticalAxis.localRotation = Quaternion.RotateTowards(VerticalAxis.localRotation, Quaternion.Euler(angle, 0f, 0f), VerticalSpeed * deltaTime);
        }
    }

    public float DeltaAngle(Vector3 target)
    {
        Vector3 localPosition = VerticalAxis.InverseTransformPoint(target);
        float angle = Vector3.Angle(Vector3.forward, localPosition);
        return angle;
    }

    public bool CanHit(Vector3 target)
    {
        Vector3 localPosition = Base.InverseTransformPoint(target);
        Vector3 angles = Turret.CalculateAngleTowards(localPosition);

        return HorizontalRange.x < angles.y && angles.y < HorizontalRange.y
            && VerticalRange.x < angles.x && angles.x < VerticalRange.y;
    }

    public static Vector2 CalculateAngleTowards (Vector3 localPosition)
    {
        Vector3 angles = Quaternion.FromToRotation(Vector3.forward, localPosition).eulerAngles;

        float horizontal = angles.y;
        float vertical = angles.x;

        if (localPosition.y > 0f)
            vertical = (360f - angles.x) * -1;

        if (vertical < -359f)
            vertical = 0f; // idk i think angles just hate me

        if (localPosition.x < 0f)
            horizontal = (360f - angles.y) * -1;

        if (horizontal < -359f)
            horizontal = 0f;

        return new Vector2(vertical, horizontal);
    }

    public static Vector2 Clamp(Vector2 angles, Vector2 horMinMax, Vector2 verMinMax)
    {
        return new Vector2(Mathf.Clamp(angles.x, verMinMax.x, verMinMax.y), Mathf.Clamp(angles.y, horMinMax.x, horMinMax.y));
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 targetPos = HorizontalAxis.TransformPoint(_horizontalTargetLocalPosition + VerticalAxis.localPosition);
        Gizmos.DrawSphere(targetPos, 0.5f);
        Gizmos.DrawLine(VerticalAxis.position, targetPos);
    }
}
