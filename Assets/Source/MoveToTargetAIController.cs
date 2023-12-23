using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class MoveToTargetAIController : AttackerController
{
    protected override void MoveTowardsTarget()
    {
        Vector3 flatCurrent = transform.position.Flat();
        Vector3 flatTarget = CurrentTarget.GetCenter().Flat();
        
        if (Vector3.SqrMagnitude(flatCurrent - flatTarget) > HoldRange * HoldRange)
        {
            MoveTowardsPosition(CurrentTarget.GetCenter());
        }
        else
        {
            Controllable.Accelerate(0f);
        }
    }
}
