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
        _engaging = ShouldEngage(_engaging, GetTargetSquareDistance());
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

    private bool ShouldEngage (bool currentlyEngaging, float targetSqrDist)
    {
        if (!DisengageWhileCantFire)
        {
            if (targetSqrDist <= HoldRange * HoldRange)
            {
                currentlyEngaging = false;
            }
            if (targetSqrDist >= ReengageRange * ReengageRange)
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
            angle = _currentWaypoint.IncomingAngle + LaneOffset;
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
        float dist = DistToFrontline();
        float sign = Mathf.Sign(dist);
        if (ShouldHoldOnFrontline())
        {
            _engaging = ShouldEngage(_engaging, Mathf.Pow(dist, 2f) * sign);

            if (!_engaging)
            {
                SmoothTurnTowardsAngle(GetDisengageAngle());
            }
        }
        else
        {
            _engaging = true;
        }
        Controllable.Accelerate(1f);
    }
}
