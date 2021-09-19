using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryBody : MobileBody, IControllable
{
    public bool IsMoving;

    private float _accFactor;
    private float _rotFactor;
    private float _targetRot;

    public float MovementSpeed;
    public float RotateSpeed;

    public override float CurrentSpeed { get; protected set; }

    private void FixedUpdate()
    {
        IsMoving = Mathf.Abs(_accFactor) > 0.01f;

        CurrentSpeed = _accFactor * MovementSpeed;
        Move (transform.forward * _accFactor * MovementSpeed * Time.fixedDeltaTime);
        Rotate(0f, _rotFactor * RotateSpeed * Time.fixedDeltaTime, 0f);
    }

    public Vector3 Accelerate(float factor)
    {
        _accFactor = Mathf.Lerp(Mathf.Clamp(factor, -1f, 1f), 0f, 0.03f);
        return transform.forward * _accFactor * MovementSpeed;
    }

    public Vector3 Turn(float factor)
    {
        _rotFactor = Mathf.Lerp(Mathf.Clamp(factor, -1f, 1f), 0f, 0.03f);
        return transform.forward * _accFactor * MovementSpeed;
    }
}
