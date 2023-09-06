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
        Projectile.OnHit += Explode;
    }

    private void Explode(Projectile proj, Collider col, Vector3 point, Vector3 norm)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRange, proj.HitLayerMask);
        foreach (Collider collider in colliders)
        {
            if (Physics.Linecast(proj.transform.position, collider.transform.position, out RaycastHit hit, Projectile.HitLayerMask) && hit.collider == collider)
            {
                IDamagable health = collider.GetComponentInParent<IDamagable>();
                if (health != null)
                {
                    Vector3 hitPoint = collider.ClosestPoint(point);
                    Projectile.DoDamage(health, ExplosionDamage, ExplosionDamageType, hitPoint, (collider.transform.position - transform.position).normalized);
                }
            } 
        }
    }
}
