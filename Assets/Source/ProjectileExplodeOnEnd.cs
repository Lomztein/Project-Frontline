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
        Projectile.OnHit += Explode; ;   
    }

    private void Explode(Projectile arg1, Collider arg2, Vector3 arg3, Vector3 arg4)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRange, arg1.HitLayerMask);
        foreach (Collider collider in colliders)
        {
            IDamagable[] healths = collider.GetComponentsInParent<IDamagable>();
            foreach (IDamagable health in healths)
            {
                Projectile.DoDamage(health, ExplosionDamage, ExplosionDamageType, transform.position, (collider.transform.position - transform.position).normalized);
            }
        }
    }
}
