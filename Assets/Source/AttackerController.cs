 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Util;

public class AttackerController : ControllableController, ITeamComponent, ICommanderComponent, IController
{
    private const float HOLD_VARIANCE = 2.5f;

    protected NavigationNode[] MovementPath;
    protected int MovementPathIndex;

    protected NavigationNode NextNode
        => MovementPathIndex == MovementPath.Length - 1 ? null : MovementPath[MovementPathIndex + 1];
    protected NavigationNode PrevNode
        => MovementPath[MovementPathIndex];

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

    public void SetPath (NavigationNode[] path)
    {
        MovementPath = path;
        MovementPathIndex = 0;
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
    {
        return VectorUtils.DifferenceAlongDirection(Navigation.OutgoingVector(PrevNode, NextNode), Commander.Frontline.Position, transform.position);
    }


    protected virtual void MoveAlongWaypoints ()
    {
        if (MovementPath != null)
        {
            if (ShouldHoldOnFrontline()) 
            {
                Controllable.Accelerate(0f);
            }
            else
            {
                Controllable.Accelerate(1f);
            }
            float angle = Vector3.SignedAngle(transform.forward, Navigation.OutgoingVector(PrevNode, NextNode), Vector3.up);
            SmoothTurnTowardsAngle(angle);

            if (MovementPathIndex < MovementPath.Length - 2)
            {
                Vector3 dir = (Navigation.OutgoingVector(PrevNode, NextNode) + Navigation.OutgoingVector(MovementPath[MovementPathIndex + 1], MovementPath[MovementPathIndex + 2])) / 2f;
                if (Vector3.Dot((NextNode.Position - transform.position).normalized, dir) < 0f)
                    MovementPathIndex++;
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
}
