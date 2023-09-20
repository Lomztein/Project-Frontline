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

    private float _effeciencyScore;
    private float _effeciencyTarget;
    public float Effeciency => _effeciencyScore / _effeciencyTarget;

    private Dictionary<string, float> _stats = new Dictionary<string, float>();

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

    public string GetCostModifierDesription(Commander commander)
        => string.Join("\n", GetComponents<IUnitCostModifier>().Select(x => x.GetDescription(BaseCost, this, commander)).Where(x => !string.IsNullOrWhiteSpace(x))).Trim();

    public bool CanPurchase(Commander commander)
    {
        IUnitPurchasePredicate[] predicates = GetComponents<IUnitPurchasePredicate>();
        var any = predicates.Where(x => x.Behaviour == IUnitPurchasePredicate.AnyAll.Any);
        var all = predicates.Where(x => x.Behaviour == IUnitPurchasePredicate.AnyAll.All);
        return all.All(x => x.CanPurchase(this, commander)) && (!any.Any() || any.Any(x => x.CanPurchase(this, commander)));
    }

    public string GetCanPurchaseDesription(Commander commander)
        => string.Join("\n", GetComponents<IUnitPurchasePredicate>().Select(x => x.GetDescription(this, commander)).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()).Trim();

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
            Health.TakeDamage(new DamageInfo(Health.MaxHealth / 3f, DamageModifier.One, transform.position, transform.forward, this, Health));
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

    public float AddToEffeciencyScore(float score) 
        => _effeciencyScore += score;

    public float AddToEffeciencyTarget(float score)
        => _effeciencyTarget += score;

    private void Start()
    {
        Health.OnDamageTaken += Health_OnDamageTaken;

        foreach (IWeapon weapon in _weaponCache)
        {
            weapon.OnKill += Weapon_OnKill;
            weapon.OnHit += Weapon_OnHit;
        }
    }

    private void Health_OnDamageTaken(Health arg1, DamageInfo info)
    {
        _lastEngageTime = Time.time;
    }

    public Dictionary<string, float> GetStats()
        => new Dictionary<string, float>(_stats);

    public float SetStat(string stat, float value)
    {
        _stats[stat] = value;
        return _stats[stat];
    }

    public float? GetStat(string stat)
    {
        if (_stats.TryGetValue(stat, out var value)) return value;
        return null;
    }

    public float ChangeStat(string stat, float value)
    {
        float val = GetStat(stat) ?? 0;
        return SetStat(stat, val + value);
    }

    private void Weapon_OnHit(IWeapon arg1, Projectile arg2, Collider arg3, Vector3 arg4, Vector3 arg5)
    {
        _lastEngageTime = Time.time;
    }

    private void Weapon_OnKill(IWeapon arg1, Projectile arg2, IDamagable arg3)
    {
        OnKill?.Invoke(this, arg1, arg2, arg3);
        ChangeStat("Kills", 1);
    }

    public WeaponInfo[] GetWeaponInfo()
    {
        return GetComponentsInChildren<WeaponInfo>();
    }
}
