using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealerAIControllerModifier : AIControllerModifier
{
    public AIController Controller;
    private Health _currentTargetHealth;

    public override void OnInitialized (AIController controller)
    {
        Controller = controller;

        Controller.SetTargetEvaluator((pos, go) =>
        {
        Health health = go.GetComponentInParent<Health>();
        if (health)
        {
                return 1 - (health.CurrentHealth / health.MaxHealth) + Random.Range(-0.25f, 0.25f);
            }
            return float.MinValue;
        });
        Controller.SetTargetFilter(go =>
        {
            Health health = go.GetComponentInParent<Health>();
            if (health)
            {
                return (health.CurrentHealth < health.MaxHealth - 10f) && go.transform.root != transform.root && health.ArmorType != DamageMatrix.Armor.Shield;
            }
            return false;
        });

        Controller.OnTargetAcquired += Controller_OnTargetAcquired;
        Controller.OnTargetLost += Controller_OnTargetLost;
        Controller.SetTargetLayerMask(Controller.Team.GetLayerMask());
    }

    private void Controller_OnTargetLost(ITarget obj)
    {
        _currentTargetHealth = null;
    }

    private void Controller_OnTargetAcquired(ITarget obj)
    {
        if (obj is ColliderTarget target)
        {
            _currentTargetHealth = target.Collider.GetComponentInParent<Health>();
        }
    }

    private void FixedUpdate()
    {
        if (_currentTargetHealth)
        {
            if (_currentTargetHealth.CurrentHealth > _currentTargetHealth.MaxHealth - 0.01f)
            {
                Controller.LooseTarget();
            }
        }
    }
}
