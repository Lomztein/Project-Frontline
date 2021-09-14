﻿using System;

public interface IWeapon
{
    event Action OnFire;

    bool CanFire();
    void TryFire(ITarget intendedTarget);

    float Damage { get; }
    float Firerate { get; }

    DamageMatrix.Damage DamageType { get; }
}

public static class WeaponExtensions
{
    public static float GetDPS(this IWeapon weapon) => weapon.Damage * weapon.Firerate;
}