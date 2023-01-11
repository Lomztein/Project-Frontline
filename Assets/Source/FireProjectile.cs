using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : Projectile
{
    public float InitialHurtsphereSize;
    public float HurtsphereSizePerSecond;
    private float _hurtsphereSize;

    public GameObject BurnPrefab;
    public float BaseDamage;

    private void Start()
    {
        OnHit += FireProjectile_OnHit;
    }

    private void FireProjectile_OnHit(Projectile arg1, Collider arg2, Vector3 arg3, Vector3 arg4)
    {
        ApplyBurn(arg2);
    }

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
        }
        if (Physics.Raycast(transform.position, Velocity, Speed * Time.fixedDeltaTime, HitLayerMask | TerrainLayerMask))
        {
            Velocity = Vector3.zero;
        }
        _hurtsphereSize += HurtsphereSizePerSecond * Time.fixedDeltaTime;
        transform.position += Velocity * Time.fixedDeltaTime;
    }

    private void ApplyBurn (Collider target)
    {
        BurnController cont = target.transform.root.GetComponentInChildren<BurnController>();
        if (cont)
        {
            cont.Destroyer.ResetDestroyTimer();
        }
        else
        {
            GameObject newBurn = Instantiate(BurnPrefab, target.transform);
            newBurn.transform.localPosition = Vector3.zero;
            newBurn.GetComponent<BurnController>().SetTarget(target);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _hurtsphereSize);
    }
}
