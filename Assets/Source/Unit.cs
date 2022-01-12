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
    public IWeapon[] Weapons;

    public event Action<Unit, IWeapon, Projectile, IDamagable> OnKill;

    private void Awake()
    {
        Weapons = _weapons.Select(x => x.GetComponent<IWeapon>()).ToArray();
        foreach (IWeapon weapon in Weapons)
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
