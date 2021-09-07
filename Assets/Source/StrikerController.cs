using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerController : AttackerController
{
    private bool _engaging = true;

    protected override void MoveTowardsTarget()
    {
        float sqrDist = GetTargetSquareDistance();
        if (sqrDist <= HoldRange * HoldRange)
        {
            _engaging = false;
        }
        if (_engaging)
        {
            base.MoveTowardsTarget();
        }
        else
        {
            Vector3 local = GetTargetLocalPosition();
            float angle = Mathf.Clamp(Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg - 180), -AngleClamp, AngleClamp);
            float speed = 1f;

            Controllable.Accelerate(speed);
            Controllable.Turn(angle / AngleClamp);
        }
    }

    protected override void MoveAlongWaypoints()
    {
        base.MoveAlongWaypoints();
        _engaging = true;
    }
}
