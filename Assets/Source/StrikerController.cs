using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StrikerController : AttackerController
{
    private bool _engaging = true;
    public float ReengageRange;
    public bool DisengageAlongWaypoint;
    public bool DisengageWhileCantFire;

    protected override void MoveTowardsTarget()
    {
        _engaging = ShouldEngage(_engaging, GetTargetSquareDistance(), HoldRange * HoldRange, ReengageRange * ReengageRange);
        if (_engaging)
        {
            base.MoveTowardsTarget();
        }
        else
        {
            float speed = 1f;

            Controllable.Accelerate(speed);
            SmoothTurnTowardsAngle(GetDisengageAngle());
        }
    }

    private bool ShouldEngage (bool currentlyEngaging, float targetSqrDist, float sqrHoldRange, float sqrReengageRange)
    {
        if (!DisengageWhileCantFire)
        {
            if (targetSqrDist <= sqrHoldRange)
            {
                currentlyEngaging = false;
            }
            if (targetSqrDist >= sqrReengageRange)
            {
                currentlyEngaging = true;
            }
        }
        else
        {
            currentlyEngaging = Weapons.Any(x => x.CanFire());
        }
        return currentlyEngaging;
    }

    private float GetDisengageAngle ()
    {
        float angle;
        if (DisengageAlongWaypoint)
        {
            angle = Navigation.IncomingAngle(PrevNode, NextNode) + LaneOffset;
            angle = Mathf.DeltaAngle(transform.eulerAngles.y, angle);
        }
        else
        {
            Vector3 local = GetTargetLocalPosition();
            angle = Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg - 180);
        }
        return angle;
    }

    protected override void MoveAlongWaypoints()
    {
        base.MoveAlongWaypoints();
        float sqrDist;
        if (StayBehindFrontline)
        {
            float dist = DistToFrontline();
            float sign = Mathf.Sign(dist);
            sqrDist = dist * dist * sign;
        }
        else
        {
            sqrDist = 9999;
        }
        _engaging = ShouldEngage(_engaging, sqrDist, HoldRange * HoldRange, ReengageRange * ReengageRange);

        if (!_engaging)
        {
            SmoothTurnTowardsAngle(GetDisengageAngle());
        }
        Controllable.Accelerate(1f);
    }
}
