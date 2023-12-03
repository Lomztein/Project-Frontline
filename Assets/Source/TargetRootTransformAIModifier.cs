using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRootTransformAIModifier : AIControllerModifier
{
    private AIController _controller;

    public override void OnInitialized(AIController controller)
    {
        _controller = controller;
        controller.OnTargetAcquired += Controller_OnTargetAcquired;
    }

    private void Controller_OnTargetAcquired(ITarget obj)
    {
        if (obj is ColliderTarget col)
            _controller.SetTarget(new TransformTarget(col.Collider.transform.root));
    }
}
