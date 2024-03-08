using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolObject
{
    [HideInInspector] public float Speed;
    [HideInInspector] public float Damage;
    [HideInInspector] public DamageModifier Modifier;
    [HideInInspector] public Vector3 Velocity;
    public ITarget Target;

    public float Life;
    public Effect HitEffect;
    public Effect TrailEffect;
    public float EffectRecallTime;
    private float _endTime;

    protected const int TerrainLayerMask = 1 << 8;
    public LayerMask HitLayerMask;

    public bool IsAvailable => !gameObject.activeSelf && !AreEffectsPlaying(); 
    public GameObject GameObject => gameObject;

    public event Action<Projectile, Vector3> OnFired; // The projectile is fired
    public event Action<Projectile, Collider, Vector3, Vector3> OnHit; // The projectile hits something directly
    public event Action<Projectile, IDamagable, DamageInfo> OnDoDamage; // The projectile does damage to something
    public event Action<Projectile, IDamagable, DamageInfo> OnDamageDone; // The projectile is done doing damage to something
    public event Action<Projectile, IDamagable> OnKill; // The projectile kills something
    public event Action<Projectile> OnEnd; // The projectile is finished and stored for reuse

    public virtual void Fire(Vector3 direction)
    {
        Invoke(nameof(End), Life);
        transform.LookAt(transform.position + direction);
        Velocity = Speed * direction;
        OnFired?.Invoke(this, direction);
    }

    protected virtual void FixedUpdate()
    {
        float dist = Speed * Time.fixedDeltaTime + 0.2f;
        if (Physics.Raycast(transform.position, transform.forward * dist, out RaycastHit hit, dist, HitLayerMask | TerrainLayerMask))
        {
            DoDamage(hit.collider, hit.point);
            HandleHitEffects(hit.point, hit.normal);
            InvokeOnHit(hit.collider, hit.point, hit.normal);
            End();
        }
        transform.position += Velocity * Time.fixedDeltaTime;
    }

    protected virtual void DoDamage (Collider col, Vector3 point)
    {
        var damagable = col.GetComponentInParent<IDamagable>();
        DoDamage(damagable, Damage, Modifier, point, Velocity.normalized);
    }

    protected virtual void DoDamage(Collider col, float damage, Vector3 point)
    {
        var damagable = col.GetComponentInParent<IDamagable>();
        DoDamage(damagable, damage, Modifier, point, Velocity.normalized);
    }

    public void DoDamage (IDamagable damagable, float damage, DamageModifier modifier, Vector3 point, Vector3 direction)
    {
        if (damagable != null)
        {
            var info = new DamageInfo(damage, modifier, point, direction, this, damagable);
            OnDoDamage?.Invoke(this, damagable, info);
            if (damagable.TakeDamage(info) <= 0f && info.KilledTarget)
            {
                OnKill?.Invoke(this, damagable);
            }
            OnDamageDone?.Invoke(this, damagable, info);
        }
    }

    protected void InvokeOnHit (Collider hitCollider, Vector3 hitPoint, Vector3 hitDirection)
    {
        OnHit?.Invoke(this, hitCollider, hitPoint, hitDirection);
    }

    public virtual void HandleHitEffects(Vector3 point, Vector3 normal)
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

    public virtual void End()
    {
        if (TrailEffect)
        {
            TrailEffect.Detatch();
            TrailEffect.Stop();
            TrailEffect.Recall(transform, EffectRecallTime);
        }

        _endTime = Time.time;
        CancelInvoke();
        gameObject.SetActive(false);
        OnEnd?.Invoke(this);
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
        return Time.time < _endTime + EffectRecallTime + 0.1;
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
