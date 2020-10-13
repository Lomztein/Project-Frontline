using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
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
    
    public float AimTowards (Vector3 position)
    {
        _baseTargetLocalPosition = Base.InverseTransformPoint(position);
        _horizontalTargetLocalPosition = HorizontalAxis.InverseTransformPoint(position - new Vector3(0f, VerticalAxis.localPosition.y));

        return DeltaAngle(position);
    }

    private void FixedUpdate()
    {
        RotateHorizontal(_baseTargetLocalPosition, Time.fixedDeltaTime);
        RotateVertical(_horizontalTargetLocalPosition, Time.fixedDeltaTime);
    }

    private void RotateHorizontal (Vector3 localPosition, float deltaTime)
    {
        float angle = Mathf.Clamp (Mathf.Atan2(localPosition.x, localPosition.z) * Mathf.Rad2Deg, HorizontalRange.x, HorizontalRange.y);
        HorizontalAxis.localRotation = Quaternion.RotateTowards(HorizontalAxis.localRotation, Quaternion.Euler(0f, angle, 0f), HorizontalSpeed * deltaTime);
    }

    private void RotateVertical (Vector3 localPosition, float deltaTime)
    {
        float angle = Mathf.Clamp(-Mathf.Atan2(localPosition.y, localPosition.z) * Mathf.Rad2Deg, VerticalRange.x, VerticalRange.y);
        VerticalAxis.localRotation = Quaternion.RotateTowards(VerticalAxis.localRotation, Quaternion.Euler(angle, 0f, 0f), VerticalSpeed * deltaTime);
    }

    public float DeltaAngle (Vector3 target)
    {
        Vector3 localPosition = VerticalAxis.InverseTransformPoint(target);
        float angle = Vector3.Angle(Vector3.forward, localPosition);
        return angle;
    }

    public bool CanHit(Vector3 target)
    {
        return true;
    }
}
