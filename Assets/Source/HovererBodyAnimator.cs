using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovererBodyAnimator : MonoBehaviour
{
    public HovererBody Body;
    public float RotationMultiplier;
    public Transform BaseTransform;
    public Transform BodyTransform;
    public float MaxDelta = 30f;

    private void FixedUpdate()
    {
        Vector3 velFactor = BaseTransform.InverseTransformVector(Body.Velocity);
        BodyTransform.localRotation =
            Quaternion.Lerp(BodyTransform.localRotation, Quaternion.Euler((velFactor.z) * RotationMultiplier, 0f, 0f), MaxDelta * Time.fixedDeltaTime);
    }
}
