using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IFactionComponent
{
    public float Speed;
    public float Damage;
    public DamageArmorMapping.Damage DamageType;
    public Vector3 Velocity;
    public ITarget Target;

    public float Life;
    public GameObject HitParticle;
    public float HitParticleLife;

    private const int TerrainLayerMask = 1 << 8;
    private LayerMask _hitLayer;

    public void Fire (Vector3 direction)
    {
        Destroy(gameObject, Life);
        transform.LookAt(transform.position + direction);
        Velocity = Speed * direction;
    }
    private void FixedUpdate()
    {
        transform.position += Velocity * Time.fixedDeltaTime;
        if (Physics.Raycast(transform.position, transform.forward * Speed * Time.fixedDeltaTime, out RaycastHit hit, Speed * Time.fixedDeltaTime, _hitLayer | TerrainLayerMask))
        {
            Destroy(gameObject);
            Destroy (Instantiate(HitParticle, hit.point, Quaternion.LookRotation(hit.normal)), HitParticleLife);

            var damagable = hit.collider.GetComponentInParent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(DamageType, Damage);
            }
        }
    }

    public void SetFaction(Faction faction)
    {
        _hitLayer = faction.GetOtherLayerMasks();
    }
}
