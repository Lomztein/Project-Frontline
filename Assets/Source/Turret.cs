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
    
    public void Target (Vector3 position, float deltaTime)
    {
        RotateHorizontal(position, deltaTime);
        RotateVertical(position, deltaTime);
    }

    private void RotateHorizontal (Vector3 position, float deltaTime)
    {
        Vector3 localPosition = Base.InverseTransformPoint(position);
        float angle = Mathf.Clamp (Mathf.Atan2(localPosition.x, localPosition.z) * Mathf.Rad2Deg, HorizontalRange.x, HorizontalRange.y);
        HorizontalAxis.localRotation = Quaternion.RotateTowards(HorizontalAxis.localRotation, Quaternion.Euler(0f, angle, 0f), HorizontalSpeed * deltaTime);
    }

    private void RotateVertical (Vector3 position, float deltaTime)
    {
        Vector3 localPosition = HorizontalAxis.InverseTransformPoint(position - new Vector3 (0f, VerticalAxis.localPosition.y));
        float angle = Mathf.Clamp(-Mathf.Atan2(localPosition.y, localPosition.z) * Mathf.Rad2Deg, VerticalRange.x, VerticalRange.y);
        Debug.Log(angle);
        VerticalAxis.localRotation = Quaternion.RotateTowards(VerticalAxis.localRotation, Quaternion.Euler(angle, 0f, 0f), VerticalSpeed * deltaTime);
    }
}
