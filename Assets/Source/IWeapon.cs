﻿using System;
using UnityEngine;

public interface IWeapon
{

    bool CanFire();
    bool TryFire(ITarget intendedTarget);

    float Damage { get; }
    float Firerate { get; }
    float Speed { get; }

    int Ammo { get; }
    int MaxAmmo { get; }

    float GetDPS();

    void SetHitLayerMask(LayerMask mask);

    DamageMatrix.Damage DamageType { get; }

    event Action<IWeapon> OnFire;
    event Action<IWeapon, Projectile> OnProjectile;
    event Action<IWeapon, Projectile, Collider, Vector3, Vector3> OnHit;
    event Action<IWeapon, Projectile, IDamagable, DamageInfo> OnDoDamage;
    event Action<IWeapon, Projectile, IDamagable, DamageInfo> OnDamageDone;
    event Action<IWeapon, Projectile, IDamagable> OnKill;
}

public static class WeaponUtils
{
    public static float GetDPSOrOverride (this IWeapon weapon)
    {
        if (weapon is Component component && component.TryGetComponent(out IWeaponDPSOverride @override))
        {
            return @override.GetDPS();
        }
        return weapon.GetDPS();
    }
}