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

    public Vector3 _targetLocalPosition;

    public void AimTowards(Vector3 position)
    {
        _targetLocalPosition = Base.InverseTransformPoint(position);
    }

    private void FixedUpdate()
    {
        float x = Mathf.Clamp(Mathf.Atan2(_targetLocalPosition.x, _targetLocalPosition.z) * Mathf.Rad2Deg, HorizontalRange.x, HorizontalRange.y);
        float y = Mathf.Clamp(Mathf.Atan2(_targetLocalPosition.y, _targetLocalPosition.z) * Mathf.Rad2Deg, VerticalRange.x, VerticalRange.y);

        VectoringPlatform.localRotation = Quaternion.RotateTowards(VectoringPlatform.transform.localRotation, Quaternion.Euler(-y, x, 0f), VectoringSpeed * Time.fixedDeltaTime);
    }

    public bool CanHit(Vector3 target)
    {
        Vector3 localPosition = Base.InverseTransformPoint(target);

        float x = Mathf.Atan2(localPosition.x, localPosition.z) * Mathf.Rad2Deg;
        float y = Mathf.Atan2(localPosition.y, localPosition.z) * Mathf.Rad2Deg;

        return HorizontalRange.x < x && x < HorizontalRange.y
            && VerticalRange.x < y && y < VerticalRange.y;
    }

    public float DeltaAngle(Vector3 target)
    {
        Vector3 localPosition = VectoringPlatform.InverseTransformPoint(target);
        float angle = Vector3.Angle(Vector3.forward, localPosition);
        return angle;
    }
}
