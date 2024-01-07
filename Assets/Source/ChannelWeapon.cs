using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChannelWeapon : MonoBehaviour, IWeapon
{
    public abstract float Damage { get; }
    public virtual float Firerate => 1 / Time.fixedDeltaTime;
    public abstract float Speed { get; }
    public virtual int Ammo => Mathf.RoundToInt(Mathf.Clamp((Time.time - _channelStartTime + MaxChannelTime) * Firerate, 0, MaxAmmo));
    public virtual int MaxAmmo => Mathf.RoundToInt(MaxChannelTime * Firerate);
    public abstract DamageModifier Modifier { get; }

    public event Action<IWeapon> OnFire;
    public event Action<IWeapon, Projectile> OnProjectile;
    public event Action<IWeapon, Projectile, Collider, Vector3, Vector3> OnHit;
    public event Action<IWeapon, Projectile, IDamagable, DamageInfo> OnDoDamage;
    public event Action<IWeapon, Projectile, IDamagable, DamageInfo> OnDamageDone;
    public event Action<IWeapon, Projectile, IDamagable> OnKill;

    public enum State { Ready, Channeling, Recharging }

    public float RechargeTime;
    public float MaxChannelTime;
    public State ChannelState;

    public float ChannelEndDelay = 0.1f;
    private float _lastChannelTime;
    private float _channelStartTime;
    private float _channelEndTime;

    private bool _isFiring;
    protected ITarget CurrentChannelTarget { get; private set; }
    protected LayerMask HitLayerMask { get; private set; }

    public event Action<ChannelWeapon, ITarget> OnChannelBegin;
    public event Action<ChannelWeapon, ITarget> OnChannelEnd;

    public virtual bool CanFire()
    {
        return ChannelState == State.Ready 
            || ChannelState == State.Channeling;
    }

    public virtual void SetHitLayerMask(LayerMask mask)
    {
        HitLayerMask = mask;
    }

    public virtual bool TryFire(ITarget intendedTarget)
    {
        if (CanFire())
        {
            _isFiring = true;
            _lastChannelTime = Time.time;

            if (CurrentChannelTarget.ExistsAndValid() && intendedTarget != CurrentChannelTarget)
            {
                SwapTarget(CurrentChannelTarget, intendedTarget);
            }
            CurrentChannelTarget = intendedTarget;
            return true;
        }
        return false;
    }

    protected void Start()
    {
        Health health = GetComponentInParent<Health>();
        if (health != null)
        {
            health.OnDeath += Health_OnDeath;
        }
    }

    private void Health_OnDeath(Health obj)
    {
        if (CurrentChannelTarget.ExistsAndValid())
        {
            EndChannel(CurrentChannelTarget);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (_isFiring && Time.time > _lastChannelTime + ChannelEndDelay)
        {
            _isFiring = false;
        }

        if (ChannelState == State.Ready)
        {
            HandleReadyState(Time.fixedDeltaTime);
        }
        if (ChannelState == State.Channeling)
        {
            HandleChannelState(Time.fixedDeltaTime);
        }
        if (ChannelState == State.Recharging) 
        {
            HandleRechargeState(Time.fixedDeltaTime);
        }
    }

    private void HandleReadyState(float dt)
    {
        if (_isFiring)
        {
            _channelStartTime = Time.time;
            SetState(State.Channeling);
            BeginChannel(CurrentChannelTarget);
        }
    }

    private void HandleChannelState (float dt)
    {
        if (_isFiring)
        {
            if (Time.time > _channelStartTime + MaxChannelTime)
            {
                _channelEndTime = Time.time;
                EndChannel(CurrentChannelTarget);
                SetState(State.Recharging);
            }
        }
        if (!_isFiring || !CurrentChannelTarget.ExistsAndValid())
        {
            _channelEndTime = Time.time;
            EndChannel(CurrentChannelTarget);
            SetState(State.Recharging);
            CurrentChannelTarget = null;
        }
    }

    private void HandleRechargeState(float dt)
    {
        _isFiring = false;
        if (Time.time > _channelEndTime + RechargeTime)
        {
            SetState(State.Ready);
        }
    }

    private void SwapTarget(ITarget oldTarget, ITarget newTarget)
    {
        EndChannel(oldTarget);
        BeginChannel(newTarget);
    }

    private void SetState(State state)
        => ChannelState = state;

    public abstract void BeginChannel(ITarget target);

    public abstract void EndChannel(ITarget target);

    public float GetDPS()
        => Damage * Firerate;
}
