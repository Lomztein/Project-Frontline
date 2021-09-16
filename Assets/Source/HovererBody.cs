using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovererBody : MonoBehaviour, IControllable
{
    public float AccelerationSpeed;
    public float AngularAccelerationSpeed;

    public float HoverHeight;
    public float HoverSpeed;

    public float Dampening;
    public float AngularDampening;

    public float AccFactorLerp;
    public float TurnFactorLeap;

    public float AccFactor { get; private set; }
    public float TurnFactor { get; private set; }
    public Vector3 Velocity { get; private set; }
    public float AngularVelocity { get; private set; }

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

        Velocity *= Dampening;
        AngularVelocity *= AngularDampening;

        transform.position += Velocity * Time.fixedDeltaTime;
        transform.Rotate(0f, AngularVelocity * Time.fixedDeltaTime, 0f);
    }

    private void Hover ()
    {
        float hoverDelta = HoverHeight - transform.position.y;
        float hSpeed = Mathf.Min(Mathf.Abs(hoverDelta), HoverSpeed) * Mathf.Sign(hoverDelta);
        Velocity += Vector3.up * hSpeed * Time.fixedDeltaTime;
    }

    private void Turn ()
    {
        AngularVelocity += AngularAccelerationSpeed * TurnFactor * Time.fixedDeltaTime;
    }

    private void Accelerate ()
    {
        Velocity += transform.forward * AccelerationSpeed * AccFactor * Time.fixedDeltaTime;
    }
}
