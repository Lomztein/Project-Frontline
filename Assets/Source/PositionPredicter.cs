using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPredicter
{
    private Vector3 _targetPrevPosition;
    private Vector3 _targetVelocity;

    public void Tick(Vector3 targetPosition, float deltaTime)
    {
        _targetVelocity= (targetPosition - _targetPrevPosition) / deltaTime;
        _targetPrevPosition = targetPosition;
    }

    public Vector3 GetPredictedPosition(Vector3 targetPosition, float time)
        => targetPosition + _targetVelocity * time;

    public Vector3 GetPredictedPosition(Vector3 targetPosition, float distance, float speed)
        => GetPredictedPosition(targetPosition, distance / speed);
}
