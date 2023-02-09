using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealingBeamProjectile : Projectile
{
    public HitscanProjectileRenderer Renderer;

    public override void Fire(Vector3 direction)
    {
        base.Fire(direction);
        HitLayerMask = TeamInfo.Invert(HitLayerMask);

        float range = Life * Speed;
        var hits = Physics.RaycastAll(transform.position, direction, range, HitLayerMask | TerrainLayerMask);
        bool any = false;

        foreach (var hit in hits)
        {
            if (IsValidTarget(hit, out Health health))
            {
                health.Heal(Damage);
                HandleHitEffects(hit.point, hit.normal);
                Renderer.SetPositions(transform.position, hit.point);
                InvokeOnHit(hit.collider, hit.point, hit.normal);
                any = true;
                break;
            }
        }
        if (!any)
        {
            Renderer.SetPositions(transform.position, transform.position + direction * range);
        }
    }

    private bool IsValidTarget (RaycastHit hit, out Health health)
    {
        health = hit.collider.GetComponentInParent<Health>();
        if (hit.collider.CompareTag("Shield"))
        {
            return false;
        }

        if (health)
        {
            return health.CurrentHealth < health.MaxHealth - 0.01;
        }
        return false;
    }

    protected override void FixedUpdate()
    {
    }
}
