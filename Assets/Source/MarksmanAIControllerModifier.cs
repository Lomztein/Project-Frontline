using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarksmanAIControllerModifier : AIControllerModifier
{
    private AIController _controller;

    public override void OnInitialized(AIController controller)
    {
        _controller = controller;
        _controller.OnTargetAcquired += Controller_OnTargetAcquired;
    }

    private void Controller_OnTargetAcquired(ITarget obj)
    {
        if (obj.ExistsAndValid() && obj is ColliderTarget colTarget)
        {
            var weakpoints = colTarget.Collider.GetComponentInParent<Unit>().Weakpoints;
            if (weakpoints.Length > 0)
            {
                _controller.SetTarget(new TransformTarget(weakpoints.First().transform));
            }
        }
    }
}
