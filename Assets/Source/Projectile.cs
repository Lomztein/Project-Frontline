using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Projectile : MonoBehaviour, IFactionComponent, IPoolObject
{
    [HideInInspector] public float Speed;
    [HideInInspector] public float Damage;
    [HideInInspector] public DamageArmorMapping.Damage DamageType;
    [HideInInspector] public Vector3 Velocity;
    public ITarget Target;

    public float Life;
    public Effect HitEffect;
    public Effect TrailEffect;
    public float EffectRecallTime;

    private const int TerrainLayerMask = 1 << 8;
    private LayerMask _hitLayer;

    public bool IsAvailable => !gameObject.activeSelf && !AreEffectsPlaying();
    public GameObject GameObject => gameObject;

    public void Fire(Vector3 direction)
    {
        Invoke(nameof(End), Life);
        transform.LookAt(transform.position + direction);
        Velocity = Speed * direction;
    }
    private void FixedUpdate()
    {
        transform.position += Velocity * Time.fixedDeltaTime;
        if (Physics.Raycast(transform.position, transform.forward * Speed * Time.fixedDeltaTime, out RaycastHit hit, Speed * Time.fixedDeltaTime, _hitLayer | TerrainLayerMask))
        {
            var damagable = hit.collider.GetComponentInParent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(new DamageInfo (Damage, DamageType, hit.point, Velocity.normalized));
            }

            Hit(hit.point, hit.normal);
            End();
        }
    }

    private void Hit(Vector3 point, Vector3 normal)
    {
        if (HitEffect)
        {
            HitEffect.transform.position = point;
            HitEffect.transform.forward = normal;

            HitEffect.Detatch();
            HitEffect.Play();
            HitEffect.Recall(transform, EffectRecallTime);
        }

        if (TrailEffect)
        {
            TrailEffect.transform.position = point;
        }
    }

    private void End()
    {
        if (TrailEffect)
        {
            TrailEffect.Detatch();
            TrailEffect.Stop();
            TrailEffect.Recall(transform, EffectRecallTime);
        }

        CancelInvoke();
        gameObject.SetActive(false);
    }

    private bool AreEffectsPlaying ()
    {
        if (HitEffect && HitEffect.IsPlaying)
        {
            return true;
        }
        if (TrailEffect && TrailEffect.IsPlaying)
        {
            return true;
        }
        return false;
    }

    public void SetFaction(Faction faction)
    {
        _hitLayer = faction.GetOtherLayerMasks();
    }

    public void OnInstantiated()
    {
    }

    public void OnEnabled()
    {
        gameObject.SetActive(true);

        if (HitEffect)
        {
            HitEffect.Attach(transform);
        }
        if (TrailEffect)
        {
            TrailEffect.Attach(transform);
            TrailEffect.Play();
        }
    }

    public void Dispose()
    {
        Destroy(gameObject);
        Destroy(HitEffect.gameObject);
        Destroy(TrailEffect.gameObject);
    }
}
