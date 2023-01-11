using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplodeOnEnd : MonoBehaviour
{
    public Projectile Projectile;

    public float ExplosionRange;
    public float ExplosionDamage;
    public DamageMatrix.Damage ExplosionDamageType;

    // Start is called before the first frame update
    void Start()
    {
        Projectile.OnEnd += Explode;
    }

    private void Explode(Projectile proj)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRange, proj.HitLayerMask);
        foreach (Collider collider in colliders)
        {
            if (Physics.Linecast(proj.transform.position, collider.transform.position, out RaycastHit hit, Projectile.HitLayerMask) && hit.collider == collider)
            {
                IDamagable[] healths = collider.GetComponentsInParent<IDamagable>();
                foreach (IDamagable health in healths)
                {
                    Projectile.DoDamage(health, ExplosionDamage, ExplosionDamageType, hit.collider, transform.position, (collider.transform.position - transform.position).normalized);
                }
            } 
        }
    }
}
