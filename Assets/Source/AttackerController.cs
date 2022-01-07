using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerController : AIController, ITeamComponent, IController
{
    private const float HOLD_VARIANCE = 2.5f;
    private const float STEER_DEVIANCE_CLAMP = 5f;

    protected Waypoint _currentWaypoint;
    public float HoldRange;

    protected override void Awake()
    {
        base.Awake();
        HoldRange = Math.Min(HoldRange, HoldRange + UnityEngine.Random.Range(-HOLD_VARIANCE, 0f));
    }

    public void SetWaypoint (Waypoint waypoint)
    {
        _currentWaypoint = waypoint;
    }

    protected virtual void MoveAlongWaypoints ()
    {
        if (_currentWaypoint)
        {
            Controllable.Accelerate(1f);
            float angle = Vector3.SignedAngle(transform.forward, _currentWaypoint.OutgoingVector, Vector3.up);
            SmoothTurnTowardsAngle(angle);
        }
    }

    protected virtual void MoveTowardsTarget ()
    {
        Vector3 local = GetTargetLocalPosition();
        float angle = Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg);
        float speed = 1f;

        if (local.sqrMagnitude < HoldRange * HoldRange)
        {
            speed = 0f;
        }

        Controllable.Accelerate(speed);
        SmoothTurnTowardsAngle(angle);
    }

    protected void SmoothTurnTowardsAngle(float angle) {
        float factor = Mathf.Abs(angle) > STEER_DEVIANCE_CLAMP ? Mathf.Sign(angle) : angle / STEER_DEVIANCE_CLAMP;
        Controllable.Turn(factor);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (CurrentTarget.ExistsAndValid())
        {
            MoveTowardsTarget();
        }
        else
        {
            MoveAlongWaypoints();
        }
    }
}
