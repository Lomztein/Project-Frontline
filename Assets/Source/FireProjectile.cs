using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : Projectile
{
    public float InitialHurtsphereSize;
    public float HurtsphereSizePerSecond;
    private float _hurtsphereSize;

    public float BaseDamage;


    public override void Fire(Vector3 direction)
    {
        base.Fire(direction);
        BaseDamage = Damage * 50;
        _hurtsphereSize = InitialHurtsphereSize;
    }

    protected override void FixedUpdate()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, _hurtsphereSize, HitLayerMask);
        foreach (Collider col in cols)
        {
            Damage = BaseDamage * Time.fixedDeltaTime;
            DoDamage(col, transform.position);
            InvokeOnHit(col, transform.position, (col.transform.position - transform.position).normalized);
        }
        if (Physics.Raycast(transform.position, Velocity, Speed * Time.fixedDeltaTime, HitLayerMask | TerrainLayerMask))
        {
            Velocity = Vector3.zero;
        }
        _hurtsphereSize += HurtsphereSizePerSecond * Time.fixedDeltaTime;
        transform.position += Velocity * Time.fixedDeltaTime;
    }



    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _hurtsphereSize);
    }
}
