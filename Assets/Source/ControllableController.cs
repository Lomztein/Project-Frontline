using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableController : AIController
{
    private const float STEER_DEVIANCE_CLAMP = 5f;
    public IControllable Controllable;

    protected override void Awake()
    {
        base.Awake();
        Controllable = GetComponentInChildren<IControllable>();
    }

    public void MoveTowardsPosition(Vector3 position)
    {
        TurnTowardsPosition(position);
        float speed = 1f;
        Controllable.Accelerate(speed);
    }

    public void TurnTowardsPosition(Vector3 position)
    {
        Vector3 local = PositionToLocalPosition(position);
        float angle = Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg);
        SmoothTurnTowardsAngle(angle);
    }

    public void Stop ()
    {
        Controllable.Accelerate(0f);
    }

    protected void SmoothTurnTowardsAngle(float angle)
    {
        float factor = Mathf.Abs(angle) > STEER_DEVIANCE_CLAMP ? Mathf.Sign(angle) : angle / STEER_DEVIANCE_CLAMP;
        Controllable.Turn(factor);
    }
}
