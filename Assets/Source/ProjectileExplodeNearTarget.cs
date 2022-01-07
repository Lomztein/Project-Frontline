using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplodeNearTarget : MonoBehaviour
{
    public Projectile Projectile;
    public float ExplosiveDamage;
    public float ExplosiveRange;
    public AnimationCurve DamageFalloff;
    public DamageMatrix.Damage DamageType;

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
            float dist = Vector3.Distance(transform.position, Projectile.Target.GetPosition());
            Invoke(nameof(Burst), dist / Projectile.Speed);
            Invoke(nameof(BurstEffect), dist / Projectile.Speed); // the jank is real
        }
    }

    private void BurstEffect ()
    {
        Projectile.Hit(transform.position, transform.forward);
    }

    private void Burst ()
    {
        Projectile.End();

        Collider[] nearby = Physics.OverlapSphere(transform.position, ExplosiveRange, Projectile.HitLayerMask);
        foreach (Collider col in nearby)
        {
            var damagable = col.GetComponentInParent<IDamagable>();
            if (damagable != null)
            {
                float factor = Vector3.Distance(transform.position, col.transform.position) / ExplosiveRange;
                float damage = DamageFalloff.Evaluate(factor) * ExplosiveDamage;

                damagable.TakeDamage(new DamageInfo(damage, DamageType, col.transform.position, (col.transform.position - transform.position).normalized));
            }
        }
    }
}
