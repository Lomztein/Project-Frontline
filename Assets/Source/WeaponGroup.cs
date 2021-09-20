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

    public event Action OnFire;

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
            weapon.OnFire += OnWeaponFire;
        }
    }

    private IWeapon[] GetWeapons()
    {
        if (_weapons == null)
        {
            _weapons = Weapons.Select(x => x.GetComponent<IWeapon>()).ToArray();
        }
        return _weapons;
    }

    private void OnWeaponFire()
    {
        OnFire?.Invoke();
    }

    public bool CanFire() => _weapons.All(x => x.CanFire());

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
