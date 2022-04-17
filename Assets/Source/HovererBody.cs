using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class HovererBody : MobileBody, IControllable
{
    public float AccelerationSpeed;
    public float AngularAccelerationSpeed;

    public float HoverHeight;
    public float HoverSpeed;

    public float DragCoeffecient;
    public float AngularDragCoeffecient;

    public float AccFactorLerp;
    public float TurnFactorLeap;

    public float AccFactor { get; private set; }
    public float TurnFactor { get; private set; }
    public float AngularVelocity { get; private set; }

    private Vector3 _velocity;

    public override float CurrentSpeed { get; protected set; }

    public Vector3 Accelerate(float factor)
    {
        AccFactor = Mathf.Lerp(AccFactor, factor, AccFactorLerp * Time.fixedDeltaTime);
        return Velocity;
    }

    public Vector3 Turn(float factor)
    {
        TurnFactor = Mathf.Lerp(TurnFactor, factor, TurnFactorLeap * Time.fixedDeltaTime);
        return Velocity;
    }

    private void FixedUpdate()
    {
        Hover();
        Turn();
        Accelerate();

        _velocity += UnityUtils.ComputeSimpleDragForce(_velocity, DragCoeffecient) * Time.fixedDeltaTime;
        AngularVelocity -= UnityUtils.ComputeSimpleDrag(Mathf.Abs(AngularVelocity), AngularDragCoeffecient) * Mathf.Sign(AngularVelocity) * Time.fixedDeltaTime;
        CurrentSpeed = transform.InverseTransformVector(_velocity).z;

        Move (_velocity * Time.fixedDeltaTime);
        Rotate(0f, AngularVelocity * Time.fixedDeltaTime, 0f);
    }

    private void Hover ()
    {
        float hoverDelta = HoverHeight - transform.position.y;
        float hSpeed = Mathf.Min(Mathf.Abs(hoverDelta), HoverSpeed) * Mathf.Sign(hoverDelta);
        _velocity += Vector3.up * hSpeed * Time.fixedDeltaTime;
    }

    private void Turn ()
    {
        AngularVelocity += AngularAccelerationSpeed * TurnFactor * Time.fixedDeltaTime;
    }

    private void Accelerate ()
    {
        _velocity += transform.forward * AccelerationSpeed * AccFactor * Time.fixedDeltaTime;
    }
}
