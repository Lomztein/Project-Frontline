using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanProjectile : Projectile
{
    public HitscanProjectileRenderer Renderer;

    public override void Fire(Vector3 direction)
    {
        base.Fire(direction);
        float range = Life * Speed;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, range, HitLayerMask | TerrainLayerMask))
        {
            DoDamage(hit.collider, hit.point);
            Hit(hit.point, hit.normal);
            Renderer.SetPositions(transform.position, hit.point);
        }
        else
        {
            Renderer.SetPositions(transform.position, transform.position + direction * range);
        }
    }

    protected override void FixedUpdate()
    {
    }
}
