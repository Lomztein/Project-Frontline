using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheeledVehicleBodyAnimator : MonoBehaviour
{
    public VehicleBody Body;
    public float WheelRadius;
    public float WheelAngleMultiplier;

    public Transform[] TurnWheels;
    public Transform[] Wheels;

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
}
