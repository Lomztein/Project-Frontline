using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolObject
{
    [HideInInspector] public float Speed;
    [HideInInspector] public float Damage;
    [HideInInspector] public DamageMatrix.Damage DamageType;
    [HideInInspector] public Vector3 Velocity;
    public ITarget Target;

    public float Life;
    public Effect HitEffect;
    public Effect TrailEffect;
    public float EffectRecallTime;

    protected const int TerrainLayerMask = 1 << 8;
    public LayerMask HitLayerMask;

    public bool IsAvailable => !gameObject.activeSelf && !AreEffectsPlaying(); 
    public GameObject GameObject => gameObject;

    public event Action<Projectile, Vector3> OnFired;
    public event Action<Projectile, Collider, Vector3, Vector3> OnHit;
    public event Action<Projectile, IDamagable> OnKill;
    public event Action<Projectile> OnEnd;

    public virtual void Fire(Vector3 direction)
    {
        Invoke(nameof(End), Life);
        transform.LookAt(transform.position + direction);
        Velocity = Speed * direction;
        OnFired?.Invoke(this, direction);
    }
    protected virtual void FixedUpdate()
    {
        transform.position += Velocity * Time.fixedDeltaTime;
        float dist = Speed * Time.fixedDeltaTime + 0.2f;
        if (Physics.Raycast(transform.position, transform.forward * dist, out RaycastHit hit, dist, HitLayerMask | TerrainLayerMask))
        {
            DoDamage(hit.collider, hit.point);
            Hit(hit.point, hit.normal);
            End();

            OnHit?.Invoke(this, hit.collider, hit.point, hit.normal);
        }
    }

    protected virtual void DoDamage (Collider col, Vector3 point)
    {
        var damagable = col.GetComponentInParent<IDamagable>();
        DoDamage(damagable, Damage, DamageType, point, Velocity.normalized);
    }

    public void DoDamage (IDamagable damagable, float damage, DamageMatrix.Damage type, Vector3 point, Vector3 direction)
    {
        if (damagable != null)
        {
            if (damagable.TakeDamage(new DamageInfo(damage, type, point, direction)) <= 0f)
            {
                OnKill?.Invoke(this, damagable);
            }
        }
    }

    public virtual void Hit(Vector3 point, Vector3 normal)
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
        return false;
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
