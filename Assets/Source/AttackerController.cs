using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerController : AIController, IFactionComponent, IController
{
    private const float HOLD_VARIANCE = 2.5f;

    private Waypoint _currentWaypoint;
    public float AngleClamp;
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
            float angle = Mathf.Clamp(Vector3.SignedAngle(transform.forward, _currentWaypoint.OutgoingVector, Vector3.up), -AngleClamp, AngleClamp);
            Controllable.Turn(angle);
        }
    }

    protected virtual void MoveTowardsTarget ()
    {
        Vector3 local = GetTargetLocalPosition();
        float angle = Mathf.Clamp(Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg), -AngleClamp, AngleClamp);
        float speed = 1f;

        if (local.sqrMagnitude < HoldRange * HoldRange)
        {
            speed = 0f;
        }

        Controllable.Accelerate(speed);
        Controllable.Turn(angle / AngleClamp);
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
