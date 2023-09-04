using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour, IPurchasable
{
    public const float ENGAGE_TIME = 10f;
    public UnitInfo Info;

    public string Name => Info.Name;
    public string Description => Info.Description;
    public Sprite Sprite => null;
    public Health Health;

    [SerializeField] private GameObject[] _weapons;
    private List<IWeapon> _weaponCache;

    public event Action<Unit, IWeapon, Projectile, IDamagable> OnKill;

    private float _lastEngageTime;

    public bool IsEngaged => _lastEngageTime + ENGAGE_TIME > Time.time;

    public int BaseCost => Info.Cost;

    [Header("Parts")] // UnitParts are specifically designated parts of the unit that might be of interest to others.
    public UnitPart[] Parts;
    public WeakpointUnitPart[] Weakpoints;

    public IEnumerable<T> GetParts<T>() where T : UnitPart
        => Parts.Where(x => x != null).Select(x => x as T).Where(x => x != null);

    public int GetCost (Commander commander)
    {
        int cost = Info.Cost;
        IUnitCostModifier[] modifiers = GetComponents<IUnitCostModifier>();
        foreach (var modifier in modifiers)
        {
            cost = modifier.Modify(cost, this, commander);
        }
        return cost;
    }

    public bool CanPurchase(Commander commander)
    {
        IUnitPurchasePredicate[] predicates = GetComponents<IUnitPurchasePredicate>();
        return predicates.All(x => x.CanPurchase(this, commander));
    }

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

    private void CheckBattlefieldBounds()
    {
        // Kill the unit if they go way too far out of bounds.
        if (!MatchSettings.Current.MapInfo.Contains(transform.position / 2f))
        {
            Health.TakeDamage(new DamageInfo(Health.MaxHealth / 3f, DamageMatrix.Damage.PointDefense, transform.position, transform.forward));
        }
    }

    public void AddWeapon(IWeapon weapon)
        => _weaponCache.Add(weapon);

    private void Awake()
    {
        Weakpoints = GetParts<WeakpointUnitPart>().ToArray();

        GetWeapons();
        Health = GetComponent<Health>();
        InvokeRepeating(nameof(CheckBattlefieldBounds), 10f, 10f);
    }

    private void Start()
    {
        Health.OnDamageTaken += Health_OnDamageTaken;

        foreach (IWeapon weapon in _weaponCache)
        {
            weapon.OnKill += Weapon_OnKill;
            weapon.OnHit += Weapon_OnHit;
        }
    }

    private void Health_OnDamageTaken(Health arg1, float arg2)
    {
        _lastEngageTime = Time.time;
    }

    private void Weapon_OnHit(IWeapon arg1, Projectile arg2, Collider arg3, Vector3 arg4, Vector3 arg5)
    {
        _lastEngageTime = Time.time;
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
