using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileIncomingDamage : MonoBehaviour
{
    public Projectile Parent;
    private float _damageAgainstTarget;

    private void Awake()
    {
        Parent.OnFired += Parent_OnFired;
        Parent.OnEnd += Parent_OnEnd;
    }

    private void Parent_OnEnd(Projectile projectile)
    {
        if (projectile.Target != null && projectile.Target.GetGameObject())
        {
            TargetIncomingDamageTracking.SubtractIncomingDamage(projectile.Target.GetGameObject(), _damageAgainstTarget);
        }
    }

    private void Parent_OnFired(Projectile projectile, Vector3 direction)
    {
        if (projectile.Target != null && projectile.Target.GetGameObject())
        {
            DamageModifier modifier = DamageModifier.One;
            Health health = projectile.Target.GetGameObject().GetComponentInParent<Health>();
            if (health)
            {
                modifier = health.Modifier;
            }
            _damageAgainstTarget = DamageModifier.Combine(modifier, projectile.Modifier) * projectile.Damage - 0.1f;
            TargetIncomingDamageTracking.AddIncomingDamage(projectile.Target.GetGameObject(), _damageAgainstTarget);
        }
    }
}
