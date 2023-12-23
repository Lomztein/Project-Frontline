using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidOverAttackingTargetAIControllerModifier : AIControllerModifier
{
    private AIController _controller;

    public override void OnInitialized(AIController controller)
    {
        _controller = controller;
        foreach (var weapon in controller.Weapons)
        {
            weapon.OnFire += OnFire;
        }
        _controller.AppendTargetFilter(FilterDoomedTargets);
    }

    private void OnFire(IWeapon obj)
    {
        _controller.LooseTarget();
        _controller.FindNewTarget();
    }

    private bool FilterDoomedTargets (GameObject target)
    {
        Health health = target.GetComponentInChildren<Health>();
        return health.CurrentHealth > TargetIncomingDamageTracking.GetIncomingDamage(target);
    }
}
