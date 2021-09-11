using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBodyAnimator : MonoBehaviour
{
    public Vector3 RotationMultipliers;

    public DroneBody Body;
    public Transform Transform;

    private void FixedUpdate()
    {
        Transform.localRotation = Quaternion.Euler(Body.CurrentSpeed * RotationMultipliers.x, 0f, -Body.CurrentAngularSpeed * RotationMultipliers.z);
    }
}
