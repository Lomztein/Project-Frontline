using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Util;

public class CirclerController : AttackerController
{
    public bool CircleRight;
    [Range(0,1)]
    public float ChanceToCircleRight;
    public float CircleSign => CircleRight ? 1f : -1f;
    public float Speed = 50f;
    public float Turnrate = 60f;
    public float Margin = 5f;

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
        float dist = Vector3.Distance(transform.position.Flat(), CurrentTarget.GetCenter().Flat());

        float ms = Mathf.Pow(Margin, 2f);
        float hs = Mathf.Pow(HoldRange, 2f);
        float ts = Mathf.Pow(dist, 2f);
        if (ts > hs + ms)
        {
            MoveTowardsPosition(CalcEntrancePoint(transform.position.Flat(), CurrentTarget.GetCenter()));
        }
        else
        {
            Vector3 point = CalcPerpendicularPoint(transform.position, CurrentTarget.GetCenter());
            Vector3 diff = point - transform.position.Flat();
            Vector3 targetDiff = CurrentTarget.GetCenter().Flat() - transform.position.Flat();

            float ca = Mathf.Atan2(targetDiff.x, targetDiff.z) * Mathf.Rad2Deg;

            float c4 = Mathf.PI * 2f * HoldRange / 4f;
            float t = c4 / Speed;
            float oa = 90f / t;

            float angle = ca - (90f - oa) * CircleSign;
            float da = Mathf.DeltaAngle(transform.eulerAngles.y, angle);

            Controllable.Turn(Mathf.Clamp(da / Turnrate, -1f, 1f));
        }
    }

    private Vector3 CalcPerpendicularPoint(Vector3 currentPos, Vector3 targetPos)
    {
        currentPos = currentPos.Flat();
        targetPos = targetPos.Flat();

        Vector3 between = (targetPos - currentPos).normalized;
        Vector3 perpendicular = Vector3.Cross(between, Vector3.up) * CircleSign;
        return targetPos + perpendicular * HoldRange;
    }

    // Stole from here https://stackoverflow.com/a/15846131
    private Vector3 CalcEntrancePoint(Vector3 currentPos, Vector3 targetPos)
    {
        Vector3 diff = targetPos - currentPos;
        float mag = diff.magnitude;
        float a = Mathf.Asin(HoldRange / mag);
        float b = Mathf.Atan2(diff.x, diff.z);
        float t = b - a * CircleSign;
        return new Vector3(
            HoldRange * Mathf.Cos(t) * CircleSign * -1f,
            0f,
            HoldRange * Mathf.Sin(t) * CircleSign
        );
    }

    private void OnDrawGizmos()
    {
        if (CurrentTarget.ExistsAndValid())
        {
            Vector3 circleEntrancePoint = CalcEntrancePoint(transform.position.Flat(), CurrentTarget.GetCenter());
            Gizmos.DrawSphere(circleEntrancePoint, 0.5f);
            Gizmos.DrawLine(transform.position.Flat(), circleEntrancePoint);
            Gizmos.DrawWireSphere(CurrentTarget.GetCenter().Flat(), HoldRange);
        }
    }
}
