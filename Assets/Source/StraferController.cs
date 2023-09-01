using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraferController : AttackerController
{
    protected override void MoveTowardsTarget()
    {
        if (GetTargetSquareDistance() > HoldRange * HoldRange)
        {
            base.MoveTowardsTarget();
        }
        else
        {
            Vector3 thisPlane = new Vector3(transform.position.x, 0f, transform.position.y);
            Vector3 targetPlane = new Vector3(CurrentTarget.GetCenter().x, 0f, CurrentTarget.GetCenter().y);
            Vector3 diff = targetPlane - thisPlane;
            if (Vector3.Dot(transform.forward, diff.normalized) > 0f)
            {
                base.MoveTowardsTarget();
                Controllable.Accelerate(1f);
            }
            else
            {
                Controllable.Accelerate(1f);
                Controllable.Turn(0f);
            }
        }
    }
}
