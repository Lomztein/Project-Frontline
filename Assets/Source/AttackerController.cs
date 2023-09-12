 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Util;

public class AttackerController : ControllableController, ITeamComponent, ICommanderComponent, IController
{
    private const float HOLD_VARIANCE = 2.5f;


    protected IWaypoint _currentWaypoint;
    public float Lane { get; private set; } // x-position relative to commander.
    public float LaneOffset => Commander.transform.InverseTransformPoint(transform.position).x - Lane;

    protected Commander Commander { get; private set; }

    public float HoldRange;
    public bool StayBehindFrontline;

    protected override void Awake()
    {
        base.Awake();
        HoldRange = Math.Min(HoldRange, HoldRange + UnityEngine.Random.Range(-HOLD_VARIANCE, 0f));
    }

    public void SetWaypoint (IWaypoint waypoint)
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
        => VectorUtils.DifferenceAlongDirection(Waypoint.OutgoingVector(_currentWaypoint), Commander.Frontline.Position, transform.position);

    protected virtual void MoveAlongWaypoints ()
    {
        if (_currentWaypoint != null)
        {
            if (ShouldHoldOnFrontline()) 
            {
                Controllable.Accelerate(0f);
            }
            else
            {
                Controllable.Accelerate(1f);
            }
            float angle = Vector3.SignedAngle(transform.forward, Waypoint.OutgoingVector(_currentWaypoint), Vector3.up);
            SmoothTurnTowardsAngle(angle);

            IWaypoint next = _currentWaypoint.GetNext();
            if (next != null)
            {
                Vector3 dir = (Waypoint.OutgoingVector(_currentWaypoint) + Waypoint.OutgoingVector(next)) / 2f;
                if (Vector3.Dot((next.Position - transform.position).normalized, dir) < 0f)
                    _currentWaypoint = next;
            }
        }
    }

    protected virtual void MoveTowardsTarget ()
    {
        Vector3 local = GetTargetLocalPosition();
        Vector3 targetPos = GetTarget().GetCenter();
        if (local.sqrMagnitude < HoldRange * HoldRange)
        {
            Stop();
            TurnTowardsPosition(targetPos);
        }
        else
        {
            MoveTowardsPosition(targetPos);
        }
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
        Lane = commander.transform.InverseTransformPoint(transform.position).x;
    }

    private void OnDrawGizmosSelected()
    {
        if (StayBehindFrontline && _currentWaypoint != null && Commander)
        {
            Vector3 margin = Waypoint.OutgoingVector(_currentWaypoint) * HoldRange / 2f;
            Gizmos.DrawSphere(transform.position + margin, 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + margin);
        }
    }
}
