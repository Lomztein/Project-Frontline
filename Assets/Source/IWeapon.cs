using System;
using UnityEngine;

public interface IWeapon
{

    bool CanFire();
    bool TryFire(ITarget intendedTarget);

    float Damage { get; }
    float Firerate { get; }
    float Speed { get; }

    DamageMatrix.Damage DamageType { get; }

    event Action<IWeapon> OnFire;
    event Action<IWeapon, Projectile> OnProjectile;
    event Action<IWeapon, Projectile, Collider, Vector3, Vector3> OnHit;
    event Action<IWeapon, Projectile, IDamagable> OnKill;
}

public static class WeaponExtensions
{
    public static float GetDPS(this IWeapon weapon) => weapon.Damage * weapon.Firerate;
}