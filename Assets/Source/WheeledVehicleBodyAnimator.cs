using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheeledVehicleBodyAnimator : MonoBehaviour
{
    public VehicleBody Body;
    public Animator Animator;
    public float WheelRadius;
    public float WheelAngleMultiplier;

    public float MaxAccelerationDeltaVelocity;
    public float MaxDeltaAngle;

    public Transform[] TurnWheels;
    public Transform[] Wheels;

    private Vector3 _lastVelocity;
    private float _lastAngle;

    private void Start()
    {
    }

    public void Update()
    {
        float wheelCircumference = WheelRadius * 2 * Mathf.PI;
        float wheelRotateSpeed = Body.CurrentSpeed / wheelCircumference * 360f;

        foreach (Transform wheel in Wheels)
        {
            wheel.Rotate(wheelRotateSpeed * Time.deltaTime, 0f, 0f);
        }
        foreach (Transform wheel in TurnWheels)
        {
            float angle = Body.CurrentTurnAngle * WheelAngleMultiplier;
            wheel.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private void FixedUpdate()
    {
        Vector3 deltaVelocity = Body.Velocity - _lastVelocity;
        _lastVelocity = Body.Velocity;
        float deltaAngle = Body.transform.eulerAngles.y - _lastAngle;
        _lastAngle = Body.transform.eulerAngles.y;
        
        float accDelta = deltaVelocity.z / Time.fixedDeltaTime / MaxAccelerationDeltaVelocity;
        float turnDelta = deltaAngle / Time.fixedDeltaTime / MaxDeltaAngle;

        if (Body.CurrentSpeed < 0f)
        {
            turnDelta = -turnDelta;
        }

        Animator.SetFloat("AccFactor", Mathf.Clamp(accDelta, -1f, 1f), 0.25f, Time.fixedDeltaTime);
        Animator.SetFloat("TurnFactor", Mathf.Clamp(turnDelta, -1f, 1f), 0.25f, Time.fixedDeltaTime);
    }
}
