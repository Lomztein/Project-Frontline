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
    private List<IWeapon> _weaponCache;

    public event Action<Unit, IWeapon, Projectile, IDamagable> OnKill;

    public IEnumerable<IWeapon> GetWeapons ()
    {
        if (_weaponCache == null)
        {
            _weaponCache = new List<IWeapon>();
            for (int i = 0; i < _weapons.Length; i++)
            {
                _weaponCache.Add(_weapons[i].GetComponent<IWeapon>());
            }
        }
        return _weaponCache;
    }

    public void AddWeapon(IWeapon weapon)
        => _weaponCache.Add(weapon);

    private void Awake()
    {
        GetWeapons();
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
