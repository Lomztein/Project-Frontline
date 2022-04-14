using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetProjectilesAIControllerModifier : AIControllerModifier
{
    public override void OnInitialized(AIController controller)
    {
        controller.SetTargetLayerMask(controller.Team.ProjectileGetOtherLayerMasks());
        foreach (IWeapon weapon in controller.Weapons)
        {
            if (weapon is Weapon concrete)
            {
                concrete.SetHitLayerMask(controller.Team.ProjectileGetOtherLayerMasks());
            }
        }
    }

}
