using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindControllerAIControllerModifier : AIControllerModifier
{
    public MindControlChannelWeapon Weapon;
    private AIController _controller;

    public override void OnInitialized(AIController controller)
    {
        _controller = controller;
        _controller.SetTargetLayerMask(_controller.TargetLayer | _controller.Team.GetLayerMask());
        _controller.AppendTargetFilter(FilterAlliedTargets);
    }

    private bool FilterAlliedTargets(GameObject obj)
    {
        Unit targetUnit = obj.GetComponentInParent<Unit>();
        if (targetUnit != null)
        {
            if (targetUnit != Weapon.CurrentControllingUnit)
            {
                return targetUnit.TeamInfo != _controller.Team;
            }
        }
        return true;
    }
}
