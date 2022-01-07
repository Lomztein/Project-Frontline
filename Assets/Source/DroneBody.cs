using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBody : MobileBody, IControllable
{
    private float _accFactor;
    private float _turnFactor;

    public float TurnSpeed;

    public float HoverHeight;
    public float HoverSpeed;

    public override float CurrentSpeed { get; protected set; }
    public float CurrentAngularSpeed => _turnFactor * TurnSpeed;

    public float AccelerationLerpSpeed = 2.5f;
    public float TurnLerpSpeed = 2.5f;

    public void FixedUpdate()
    {
        float hoverDelta = HoverHeight - transform.position.y;
        float hSpeed = Mathf.Min(Mathf.Abs(hoverDelta), HoverSpeed) * Mathf.Sign(hoverDelta);
        CurrentSpeed = _accFactor * MaxSpeed;

        Move (transform.forward * CurrentSpeed * Time.fixedDeltaTime + Vector3.up * hSpeed * Time.fixedDeltaTime);
        Rotate (Quaternion.Euler (0f, _turnFactor * TurnSpeed * Time.fixedDeltaTime, 0f));
    }

    public Vector3 Accelerate(float factor)
    {
        _accFactor = Mathf.Lerp (_accFactor, Mathf.Clamp(factor, -1f, 1f), AccelerationLerpSpeed * Time.fixedDeltaTime);
        return transform.forward * MaxSpeed * _accFactor * Time.fixedDeltaTime;
    }

    public Vector3 Turn(float factor)
    {
        _turnFactor = Mathf.Lerp (_turnFactor, Mathf.Clamp(factor, -1f, 1f), TurnLerpSpeed * Time.fixedDeltaTime);
        return transform.forward * MaxSpeed * _accFactor * Time.fixedDeltaTime;
    }
}
