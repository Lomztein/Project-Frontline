 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class AttackerController : AIController, ITeamComponent, ICommanderComponent, IController
{
    private const float HOLD_VARIANCE = 2.5f;
    private const float STEER_DEVIANCE_CLAMP = 5f;

    protected Waypoint _currentWaypoint;
    protected Commander Commander { get; private set; }

    public float HoldRange;
    public bool StayBehindFrontline;

    protected override void Awake()
    {
        base.Awake();
        HoldRange = Math.Min(HoldRange, HoldRange + UnityEngine.Random.Range(-HOLD_VARIANCE, 0f));
    }

    public void SetWaypoint (Waypoint waypoint)
    {
        _currentWaypoint = waypoint;
    }

    protected bool ShouldHoldOnFrontline()
    {
        if (StayBehindFrontline && Commander)
        {
            float dist = DistToFrontline();
            return dist < HoldRange / 2f;
        }
        return false;
    }

    protected float DistToFrontline()
        => VectorUtils.DifferenceAlongDirection(_currentWaypoint.OutgoingVector, Commander.Frontline.Position, transform.position);

    protected virtual void MoveAlongWaypoints ()
    {
        if (_currentWaypoint)
        {
            if (ShouldHoldOnFrontline()) 
            {
                Controllable.Accelerate(0f);
            }
            else
            {
                Controllable.Accelerate(1f);
            }
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

    public void AssignCommander(Commander commander)
    {
        Commander = commander;
    }

    private void OnDrawGizmosSelected()
    {
        if (StayBehindFrontline && _currentWaypoint && Commander)
        {
            Vector3 margin = _currentWaypoint.OutgoingVector * HoldRange / 2f;
            Gizmos.DrawSphere(transform.position + margin, 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + margin);
        }
    }
}
