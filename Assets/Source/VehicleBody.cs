using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBody : MobileBody, IControllable
{
    public override float CurrentSpeed { get; protected set; }

    public float AccelerationSpeed;
    public float BrakeDrag;
    private float _targetSpeedFactor;

    public float MaxTurnAngle;
    public float CurrentTurnAngle { get; private set; }
    public float TurnAngleRotationSpeed;
    private float _targetTurnFactor;

    public bool Threaded;

    public Vector3 Accelerate(float factor)
    {
        _targetSpeedFactor = factor;
        return Velocity;
    }

    public Vector3 Turn(float factor)
    {
        _targetTurnFactor = factor;
        return Velocity;
    }

    private void FixedUpdate()
    {
        UpdateSpeed(Time.fixedDeltaTime);
        UpdateTurnAngle(Time.fixedDeltaTime);
        Move(Time.fixedDeltaTime);
    }

    private void UpdateSpeed(float deltaTime)
    {
        float targetSpeed = _targetSpeedFactor * MaxSpeed;
        float acc = targetSpeed > CurrentSpeed ? AccelerationSpeed : BrakeDrag;
        if (CurrentSpeed < 0f)
        {
            acc = targetSpeed < CurrentSpeed ? AccelerationSpeed : BrakeDrag;
        }
        CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetSpeed, acc * deltaTime);
    }

    private void UpdateTurnAngle(float deltaTime)
    {
        float targetAngle = _targetTurnFactor * MaxTurnAngle;
        CurrentTurnAngle = Mathf.MoveTowards(CurrentTurnAngle, targetAngle, TurnAngleRotationSpeed * deltaTime);
    }
    private void Move(float deltaTime)
    {
        Move (transform.forward * CurrentSpeed * deltaTime);
        float speedTurnFactor = Threaded ? 1f : CurrentSpeed / MaxSpeed;
        Rotate(0f, CurrentTurnAngle * speedTurnFactor * deltaTime, 0f);
    }
}
