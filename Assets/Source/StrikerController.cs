using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerController : AttackerController
{
    private bool _engaging = true;
    public float ReengageRange;
    public bool DisengageAlongWaypoint;

    protected override void MoveTowardsTarget()
    {
        float sqrDist = GetTargetSquareDistance();
        if (sqrDist <= HoldRange * HoldRange)
        {
            _engaging = false;
        }
        if (sqrDist >= ReengageRange * ReengageRange)
        {
            _engaging = true;
        }
        if (_engaging)
        {
            base.MoveTowardsTarget();
        }
        else
        {
            Vector3 local = GetTargetLocalPosition();
            float angle = Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg - 180);
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
