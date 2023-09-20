using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBody : MobileBody, IControllable
{
    public override float CurrentSpeed { get; protected set; }
    public float MinSpeed;

    public float AccelerationSpeed;
    public float RaiseSpeed;
    public float FlightHeight;

    public float AccFactor;
    public float TurnFactor;
    public float TurnRate;

    public Vector3 Accelerate(float factor)
    {
        AccFactor = factor;
        return Velocity;
    }

    public Vector3 Turn(float factor)
    {
        TurnFactor = factor;
        return Velocity;
    }

    private void FixedUpdate()
    {
        Fly();
        CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, Mathf.Lerp(MinSpeed, MaxSpeed, AccFactor), AccelerationSpeed * Time.fixedDeltaTime);
        Vector3 velocity = CurrentSpeed * Time.fixedDeltaTime * transform.forward + Fly() * Time.fixedDeltaTime * Vector3.up;
        Move(velocity);

        Rotate(Quaternion.Euler(0f, TurnFactor * TurnRate * Time.fixedDeltaTime, 0f));
    }

    private float Fly()
    {
        float flyDelta = FlightHeight - transform.position.y;
        float hSpeed = Mathf.Min(Mathf.Abs(flyDelta), RaiseSpeed) * Mathf.Sign(flyDelta);
        return hSpeed;
    }
}
