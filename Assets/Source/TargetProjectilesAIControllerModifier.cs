using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetProjectilesAIControllerModifier : AIControllerModifier
{
    public bool AppendLayerMask;

    public override void OnInitialized(AIController controller)
    {
        var layer = AppendLayerMask ? controller.TargetLayer | controller.Team.ProjectileGetOtherLayerMasks() : controller.Team.ProjectileGetOtherLayerMasks();
        controller.SetTargetLayerMask(layer);
        foreach (IWeapon weapon in controller.Weapons)
        {
            weapon.SetHitLayerMask(layer);
        }
    }

}
