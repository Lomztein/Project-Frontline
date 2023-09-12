using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplodeNearTarget : MonoBehaviour
{
    public Projectile Projectile;
    public float ExplosiveDamage;
    public float ExplosiveRange;
    public AnimationCurve DamageFalloff;
    public DamageModifier DamageModifier;

    // Start is called before the first frame update
    void Start()
    {
        Projectile.OnFired += Projectile_OnFired;
        Projectile.OnHit += Projectile_OnHit;
    }

    private void Projectile_OnHit(Projectile arg1, Collider arg2, Vector3 arg3, Vector3 arg4)
    {
        CancelInvoke();
        Burst();
    }

    private void Projectile_OnFired(Projectile arg1, Vector3 arg2)
    {
        if (Projectile.Target.ExistsAndValid())
        {
            float vertVel = Projectile.Velocity.y;
            float dist = Mathf.Abs(transform.position.y - Projectile.Target.GetCenter().y);
            Invoke(nameof(Burst), dist / vertVel);
            Invoke(nameof(BurstEffect), dist / vertVel);
            Invoke(nameof(EndProjectile), dist / vertVel); // the jank is real
        }
    }

    private void BurstEffect ()
    {
        Projectile.HandleHitEffects(transform.position, transform.forward);
    }

    private void EndProjectile ()
    {
        Projectile.End();
    }

    private void Burst ()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, ExplosiveRange, Projectile.HitLayerMask);
        foreach (Collider col in nearby)
        {
            var damagable = col.GetComponentInParent<IDamagable>();
            if (damagable != null)
            {
                float factor = Vector3.Distance(transform.position, col.transform.position) / ExplosiveRange;
                float damage = DamageFalloff.Evaluate(factor) * ExplosiveDamage;

                Vector3 hitPoint = col.ClosestPoint(transform.position);
                Projectile.DoDamage(damagable, damage, DamageModifier, hitPoint, (col.transform.position - transform.position).normalized);
            }
        }
    }
}
