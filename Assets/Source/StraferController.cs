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
            Controllable.Accelerate(1f);
            Controllable.Turn(0f);
        }
    }
}
