using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WeaponGroup : MonoBehaviour, IWeapon
{
    public GameObject[] Weapons;
    private IWeapon[] _weapons;
    private IFireControl _fireControl = new NoFireControl();

    public float Damage => GetWeapons().First().Damage;
    public float Firerate => GetWeapons().First().Firerate * _weapons.Length;
    public float Speed => GetWeapons().First().Speed;

    public DamageMatrix.Damage DamageType => GetWeapons().First().DamageType;

    public event Action<IWeapon> OnFire;
    public event Action<IWeapon, Projectile> OnProjectile;
    public event Action<IWeapon, Projectile, Collider, Vector3, Vector3> OnHit;
    public event Action<IWeapon, Projectile, IDamagable> OnKill;

    public enum CanFireBehaviour { First, Any, All }
    public CanFireBehaviour CanFireWhen;

    public float GetDPS() => GetWeapons().First().GetDPSOrOverride() * _weapons.Length;

    private void Awake()
    {
        GetWeapons();
        IFireControl replacement = GetComponent<IFireControl>();
        if (replacement != null)
        {
            _fireControl = replacement;
        }

        foreach (IWeapon weapon in _weapons)
        {
            weapon.OnFire += Weapon_OnFire;
            weapon.OnProjectile += Weapon_OnProjectile;
            weapon.OnHit += Weapon_OnHit;
            weapon.OnKill += Weapon_OnKill;
        }
    }

    private void Weapon_OnKill(IWeapon arg1, Projectile arg2, IDamagable arg3)
    {
        OnKill?.Invoke(arg1, arg2, arg3);
    }

    private void Weapon_OnHit(IWeapon arg1, Projectile arg2, Collider arg3, Vector3 arg4, Vector3 arg5)
    {
        OnHit?.Invoke(arg1, arg2, arg3, arg4, arg5);
    }

    private void Weapon_OnProjectile(IWeapon arg1, Projectile arg2)
    {
        OnProjectile?.Invoke(arg1, arg2);
    }

    private void Weapon_OnFire(IWeapon obj)
    {
        OnFire?.Invoke(obj);
    }

    private IWeapon[] GetWeapons()
    {
        if (_weapons == null)
        {
            _weapons = Weapons.Select(x => x.GetComponent<IWeapon>()).ToArray();
        }
        return _weapons;
    }

    public bool CanFire()
    {
        switch (CanFireWhen)
        {
            case CanFireBehaviour.First: return _weapons.First().CanFire();
            case CanFireBehaviour.Any: return _weapons.Any(x => x.CanFire());
            case CanFireBehaviour.All: return _weapons.All(x => x.CanFire());
        }
        return false;
    }

    public bool TryFire(ITarget intendedTarget)
    {
        if (CanFire())
        {
            _fireControl.Fire(_weapons.Length, (index) => FireControlCallback(index, intendedTarget));
            return true;
        }
        return false;
    }

    private void FireControlCallback(int index, ITarget intendedTarget)
    {
        _weapons[index].TryFire(intendedTarget);
    }

    public void SetHitLayerMask(LayerMask mask)
    {
        foreach (var weapon in _weapons)
        {
            weapon.SetHitLayerMask(mask);
        }
    }

    private class NoFireControl : IFireControl
    {
        public void Fire(int amount, Action<int> callback)
        {
            for (int i = 0; i < amount; i++)
            {
                callback(i);
            }
        }
    }
}
