using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour, IPurchasable
{
    public UnitInfo Info;

    public string Name => Info.Name;
    public string Description => Info.Description;
    public Sprite Sprite => null;
    public int Cost => Info.Cost;

    [SerializeField] private GameObject[] _weapons;
    private IWeapon[] _weaponCache;

    public event Action<Unit, IWeapon, Projectile, IDamagable> OnKill;

    public IWeapon[] GetWeapons ()
    {
        if (_weaponCache == null)
        {
            _weaponCache = new IWeapon[_weapons.Length];
            for (int i = 0; i < _weaponCache.Length; i++)
            {
                _weaponCache[i] = _weapons[i].GetComponent<IWeapon>();
            }
        }
        return _weaponCache;
    }

    private void Awake()
    {
        _weaponCache = _weapons.Select(x => x.GetComponent<IWeapon>()).ToArray();
        foreach (IWeapon weapon in _weaponCache)
        {
            weapon.OnKill += Weapon_OnKill;
        }
    }

    private void Weapon_OnKill(IWeapon arg1, Projectile arg2, IDamagable arg3)
    {
        OnKill?.Invoke(this, arg1, arg2, arg3);
    }

    public WeaponInfo[] GetWeaponInfo()
    {
        return GetComponentsInChildren<WeaponInfo>();
    }
}
