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
        float sqrDist = GetTargetSquareDistance();
        if (!DisengageWhileCantFire)
        {
            if (sqrDist <= HoldRange * HoldRange)
            {
                _engaging = false;
            }
            if (sqrDist >= ReengageRange * ReengageRange)
            {
                _engaging = true;
            }
        }
        else
        {
            _engaging = Weapons.Any(x => x.CanFire());
        }
        if (_engaging)
        {
            base.MoveTowardsTarget();
        }
        else
        {
            float angle;
            if (DisengageAlongWaypoint)
            {
                angle = _currentWaypoint.IncomingAngle;
                angle = Mathf.DeltaAngle(transform.eulerAngles.y, angle);
            }
            else
            {
                Vector3 local = GetTargetLocalPosition();
                angle = Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg - 180);
            }
            float speed = 1f;

            Controllable.Accelerate(speed);
            SmoothTurnTowardsAngle(angle);
        }
    }

    protected override void MoveAlongWaypoints()
    {
        base.MoveAlongWaypoints();
        _engaging = true;
    }
}
