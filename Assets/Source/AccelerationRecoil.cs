﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationRecoil : MonoBehaviour
{
    public VehicleBody Body;
    public VehicleBodyRecoilAnimator Anim;
    public float MaxAccelerationDeltaVelocity;
    public float MaxDeltaAngle;

    private Vector3 _lastVelocity;
    private float _lastAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 deltaVelocity = Body.Velocity - _lastVelocity;
        _lastVelocity = Body.Velocity;
        float deltaAngle = Body.transform.eulerAngles.y - _lastAngle;
        _lastAngle = Body.transform.eulerAngles.y;

        float accDelta = deltaVelocity.z / Time.fixedDeltaTime / MaxAccelerationDeltaVelocity;
        float turnDelta = deltaAngle / Time.fixedDeltaTime / MaxDeltaAngle;
        turnDelta *= Body.MaxSpeed / Body.MaxSpeed;

        if (Mathf.Abs (turnDelta) > 180f)
        {
            turnDelta = 0f;
        }

        if (Body.CurrentSpeed < 0f)
        {
            turnDelta = -turnDelta;
        }

        Anim.Recoil(Body.transform.TransformVector (-new Vector3(turnDelta, 0, accDelta)));
    }
}
