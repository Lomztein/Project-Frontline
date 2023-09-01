using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclerController : AttackerController
{
    public bool CircleRight;
    [Range(0,1)]
    public float ChanceToCircleRight;

    protected override void Awake()
    {
        base.Awake();
        if (Random.Range(0f, 1f) < ChanceToCircleRight)
        {
            CircleRight = true;
        }
    }

    protected override void MoveTowardsTarget()
    {
        if (GetTargetSquareDistance() > HoldRange * HoldRange)
        {
            base.MoveTowardsTarget();
        }
        else
        {
            Vector3 between = (CurrentTarget.GetCenter() - transform.position).normalized;
            Vector3 perpendicular = Vector3.Cross(between, Vector3.up);

            Controllable.Turn(Vector3.Dot(perpendicular, transform.right * (CircleRight ? 1f : -1f)));
            Controllable.Accelerate(1f);
        }
    }
}
